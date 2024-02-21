using Katuusagi.CSharpScriptGenerator;
using Katuusagi.SourceGeneratorCommon;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Katuusagi.MemoizationForUnity.SourceGenerator
{
    public enum ThreadSafeType
    {
        None,
        Concurrent,
        ThreadStatic,
    }

    public enum CachingStyle
    {
        Direct,
        NoneKey,
        SingleKey,
        MultipleKey,
    }

    [Generator]
    public class MemoizationGenerator : ISourceGenerator
    {
        private class FlagsComparer : IEqualityComparer<bool[]>
        {
            public static readonly FlagsComparer Default = new FlagsComparer();

            bool IEqualityComparer<bool[]>.Equals(bool[] x, bool[] y)
            {
                return x.SequenceEqual(y);
            }

            int IEqualityComparer<bool[]>.GetHashCode(bool[] obj)
            {
                // 引数の数が32を超えることはほぼないだろうからこれで
                uint hash = 0;
                var min = Math.Min(obj.Length, 32);
                for (int i = 0; i < min; ++i)
                {
                    hash |= 1U << (i % 32);
                }

                return (int)(hash ^ obj.Length);
            }
        }
        private struct RootInfo
        {
            public List<TypeInfo> TypeInfos;
            public HashSet<int> AdditionalDefaultComparers;
            public HashSet<int> AdditionalParamsComparers;
            public HashSet<bool[]> ArrayElementComparers;
        }

        private struct TypeInfo
        {
            public AncestorInfo[] Ancestors;
            public string[] Generics;
            public string[] Usings;
            public ModifierType Modifier;
            public string NameSpace;
            public string Name;
            public List<MethodInfo> Methods;

            public string FileName;
            private string _FileName
            {
                get
                {
                    var result = Name;
                    if (!string.IsNullOrEmpty(AncestorPath))
                    {
                        result = $"{AncestorPath}-{result}";
                    }

                    if (!string.IsNullOrEmpty(NameSpace))
                    {
                        result = $"{NameSpace}.{result}";
                    }

                    return $"{result}.Memoization.Generated";
                }
            }

            public string AncestorPath;
            private string _AncestorPath
            {
                get
                {
                    return string.Join("-", Ancestors.Select(v => v.Name));
                }
            }

            public bool HasGenericClearableStaticTypeCache;
            private bool _HasGenericClearableStaticTypeCache
            {
                get
                {
                    return Methods.Any(v => v.IsClearableGenericStaticTypeCache && !v.IsConcurrent);
                }
            }

            public bool HasThreadSafeGenericClearableStaticTypeCache;
            private bool _HasThreadSafeGenericClearableStaticTypeCache
            {
                get
                {
                    return Methods.Any(v => v.IsClearableGenericStaticTypeCache && v.IsConcurrent);
                }

            }

            public void Init()
            {
                FileName = _FileName;
                AncestorPath = _AncestorPath;
                HasGenericClearableStaticTypeCache = _HasGenericClearableStaticTypeCache;
                HasThreadSafeGenericClearableStaticTypeCache = _HasThreadSafeGenericClearableStaticTypeCache;
            }
        }

        private struct AncestorInfo
        {
            public ModifierType Modifier;
            public string Name;
            public string[] Generics;
        }

        private struct MethodInfo
        {
            private static readonly HashSet<string> SimpleKeys = new HashSet<string>()
            {
                "bool",
                "Boolean",
                "System.Boolean",
                "global::System.Boolean",
                "sbyte",
                "Int8",
                "System.Int8",
                "global::System.Int8",
                "byte",
                "UInt8",
                "System.UInt8",
                "global::System.UInt8",
                "short",
                "Int16",
                "System.Int16",
                "global::System.Int16",
                "ushort",
                "UInt16",
                "System.UInt16",
                "global::System.UInt16",
                "int",
                "Int32",
                "System.Int32",
                "global::System.Int32",
                "uint",
                "UInt32",
                "System.UInt32",
                "global::System.UInt32",
                "long",
                "Int64",
                "System.Int64",
                "global::System.Int64",
                "ulong",
                "UInt64",
                "System.UInt64",
                "global::System.UInt64",
                "float",
                "Single",
                "System.Single",
                "global::System.Single",
                "double",
                "Double",
                "System.Double",
                "global::System.Double",
                "char",
                "Char",
                "System.Char",
                "global::System.Char",
            };

            public string Id;
            public string[] Attributes;
            public ModifierType Modifier;
            public string Name;
            public string RawName;
            public bool CompareArrayElement;
            public string CacheComparer;
            public bool IsClearable;
            public ThreadSafeType ThreadSafeType;
            public string ReturnType;
            public bool HasGenericTypeInReturn;
            public string InterruptCacheMethod;
            public string OnCachedMethod;
            public List<GenericParameterInfo> GenericParameters;
            public List<ParameterInfo> Parameters;
            public string ParameterArrayFlag;

            public ParameterInfo[] OutputParameters;
            private IEnumerable<ParameterInfo> _OutputParameters
            {
                get
                {
                    return Parameters.Where(v => v.IsOutput);
                }
            }

            public ParameterInfo[] InputParameters;
            private IEnumerable<ParameterInfo> _InputParameters
            {
                get
                {
                    return Parameters.Where(v => v.IsInput);
                }
            }

            public bool HasGenericParameter;
            private bool _HasGenericParameter
            {
                get
                {
                    return GenericParameters.Any();
                }
            }

            public bool HasInputParameter;
            private bool _HasInputParameter
            {
                get
                {
                    return InputParameters.Any();
                }
            }

            public bool HasOutputParameter;
            private bool _HasOutputParameter
            {
                get
                {
                    return OutputParameters.Any();
                }
            }

            public bool HasReturnType;
            private bool _HasReturnType
            {
                get
                {
                    return !string.IsNullOrEmpty(ReturnType) &&
                            ReturnType != "void" &&
                            ReturnType != "Void" &&
                            ReturnType != "System.Void" &&
                            ReturnType != "global::System.Void";
                }
            }

            public bool HasKey;
            private bool _HasKey
            {
                get
                {
                    return HasGenericParameter || HasInputParameter;
                }
            }

            public bool HasGenericTypeInParameters;
            private bool _HasGenericTypeInParameters
            {
                get
                {
                    return Parameters.Any(v => v.HasGenericType);
                }
            }

            public bool HasParams;
            private bool _HasParams
            {
                get
                {
                    return Parameters.LastOrDefault().Modifier?.Contains("params") ?? false;
                }
            }

            public bool IsStatic;
            private bool _IsStatic
            {
                get
                {
                    return Modifier.HasFlag(ModifierType.Static);
                }
            }

            public bool IsUseStaticCache;
            private bool _IsUseStaticCache
            {
                get
                {
                    return IsStatic;
                }
            }

            public bool IsUseStaticTypeCache;
            private bool _IsUseStaticTypeCache
            {
                get
                {
                    return IsUseStaticCache && (HasGenericParameter || (!HasKey && !IsClearable));
                }
            }

            public bool IsUseBaseTypeCache;
            private bool _IsUseBaseTypeCache
            {
                get
                {
                    return !IsStatic && HasGenericParameter && (HasGenericTypeInReturn || HasGenericTypeInParameters);
                }
            }

            public bool IsThreadStatic;
            private bool _IsThreadStatic
            {
                get
                {
                    return IsStatic && ThreadSafeType == ThreadSafeType.ThreadStatic;
                }
            }

            public bool IsConcurrent;
            private bool _IsConcurrent
            {
                get
                {
                    return ThreadSafeType == ThreadSafeType.Concurrent;
                }
            }

            public bool IsClearableGenericStaticTypeCache;
            private bool _IsClearableGenericStaticTypeCache
            {
                get
                {
                    return IsClearable && IsUseStaticTypeCache && HasGenericParameter;
                }
            }

            public bool IsUseTypeIdKey;
            private bool _IsUseTypeIdKey
            {
                get
                {
                    return !IsUseStaticTypeCache && !IsUseBaseTypeCache && HasGenericParameter;
                }
            }

            public bool IsSimpleKey;
            private bool _IsSimpleKey
            {
                get
                {
                    return SimpleKeys.Contains(KeyType);
                }
            }

            public int CacheKeyCount;
            private int _CacheKeyCount
            {
                get
                {
                    return KeyTypeArgs.Count();
                }
            }

            public CachingStyle CachingStyle;
            private CachingStyle _CachingStyle
            {
                get
                {
                    int parameterCount = CacheKeyCount;
                    if (IsUseStaticTypeCache)
                    {
                        if (parameterCount == 0)
                        {
                            if (IsClearable)
                            {
                                return CachingStyle.NoneKey;
                            }

                            return CachingStyle.Direct;
                        }
                        else if (parameterCount == 1)
                        {
                            return CachingStyle.SingleKey;
                        }

                        return CachingStyle.MultipleKey;
                    }

                    if (parameterCount == 0)
                    {
                        return CachingStyle.NoneKey;
                    }
                    else if (parameterCount == 1)
                    {
                        return CachingStyle.SingleKey;
                    }

                    return CachingStyle.MultipleKey;
                }
            }

            public int ResultCount;
            private int _ResultCount
            {
                get
                {
                    return OutputParameters.Count() + (HasReturnType ? 1 : 0);
                }
            }

            public ModifierType CacheFieldModifier;
            private ModifierType _CacheFieldModifier
            {
                get
                {
                    if (IsUseStaticCache)
                    {
                        return ModifierType.Private | ModifierType.Static;
                    }

                    return ModifierType.Private;
                }
            }

            public string StaticFieldWrappedType;
            private string _StaticFieldWrappedType
            {
                get
                {
                    if (IsUseStaticTypeCache)
                    {
                        return $"__MemoizationCacheType_{Id}__";
                    }

                    return string.Empty;
                }
            }

            public string StaticFieldWrappedTypeFullName;
            private string _StaticFieldWrappedTypeFullName
            {
                get
                {
                    if (IsUseStaticTypeCache)
                    {
                        string genericParameters = string.Empty;
                        if (HasGenericParameter)
                        {
                            genericParameters = $"<{GenericArgs}>";
                        }
                        return $"{StaticFieldWrappedType}{genericParameters}";
                    }

                    return string.Empty;
                }
            }

            public string CacheValueName;
            private string _CacheValueName
            {
                get
                {
                    if (IsUseStaticTypeCache)
                    {
                        return $"{StaticFieldWrappedTypeFullName}.Cache";
                    }

                    return $"__MemoizationCacheValue_{Id}__";
                }
            }

            public string CacheValueTypeName;
            private string _CacheValueTypeName
            {
                get
                {
                    switch (CachingStyle)
                    {
                        case CachingStyle.Direct:
                            return ResultType;
                        case CachingStyle.NoneKey:
                            if (IsConcurrent)
                            {
                                return $"global::Katuusagi.MemoizationForUnity.LockFreeCacheValue<{ResultType}>";
                            }

                            return $"global::Katuusagi.MemoizationForUnity.CacheValue<{ResultType}>";
                        case CachingStyle.SingleKey:
                        case CachingStyle.MultipleKey:
                            if (IsBoolKey)
                            {
                                if (IsConcurrent)
                                {
                                    return $"global::Katuusagi.MemoizationForUnity.LockFreeBooleanCacheValue<{ResultType}>";
                                }

                                return $"global::Katuusagi.MemoizationForUnity.BooleanCacheValue<{ResultType}>";
                            }

                            if (IsConcurrent)
                            {
                                return $"global::System.Collections.Concurrent.ConcurrentDictionary<{KeyType}, {ResultType}>";
                            }

                            return $"global::System.Collections.Generic.Dictionary<{KeyType}, {ResultType}>";
                    }

                    return string.Empty;
                }
            }

            public string CacheValueTypeDeclarationName;
            private string _CacheValueTypeDeclarationName
            {
                get
                {
                    if (IsUseBaseTypeCache)
                    {
                        if (IsBoolKey)
                        {
                            if (IsConcurrent)
                            {
                                return $"global::Katuusagi.MemoizationForUnity.LockFreeBooleanCacheValue<{ResultType}>";
                            }

                            return $"global::Katuusagi.MemoizationForUnity.BooleanCacheValue<{ResultType}>";
                        }

                        if (IsConcurrent)
                        {
                            return $"global::System.Collections.Concurrent.ConcurrentDictionary<{GenericKeyType}, System.Collections.IDictionary>";
                        }

                        return $"global::System.Collections.Generic.Dictionary<{GenericKeyType}, System.Collections.IDictionary>";
                    }

                    return CacheValueTypeName;
                }
            }

            public string CallRawMethod;
            private string _CallRawMethod
            {
                get
                {
                    string genericArgs = string.Empty;
                    if (HasGenericParameter)
                    {
                        genericArgs = $"<{GenericArgs}>";
                    }

                    return $"{RawName}{genericArgs}({ConcatArgs})";
                }
            }

            public string CallAndDeclairRawMethod;
            private string _CallAndDeclairRawMethod
            {
                get
                {
                    string genericArgs = string.Empty;
                    if (HasGenericParameter)
                    {
                        genericArgs = $"<{GenericArgs}>";
                    }

                    return $"{RawName}{genericArgs}({ConcatDeclairArgs})";
                }
            }

            public string OnCachedMethodWithGeneric;
            private string _OnCachedMethodWithGeneric
            {
                get
                {
                    string genericArgs = string.Empty;
                    if (HasGenericParameter)
                    {
                        genericArgs = $"<{GenericArgs}>";
                    }

                    return $"{OnCachedMethod}{genericArgs}";
                }
            }

            public string FieldInitializer;
            private string _FieldInitializer
            {
                get
                {
                    if (IsUseBaseTypeCache)
                    {
                        return $"new {CacheValueTypeDeclarationName}()";
                    }

                    return FieldValue;
                }
            }

            public string FieldValue;
            private string _FieldValue
            {
                get
                {
                    switch (CachingStyle)
                    {
                        case CachingStyle.Direct:
                            return CallRawMethod;
                        case CachingStyle.NoneKey:
                            return $"new {CacheValueTypeName}()";
                        case CachingStyle.SingleKey:
                        case CachingStyle.MultipleKey:
                            return $"new {CacheValueTypeName}({Comparer})";
                    }

                    return null;
                }
            }

            public string GenericArgs;
            private string _GenericArgs
            {
                get
                {
                    return GenericParameters.Select(v => v.Type).JoinParameters();
                }
            }

            public string DeclarationKey;
            private string _DeclarationKey
            {
                get
                {
                    if (!IsUseBaseTypeCache)
                    {
                        return string.Empty;
                    }

                    return $"{CalcTypeId(0)}.Id";
                }
            }

            public string Key;
            private string _Key
            {
                get
                {
                    var parameters = InputParameters.Select(v => v.Name);
                    if (IsUseTypeIdKey)
                    {
                        parameters = parameters.Prepend($"{CalcTypeId(0)}.Id");
                    }

                    var result = parameters.JoinParameters();
                    switch (CachingStyle)
                    {
                        case CachingStyle.SingleKey:
                            if (IsSimpleKey)
                            {
                                return result;
                            }
                            return $"global::System.ValueTuple.Create({result})";
                        case CachingStyle.MultipleKey:
                            return $"({result})";
                    }

                    return result;
                }
            }

            public string GenericKeyType;
            private string _GenericKeyType
            {
                get
                {
                    if (!IsUseBaseTypeCache)
                    {
                        return string.Empty;
                    }

                    return "int";
                }
            }

            public string[] KeyTypeArgs;
            private IEnumerable<string> _KeyTypeArgs
            {
                get
                {
                    if (IsUseTypeIdKey)
                    {
                        yield return "int";
                    }

                    foreach (var parameter in InputParameters)
                    {
                        yield return parameter.Type;
                    }
                }
            }

            public string KeyType;
            private string _KeyType
            {
                get
                {
                    string result = string.Empty;
                    switch (CachingStyle)
                    {
                        case CachingStyle.SingleKey:
                            result = KeyTypeArgs.JoinParameters();
                            if (SimpleKeys.Contains(result))
                            {
                                return result;
                            }
                            return $"global::System.ValueTuple<{result}>";
                        case CachingStyle.MultipleKey:
                            var parameters = InputParameters.Select(v => $"{v.Type} {v.Name}");
                            if (IsUseTypeIdKey)
                            {
                                parameters = parameters.Prepend("int __GenericKey__");
                            }

                            result = parameters.JoinParameters();
                            return $"({result})";
                    }

                    return result;
                }
            }

            public bool IsBoolKey;
            private bool _IsBoolKey
            {
                get
                {
                    return KeyType == "bool" ||
                            KeyType == "Boolean" ||
                            KeyType == "System.Boolean" ||
                            KeyType == "global::System.Boolean";
                }
            }

            public string Result;
            private string _Result
            {
                get
                {
                    var parameters = OutputParameters.Select(v => v.Name);
                    if (HasReturnType)
                    {
                        parameters = parameters.Prepend("__return__");
                    }

                    var result = parameters.JoinParameters();
                    if (ResultCount > 1)
                    {
                        return $"({result})";
                    }
                    return result;
                }
            }

            public string ResultType;
            private string _ResultType
            {
                get
                {
                    string result = string.Empty;
                    if (ResultCount > 1)
                    {
                        var parameters = OutputParameters.Select(v => $"{v.Type} {v.Name}");
                        if (HasReturnType)
                        {
                            parameters = parameters.Prepend($"{ReturnType} __return__");
                        }

                        result = parameters.JoinParameters();
                        return $"({result})";
                    }
                    else
                    {
                        var parameters = OutputParameters.Select(v => v.Type);
                        if (HasReturnType)
                        {
                            parameters = parameters.Prepend(ReturnType);
                        }

                        return parameters.JoinParameters();
                    }
                }
            }

            public string ConcatKeyTypeArgs;
            private string _ConcatKeyTypeArgs
            {
                get
                {
                    return KeyTypeArgs.JoinParameters();
                }
            }

            public string ConcatArrayElementKeyTypeArgs;
            private string _ConcatArrayElementKeyTypeArgs
            {
                get
                {
                    return KeyTypeArgs.Select(v =>
                    {
                        string a = v;
                        if (a.EndsWith("[]"))
                        {
                            a = a.Remove(a.Length - 2, 2);
                        }

                        return a;
                    }).JoinParameters();
                }
            }

            public string ConcatArgs;
            private string _ConcatArgs
            {
                get
                {
                    return Parameters.Select(v => v.ModifieredName).JoinParameters();
                }
            }

            public string ConcatDeclairArgs;
            private string _ConcatDeclairArgs
            {
                get
                {
                    return Parameters.Select(v => v.ModifieredDeclairName).JoinParameters();
                }
            }

            public string[] ResultReturnCodes;
            private string[] _ResultReturnCodes
            {
                get
                {
                    var resultCount = ResultCount;
                    string[] result;
                    if (resultCount == 1)
                    {
                        foreach (var parameter in OutputParameters)
                        {
                            result = new string[2];
                            result[0] = $"{parameter.Name} = __cache__;";
                            result[1] = $"return;";
                            return result;
                        }

                        if (HasReturnType)
                        {
                            result = new string[1];
                            result[0] = $"return __cache__;";
                            return result;
                        }
                    }

                    result = new string[OutputParameters.Count() + 1];
                    int i = 0;
                    foreach (var parameter in OutputParameters)
                    {
                        result[i] = $"{parameter.Name} = __cache__.{parameter.Name};";
                        ++i;
                    }

                    if (HasReturnType)
                    {
                        result[i] = $"return __cache__.__return__;";
                    }
                    else
                    {
                        result[i] = "return;";
                    }

                    return result;
                }
            }

            public string Comparer;
            private string _Comparer
            {
                get
                {
                    if (CacheComparer != null)
                    {
                        return CacheComparer;
                    }

                    if (IsSimpleKey || IsBoolKey)
                    {
                        return string.Empty;
                    }

                    if (CompareArrayElement)
                    {
                        return $"global::Katuusagi.MemoizationForUnity.ArrayElementEqualityComparer{ParameterArrayFlag}<{ConcatArrayElementKeyTypeArgs}>.Default";
                    }

                    var args = ConcatKeyTypeArgs;
                    if (HasParams)
                    {
                        return $"global::Katuusagi.MemoizationForUnity.ParamsEqualityComparer<{args.Remove(args.Length - 2, 2)}>.Default";
                    }

                    if (CachingStyle == CachingStyle.SingleKey)
                    {
                        return string.Empty;
                    }

                    return $"global::Katuusagi.MemoizationForUnity.MemoizationEqualityComparer<{args}>.Default";
                }
            }

            private string CalcTypeId(int start)
            {
                string genericArg = string.Empty;
                int i;
                for (i = start; i < 7 + start && i < GenericParameters.Count; ++i)
                {
                    genericArg += $"{GenericParameters[i].Type}, ";
                }

                if (i < GenericParameters.Count)
                {
                    genericArg += $"{CalcTypeId(i)}, ";
                }

                if (!string.IsNullOrEmpty(genericArg))
                {
                    genericArg = genericArg.Remove(genericArg.Length - 2);
                }

                return $"global::Katuusagi.MemoizationForUnity.Utils.MemoizationUtils.TypeId<{genericArg}>";
            }

            public void PreInit()
            {
                OutputParameters = _OutputParameters.ToArray();
                InputParameters = _InputParameters.ToArray();
                HasGenericParameter = _HasGenericParameter;
                HasInputParameter = _HasInputParameter;
                HasReturnType = _HasReturnType;
                HasKey = _HasKey;
                HasGenericTypeInParameters = _HasGenericTypeInParameters;
                IsStatic = _IsStatic;
                IsUseStaticCache = _IsUseStaticCache;
                IsUseStaticTypeCache = _IsUseStaticTypeCache;
                IsUseBaseTypeCache = _IsUseBaseTypeCache;
                IsUseTypeIdKey = _IsUseTypeIdKey;
                HasParams = _HasParams;
                KeyTypeArgs = _KeyTypeArgs.ToArray();
                CacheKeyCount = _CacheKeyCount;
            }

            public void Init()
            {
                OutputParameters = _OutputParameters.ToArray();
                InputParameters = _InputParameters.ToArray();
                HasGenericParameter = _HasGenericParameter;
                HasInputParameter = _HasInputParameter;
                HasOutputParameter = _HasOutputParameter;
                HasReturnType = _HasReturnType;
                HasKey = _HasKey;
                HasGenericTypeInParameters = _HasGenericTypeInParameters;
                HasParams = _HasParams;
                IsStatic = _IsStatic;
                IsUseStaticCache = _IsUseStaticCache;
                IsUseStaticTypeCache = _IsUseStaticTypeCache;
                IsUseBaseTypeCache = _IsUseBaseTypeCache;
                IsThreadStatic = _IsThreadStatic;
                IsConcurrent = _IsConcurrent;
                IsClearableGenericStaticTypeCache = _IsClearableGenericStaticTypeCache;
                IsUseTypeIdKey = _IsUseTypeIdKey;
                KeyTypeArgs = _KeyTypeArgs.ToArray();
                CacheKeyCount = _CacheKeyCount;
                CachingStyle = _CachingStyle;
                KeyType = _KeyType;
                IsSimpleKey = _IsSimpleKey;
                ResultCount = _ResultCount;
                CacheFieldModifier = _CacheFieldModifier;
                GenericArgs = _GenericArgs;
                StaticFieldWrappedType = _StaticFieldWrappedType;
                StaticFieldWrappedTypeFullName = _StaticFieldWrappedTypeFullName;
                ResultType = _ResultType;
                IsBoolKey = _IsBoolKey;
                GenericKeyType = _GenericKeyType;
                CacheValueName = _CacheValueName;
                CacheValueTypeName = _CacheValueTypeName;
                CacheValueTypeDeclarationName = _CacheValueTypeDeclarationName;
                ConcatArgs = _ConcatArgs;
                ConcatDeclairArgs = _ConcatDeclairArgs;
                CallRawMethod = _CallRawMethod;
                CallAndDeclairRawMethod = _CallAndDeclairRawMethod;
                OnCachedMethodWithGeneric = _OnCachedMethodWithGeneric;
                ConcatArrayElementKeyTypeArgs = _ConcatArrayElementKeyTypeArgs;
                ConcatKeyTypeArgs = _ConcatKeyTypeArgs;
                Comparer = _Comparer;
                FieldValue = _FieldValue;
                FieldInitializer = _FieldInitializer;
                DeclarationKey = _DeclarationKey;
                Key = _Key;
                Result = _Result;
                ResultReturnCodes = _ResultReturnCodes;
            }
        }

        private struct GenericParameterInfo
        {
            public string[] Attributes;
            public string[] Wheres;
            public string Type;
        }

        private struct ParameterInfo
        {
            public string[] Attributes;
            public string Modifier;
            public string Type;
            public string Name;
            public string Default;
            public bool HasGenericType;

            public string ModifieredTypeName;
            private string _ModifieredTypeName
            {
                get
                {
                    if (string.IsNullOrEmpty(Modifier))
                    {
                        return Type;
                    }

                    return $"{Modifier} {Type}";
                }
            }

            public string ModifieredName;
            private string _ModifieredName
            {
                get
                {
                    if (Modifier.Contains("ref"))
                    {
                        return $"ref {Name}";
                    }

                    if (Modifier.Contains("out"))
                    {
                        return $"out {Name}";
                    }

                    return Name;
                }
            }

            public string ModifieredDeclairName;
            private string _ModifieredDeclairName
            {
                get
                {
                    if (Modifier.Contains("ref"))
                    {
                        return $"ref {Name}";
                    }

                    if (Modifier.Contains("out"))
                    {
                        return $"out var {Name}";
                    }

                    return Name;
                }
            }

            public bool IsOutput;
            private bool _IsOutput
            {
                get
                {
                    return Modifier.Contains("ref") || Modifier.Contains("out");
                }
            }

            public bool IsInput;
            private bool _IsInput
            {
                get
                {
                    return !Modifier.Contains("out");
                }
            }

            public void Init()
            {
                ModifieredTypeName = _ModifieredTypeName;
                ModifieredName = _ModifieredName;
                ModifieredDeclairName = _ModifieredDeclairName;
                IsOutput = _IsOutput;
                IsInput = _IsInput;
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!ContextUtils.IsReferencedAssembly(context, "Katuusagi.MemoizationForUnity"))
            {
                return;
            }

            ContextUtils.InitLog<MemoizationGenerator>(context);
            try
            {
                var rootInfo = Analyze(context);
                foreach (var typeInfo in rootInfo.TypeInfos)
                {
                    var root = new RootGenerator();
                    root.Generate(rg =>
                    {
                        foreach (var @using in typeInfo.Usings)
                        {
                            rg.Using.Generate(@using);
                        }

                        if (string.IsNullOrEmpty(typeInfo.NameSpace))
                        {
                            GenerateAncestor(rg.Type, typeInfo, typeInfo.Ancestors);
                            return;
                        }

                        rg.Namespace.Generate(typeInfo.NameSpace, ng =>
                        {
                            GenerateAncestor(ng.Type, typeInfo, typeInfo.Ancestors);
                        });
                    });
                    var builder = new CSharpScriptBuilder();
                    builder.BuildAndNewLine(root.Result);
                    context.AddSource($"{typeInfo.FileName}.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
                }

                if (!rootInfo.AdditionalDefaultComparers.Any() &&
                    !rootInfo.AdditionalParamsComparers.Any() &&
                    !rootInfo.ArrayElementComparers.Any())
                {
                    return;
                }

                var comparerRoot = new RootGenerator();
                comparerRoot.Generate(rg =>
                {
                    rg.Using.Generate("System");
                    rg.Using.Generate("System.Collections.Generic");
                    rg.Using.Generate("System.Linq");

                    rg.Namespace.Generate("Katuusagi.MemoizationForUnity", ng =>
                    {
                        foreach (var count in rootInfo.AdditionalDefaultComparers)
                        {
                            string gen = string.Empty;
                            for (int i = 1; i <= count; ++i)
                            {
                                gen += $"T{i}, ";
                            }

                            if (!string.IsNullOrEmpty(gen))
                            {
                                gen = gen.Remove(gen.Length - 2, 2);
                            }

                            ng.Type.Generate(ModifierType.Internal | ModifierType.Sealed | ModifierType.Class, "MemoizationEqualityComparer", tg =>
                            {
                                for (int i = 1; i <= count; ++i)
                                {
                                    tg.GenericParam.Generate($"T{i}");
                                }

                                tg.BaseType.Generate($"IEqualityComparer<({gen})>");
                                tg.Field.Generate(ModifierType.Static | ModifierType.ReadOnly | ModifierType.Public, $"MemoizationEqualityComparer<{gen}>", "Default", "new ()");
                                tg.Method.Generate(ModifierType.None, "bool", $"IEqualityComparer<({gen})>.Equals", mg =>
                                {
                                    mg.Param.Generate($"({gen})", "x");
                                    mg.Param.Generate($"({gen})", "y");

                                    mg.Statement.Generate($"return x.Equals(y);");
                                });

                                tg.Method.Generate(ModifierType.None, "int", $"IEqualityComparer<({gen})>.GetHashCode", mg =>
                                {
                                    mg.Param.Generate($"({gen})", "obj");

                                    mg.Statement.Generate($"var hash = new HashCode();");
                                    for (int i = 1; i <= count; ++i)
                                    {
                                        mg.Statement.Generate($"hash.Add(obj.Item{i});");
                                    }

                                    mg.Statement.Generate("return hash.ToHashCode();");
                                });
                            });
                        }

                        foreach (var count in rootInfo.AdditionalParamsComparers)
                        {
                            string gen = string.Empty;
                            for (int i = 1; i <= count; ++i)
                            {
                                gen += $"T{i}, ";
                            }

                            if (!string.IsNullOrEmpty(gen))
                            {
                                gen = gen.Remove(gen.Length - 2, 2);
                            }

                            ng.Type.Generate(ModifierType.Internal | ModifierType.Sealed | ModifierType.Class, "ParamsEqualityComparer", tg =>
                            {
                                for (int i = 1; i <= count; ++i)
                                {
                                    tg.GenericParam.Generate($"T{i}");
                                }

                                tg.BaseType.Generate($"IEqualityComparer<({gen}[])>");
                                tg.Field.Generate(ModifierType.Static | ModifierType.ReadOnly | ModifierType.Public, $"ParamsEqualityComparer<{gen}>", "Default", "new ()");
                                tg.Method.Generate(ModifierType.None, "bool", $"IEqualityComparer<({gen}[])>.Equals", mg =>
                                {
                                    mg.Param.Generate($"({gen}[])", "x");
                                    mg.Param.Generate($"({gen}[])", "y");

                                    mg.Statement.Generate($"return EqualityComparer<T1>.Default.Equals(x.Item1, y.Item1) &&");
                                    for (int i = 2; i < count; ++i)
                                    {
                                        mg.Statement.Generate($"       EqualityComparer<T{i}>.Default.Equals(x.Item{i}, y.Item{i}) &&");
                                    }
                                    mg.Statement.Generate($"       (x.Item{count} ?? Array.Empty<T{count}>()).SequenceEqual(y.Item{count} ?? Array.Empty<T{count}>());");
                                });

                                tg.Method.Generate(ModifierType.None, "int", $"IEqualityComparer<({gen}[])>.GetHashCode", mg =>
                                {
                                    mg.Param.Generate($"({gen}[])", "obj");

                                    mg.Statement.Generate($"var hash = new HashCode();");
                                    for (int i = 1; i < count; ++i)
                                    {
                                        mg.Statement.Generate($"hash.Add(obj.Item{i});");
                                    }

                                    mg.Statement.Generate($"foreach (var elem in obj.Item{count} ?? Array.Empty<T{count}>())", () =>
                                    {
                                        mg.Statement.Generate("hash.Add(elem);");
                                    });

                                    mg.Statement.Generate("return hash.ToHashCode();");
                                });
                            });
                        }


                        foreach (var flags in rootInfo.ArrayElementComparers)
                        {
                            var count = flags.Length;

                            string parameterArrayFlag = string.Empty;
                            string gen = string.Empty;
                            string arrayGen = string.Empty;
                            for (int i = 1; i <= count; ++i)
                            {
                                gen += $"T{i}, ";
                                var isArray = flags[i - 1];
                                if (isArray)
                                {
                                    arrayGen += $"T{i}[], ";
                                    parameterArrayFlag += "1";
                                }
                                else
                                {
                                    arrayGen += $"T{i}, ";
                                    parameterArrayFlag += "0";
                                }
                            }

                            if (!string.IsNullOrEmpty(gen))
                            {
                                gen = gen.Remove(gen.Length - 2, 2);
                                arrayGen = arrayGen.Remove(arrayGen.Length - 2, 2);
                            }

                            ng.Type.Generate(ModifierType.Internal | ModifierType.Sealed | ModifierType.Class, $"ArrayElementEqualityComparer{parameterArrayFlag}", tg =>
                            {
                                for (int i = 1; i <= count; ++i)
                                {
                                    tg.GenericParam.Generate($"T{i}");
                                }

                                tg.BaseType.Generate($"IEqualityComparer<({arrayGen})>");
                                tg.Field.Generate(ModifierType.Static | ModifierType.ReadOnly | ModifierType.Public, $"ArrayElementEqualityComparer{parameterArrayFlag}<{gen}>", "Default", "new ()");
                                tg.Method.Generate(ModifierType.None, "bool", $"IEqualityComparer<({arrayGen})>.Equals", mg =>
                                {
                                    mg.Param.Generate($"({arrayGen})", "x");
                                    mg.Param.Generate($"({arrayGen})", "y");

                                    var isArray = flags[0];
                                    if (count == 1)
                                    {
                                        if (isArray)
                                        {
                                            mg.Statement.Generate($"return (x.Item1 ?? Array.Empty<T1>()).SequenceEqual(y.Item1 ?? Array.Empty<T1>());");
                                        }
                                        else
                                        {
                                            mg.Statement.Generate($"return EqualityComparer<T1>.Default.Equals(x.Item1, y.Item1);");
                                        }
                                        return;
                                    }

                                    if (isArray)
                                    {
                                        mg.Statement.Generate($"return (x.Item1 ?? Array.Empty<T1>()).SequenceEqual(y.Item1 ?? Array.Empty<T1>()) &&");
                                    }
                                    else
                                    {
                                        mg.Statement.Generate($"return EqualityComparer<T1>.Default.Equals(x.Item1, y.Item1) &&");
                                    }

                                    for (int i = 2; i < count; ++i)
                                    {
                                        isArray = flags[i - 1];
                                        if (isArray)
                                        {
                                            mg.Statement.Generate($"       (x.Item{i} ?? Array.Empty<T{i}>()).SequenceEqual(y.Item{i} ?? Array.Empty<T{i}>()) &&");
                                        }
                                        else
                                        {
                                            mg.Statement.Generate($"       EqualityComparer<T{i}>.Default.Equals(x.Item{i}, y.Item{i}) &&");
                                        }
                                    }

                                    isArray = flags[count - 1];
                                    if (isArray)
                                    {
                                        mg.Statement.Generate($"       (x.Item{count} ?? Array.Empty<T{count}>()).SequenceEqual(y.Item{count} ?? Array.Empty<T{count}>());");
                                    }
                                    else
                                    {
                                        mg.Statement.Generate($"       EqualityComparer<T{count}>.Default.Equals(x.Item{count}, y.Item{count});");
                                    }
                                });

                                tg.Method.Generate(ModifierType.None, "int", $"IEqualityComparer<({arrayGen})>.GetHashCode", mg =>
                                {
                                    mg.Param.Generate($"({arrayGen})", "obj");

                                    mg.Statement.Generate($"var hash = new HashCode();");
                                    for (int i = 1; i <= count; ++i)
                                    {
                                        var isArray = flags[i - 1];
                                        if (isArray)
                                        {
                                            mg.Statement.Generate($"foreach (var elem in obj.Item{i} ?? Array.Empty<T{i}>())", () =>
                                            {
                                                mg.Statement.Generate("hash.Add(elem);");
                                            });
                                        }
                                        else
                                        {
                                            mg.Statement.Generate($"hash.Add(obj.Item{i});");
                                        }
                                    }

                                    mg.Statement.Generate("return hash.ToHashCode();");
                                });
                            });
                        }
                    });
                });

                var comparerBuilder = new CSharpScriptBuilder();
                comparerBuilder.BuildAndNewLine(comparerRoot.Result);
                context.AddSource($"Katuusagi.MemoizationForUnity.MemoizationEqualityComparer.Generated.cs", SourceText.From(comparerBuilder.ToString(), Encoding.UTF8));
            }
            catch (Exception e)
            {
                ContextUtils.LogException(e);
            }
        }

        private void GenerateAncestor(TypeGenerator typeGenerator, TypeInfo typeInfo, IEnumerable<AncestorInfo> ancestors)
        {
            if (!ancestors.Any())
            {
                GenerateType(typeGenerator, typeInfo);
                return;
            }

            var ancestor = ancestors.First();
            typeGenerator.Generate(ancestor.Modifier, ancestor.Name, tg =>
            {
                foreach (var generic in ancestor.Generics)
                {
                    tg.GenericParam.Generate(generic);
                }
                GenerateAncestor(tg.Type, typeInfo, ancestors.Skip(1));
            });
        }

        private void GenerateType(TypeGenerator typeGenerator, TypeInfo typeInfo)
        {
            var guid = Guid.NewGuid().ToString().Replace("-", string.Empty);
            var cacheValues = $"__MemoizationCacheValues_{guid}__";
            var concurrentCacheValues = $"__MemoizationThreadSafeCacheValues_{guid}__";
            typeGenerator.Generate(typeInfo.Modifier, typeInfo.Name, tg =>
            {
                foreach (var generic in typeInfo.Generics)
                {
                    tg.GenericParam.Generate(generic);
                }

                if (typeInfo.HasGenericClearableStaticTypeCache)
                {
                    tg.Field.Generate(ModifierType.Private | ModifierType.Static, "global::System.Collections.Generic.Stack<System.Collections.IDictionary>", cacheValues, "new global::System.Collections.Generic.Stack<System.Collections.IDictionary>()");
                }

                if (typeInfo.HasThreadSafeGenericClearableStaticTypeCache)
                {
                    tg.Field.Generate(ModifierType.Private | ModifierType.Static, "global::System.Collections.Concurrent.ConcurrentStack<System.Collections.IDictionary>", concurrentCacheValues, "new global::System.Collections.Concurrent.ConcurrentStack<System.Collections.IDictionary>()");
                }

                foreach (var method in typeInfo.Methods)
                {
                    var cacheValueTypeDeclarationName = method.CacheValueTypeDeclarationName;
                    var cacheValueType = method.CacheValueTypeName;
                    var cachingStyle = method.CachingStyle;
                    var comparer = method.Comparer;
                    var onCachedMethod = method.OnCachedMethod;

                    if (method.IsUseStaticTypeCache)
                    {
                        tg.Type.Generate(ModifierType.Private | ModifierType.Static | ModifierType.Class, method.StaticFieldWrappedType, ttg =>
                        {
                            foreach (var parameter in method.GenericParameters)
                            {
                                ttg.GenericParam.Generate(parameter.Type, gg =>
                                {
                                    foreach (var where in parameter.Wheres)
                                    {
                                        gg.Where.Generate(where);
                                    }
                                });
                            }

                            ttg.Field.Generate(ModifierType.Public | ModifierType.Static, cacheValueTypeDeclarationName, "Cache", fg =>
                            {
                                if (method.IsThreadStatic)
                                {
                                    fg.Attribute.Generate("global::System.ThreadStatic");
                                }
                            });
                            ttg.Method.Generate(ModifierType.Static, string.Empty, method.StaticFieldWrappedType, cg =>
                            {
                                if (cachingStyle == CachingStyle.Direct && method.OutputParameters.Any())
                                {
                                    if (method.HasReturnType)
                                    {
                                        cg.Statement.Generate($"var __return__ = {method.CallAndDeclairRawMethod};");
                                    }
                                    else
                                    {
                                        cg.Statement.Generate($"{method.CallAndDeclairRawMethod};");
                                    }

                                    cg.Statement.Generate($"Cache = {method.Result};");
                                }
                                else
                                {
                                    cg.Statement.Generate($"Cache = {method.FieldInitializer};");
                                }

                                if (cachingStyle != CachingStyle.Direct && method.IsClearable && method.HasGenericParameter)
                                {
                                    if (method.IsConcurrent || method.IsThreadStatic)
                                    {
                                        cg.Statement.Generate($"{concurrentCacheValues}.Push(Cache);");
                                    }
                                    else
                                    {
                                        cg.Statement.Generate($"{cacheValues}.Push(Cache);");
                                    }
                                }
                                else if (cachingStyle == CachingStyle.Direct && !string.IsNullOrEmpty(onCachedMethod))
                                {
                                    cg.Statement.Generate($"{method.OnCachedMethodWithGeneric}(Cache);");
                                }
                            });
                        });
                    }
                    else
                    {
                        tg.Field.Generate(method.CacheFieldModifier, cacheValueTypeDeclarationName, method.CacheValueName, fg =>
                        {
                            if (method.IsThreadStatic)
                            {
                                fg.Attribute.Generate("global::System.ThreadStatic");
                            }
                            fg.Default.Generate(method.FieldInitializer);
                        });
                    }

                    GenerateCacheMethod(tg.Method, method, cacheValues, concurrentCacheValues);
                    var interruptCacheMethod = method.InterruptCacheMethod;
                    if (!string.IsNullOrEmpty(interruptCacheMethod))
                    {
                        var type = method.IsStatic ? ModifierType.Static : ModifierType.None;
                        tg.Method.Generate(ModifierType.Private | type, "void", interruptCacheMethod, mg =>
                        {
                            foreach (var parameter in method.GenericParameters)
                            {
                                mg.GenericParam.Generate(parameter.Type, gg =>
                                {
                                    foreach (var attribute in parameter.Attributes)
                                    {
                                        gg.Attribute.Generate(attribute);
                                    }

                                    foreach (var where in parameter.Wheres)
                                    {
                                        gg.Where.Generate(where);
                                    }
                                });
                            }

                            switch (cachingStyle)
                            {
                                case CachingStyle.Direct:
                                    mg.Param.Generate(method.ResultType, "result");
                                    mg.Statement.Generate($"{method.CacheValueName} = result;");
                                    break;
                                case CachingStyle.NoneKey:
                                    mg.Param.Generate(method.ResultType, "result");
                                    mg.Statement.Generate($"{method.CacheValueName}.Set(result)");
                                    break;
                                case CachingStyle.SingleKey:
                                case CachingStyle.MultipleKey:
                                    mg.Param.Generate(method.KeyType, "key");
                                    mg.Param.Generate(method.ResultType, "result");

                                    if (method.IsConcurrent)
                                    {
                                        mg.Statement.Generate($"if (!{method.CacheValueName}.TryAdd(key, result))", () =>
                                        {
                                            mg.Statement.Generate($"{method.CacheValueName}[key] = result;");
                                        });
                                    }
                                    else
                                    {
                                        mg.Statement.Generate($"if ({method.CacheValueName}.ContainsKey(key))", () =>
                                        {
                                            mg.Statement.Generate($"{method.CacheValueName}[key] = result;");
                                        });
                                        mg.Statement.Generate($"else", () =>
                                        {
                                            mg.Statement.Generate($"{method.CacheValueName}.Add(key, result);");
                                        });
                                    }
                                    break;
                            }
                        });
                    }

                    if (!string.IsNullOrEmpty(onCachedMethod))
                    {
                        var type = method.IsStatic ? ModifierType.Static : ModifierType.None;
                        tg.Method.Generate(ModifierType.Private | ModifierType.Partial | type, "void", onCachedMethod, mg =>
                        {
                            foreach (var parameter in method.GenericParameters)
                            {
                                mg.GenericParam.Generate(parameter.Type, gg =>
                                {
                                    foreach (var attribute in parameter.Attributes)
                                    {
                                        gg.Attribute.Generate(attribute);
                                    }

                                    foreach (var where in parameter.Wheres)
                                    {
                                        gg.Where.Generate(where);
                                    }
                                });
                            }

                            switch (cachingStyle)
                            {
                                case CachingStyle.Direct:
                                case CachingStyle.NoneKey:
                                    mg.Param.Generate(method.ResultType, "result");
                                    break;
                                case CachingStyle.SingleKey:
                                case CachingStyle.MultipleKey:
                                    mg.Param.Generate(method.KeyType, "key");
                                    mg.Param.Generate(method.ResultType, "result");
                                    break;
                            }
                        });
                    }
                }

                var clearableInstanceMethods = typeInfo.Methods.Where(v => v.IsClearable && !v.IsUseStaticCache).ToArray();
                if (clearableInstanceMethods.Any())
                {
                    tg.Method.Generate(ModifierType.Public, "void", "ClearInstanceMemoizationCache", mg =>
                    {
                        foreach (var method in clearableInstanceMethods)
                        {
                            if (method.IsUseBaseTypeCache)
                            {
                                mg.Statement.Generate($"foreach (var value in {method.CacheValueName}.Values)", () =>
                                {
                                    mg.Statement.Generate($"value.Clear();");
                                });
                            }
                            else
                            {
                                mg.Statement.Generate($"{method.CacheValueName}.Clear();");
                            }
                        }
                    });
                }

                var clearableStaticMethods = typeInfo.Methods.Where(v => v.IsClearable && v.IsUseStaticCache && !v.HasGenericParameter).ToArray();
                if (clearableStaticMethods.Any() || typeInfo.HasGenericClearableStaticTypeCache)
                {
                    tg.Method.Generate(ModifierType.Public | ModifierType.Static, "void", "ClearStaticMemoizationCache", mg =>
                    {
                        if (typeInfo.HasGenericClearableStaticTypeCache)
                        {
                            mg.Statement.Generate($"foreach (var value in {cacheValues})", () =>
                            {
                                mg.Statement.Generate($"value.Clear();");
                            });
                        }

                        if (typeInfo.HasThreadSafeGenericClearableStaticTypeCache)
                        {
                            mg.Statement.Generate($"foreach (var value in {concurrentCacheValues})", () =>
                            {
                                mg.Statement.Generate($"value.Clear();");
                            });
                        }

                        foreach (var method in clearableStaticMethods)
                        {
                            mg.Statement.Generate($"{method.CacheValueName}.Clear();");
                        }
                    });
                }
            });
        }

        private void GenerateCacheMethod(MethodGenerator methodGenerator, MethodInfo method, string cacheValues, string concurrentCacheValues)
        {
            var key = method.Key;
            var result = method.Result;
            var cacheValue = method.CacheValueName;
            var onCachedMethod = method.OnCachedMethod;
            var cachingStyle = method.CachingStyle;

            methodGenerator.Generate(method.Modifier, method.ReturnType, method.Name, mg =>
            {
                foreach (var attribute in method.Attributes)
                {
                    mg.Attribute.Generate(attribute);
                }

                foreach (var parameter in method.GenericParameters)
                {
                    mg.GenericParam.Generate(parameter.Type, gg =>
                    {
                        foreach (var attribute in parameter.Attributes)
                        {
                            gg.Attribute.Generate(attribute);
                        }

                        foreach (var where in parameter.Wheres)
                        {
                            gg.Where.Generate(where);
                        }
                    });
                }

                foreach (var parameter in method.Parameters)
                {
                    mg.Param.Generate(parameter.ModifieredTypeName, parameter.Name, pg =>
                    {
                        if (!string.IsNullOrEmpty(parameter.Default))
                        {
                            pg.Default.Generate(parameter.Default);
                        }

                        foreach (var attribute in parameter.Attributes)
                        {
                            pg.Attribute.Generate(attribute);
                        }
                    });
                }

                if (method.IsUseBaseTypeCache)
                {
                    var cacheValueTypeName = method.CacheValueTypeName;
                    var comparer = method.Comparer;
                    var typeKey = method.DeclarationKey;
                    mg.Statement.Generate($"var __typeKey__ = {typeKey};");
                    mg.Statement.Generate($"{cacheValueTypeName} __table__;");
                    mg.Statement.Generate($"if ({cacheValue}.TryGetValue(__typeKey__, out var __tmp__))", () =>
                    {
                        mg.Statement.Generate($"__table__ = __tmp__ as {cacheValueTypeName};");
                    });
                    mg.Statement.Generate($"else", () =>
                    {
                        mg.Statement.Generate($"__table__ = new {cacheValueTypeName}({comparer});");
                        if (method.IsConcurrent)
                        {
                            mg.Statement.Generate($"{cacheValue}.TryAdd(__typeKey__, __table__);");
                        }
                        else
                        {
                            mg.Statement.Generate($"{cacheValue}.Add(__typeKey__, __table__);");
                        }
                    });

                    cacheValue = "__table__";
                }

                if (method.IsThreadStatic)
                {
                    mg.Statement.Generate($"if ({cacheValue} is null)", () =>
                    {
                        mg.Statement.Generate($"{cacheValue} = {method.FieldInitializer};");
                        if (cachingStyle == CachingStyle.Direct && !string.IsNullOrEmpty(onCachedMethod))
                        {
                            mg.Statement.Generate($"{method.OnCachedMethodWithGeneric}({cacheValue});");
                        }
                    });
                }

                switch (method.CachingStyle)
                {
                    case CachingStyle.Direct:
                        mg.Statement.Generate($"var __cache__ = {cacheValue};");
                        foreach (var statement in method.ResultReturnCodes)
                        {
                            mg.Statement.Generate(statement);
                        }
                        return;
                    case CachingStyle.NoneKey:
                        mg.Statement.Generate($"if ({cacheValue}.IsCached)", () =>
                        {
                            mg.Statement.Generate($"var __cache__ = {cacheValue}.Result;");
                            foreach (var statement in method.ResultReturnCodes)
                            {
                                mg.Statement.Generate(statement);
                            }
                        });

                        if (method.HasReturnType)
                        {
                            mg.Statement.Generate($"var __return__ = {method.CallRawMethod};");
                        }
                        else
                        {
                            mg.Statement.Generate($"{method.CallRawMethod};");
                        }
                        mg.Statement.Generate($"{cacheValue}.Set({result});");

                        if (!string.IsNullOrEmpty(onCachedMethod))
                        {
                            mg.Statement.Generate($"{method.OnCachedMethodWithGeneric}(__return__);");
                        }

                        if (method.HasReturnType)
                        {
                            mg.Statement.Generate($"return __return__;");
                        }
                        return;
                    case CachingStyle.SingleKey:
                    case CachingStyle.MultipleKey:
                        mg.Statement.Generate($"var __key__ = {key};");
                        mg.Statement.Generate($"if ({cacheValue}.TryGetValue(__key__, out var __cache__))", () =>
                        {
                            foreach (var statement in method.ResultReturnCodes)
                            {
                                mg.Statement.Generate(statement);
                            }
                        });

                        if (method.HasReturnType)
                        {
                            mg.Statement.Generate($"var __return__ = {method.CallRawMethod};");
                        }
                        else
                        {
                            mg.Statement.Generate($"{method.CallRawMethod};");
                        }

                        if (method.IsConcurrent)
                        {
                            mg.Statement.Generate($"{cacheValue}.TryAdd(__key__, {result});");
                        }
                        else
                        {
                            mg.Statement.Generate($"{cacheValue}.Add(__key__, {result});");
                        }

                        if (!string.IsNullOrEmpty(onCachedMethod))
                        {
                            mg.Statement.Generate($"{method.OnCachedMethodWithGeneric}(__key__, __return__);");
                        }

                        if (method.HasReturnType)
                        {
                            mg.Statement.Generate($"return __return__;");
                        }
                        return;
                }
            });
        }

        private RootInfo Analyze(GeneratorExecutionContext context)
        {
            var result = new RootInfo()
            {
                TypeInfos = new List<TypeInfo>(),
                AdditionalDefaultComparers = new HashSet<int>(),
                AdditionalParamsComparers = new HashSet<int>(),
                ArrayElementComparers = new HashSet<bool[]>(FlagsComparer.Default),
            };

            var typeGroups = context.Compilation.SyntaxTrees
                                    .OrderBy(v => v.FilePath)
                                    .SelectMany(v => v.GetCompilationUnitRoot().GetStructuredTypes())
                                    .GroupBy(v => v.GetFullName());
            foreach (var typeGroup in typeGroups)
            {
                ModifierType typeModifier = ModifierType.None;
                foreach (var type in typeGroup)
                {
                    typeModifier |= ScriptGeneratorUtils.GetModifierType(type.Keyword.Text);
                    typeModifier |= ScriptGeneratorUtils.GetModifierType(type.Modifiers.ToString());
                }

                var firstType = typeGroup.First();

                var ancestors = firstType.GetAncestors<TypeDeclarationSyntax>().Reverse().Select(v =>
                {
                    var ancestorGenerics = Array.Empty<string>();
                    if (v.TypeParameterList != null)
                    {
                        ancestorGenerics = v.TypeParameterList.Parameters.Select(v2 => v2.ToString()).ToArray();
                    }
                    return new AncestorInfo()
                    {
                        Modifier = ScriptGeneratorUtils.GetModifierType(v.Keyword.Text) | ScriptGeneratorUtils.GetModifierType(v.Modifiers.ToString()),
                        Name = v.Identifier.ToString(),
                        Generics = ancestorGenerics,
                    };
                }).ToArray();

                var generics = Array.Empty<string>();
                if (firstType.TypeParameterList != null)
                {
                    generics = firstType.TypeParameterList.Parameters.Select(v => v.ToString()).ToArray();
                }

                var usings = typeGroup.SelectMany(v => v.GetAncestorUsings()).Select(v =>
                {
                    var usingName = v.Name.ToString();
                    if (!string.IsNullOrEmpty(v.StaticKeyword.Text))
                    {
                        usingName = $"{v.StaticKeyword.Text} {usingName}";
                    }

                    if (!string.IsNullOrEmpty(v.Alias?.Name?.ToString()))
                    {
                        usingName = $"{v.Alias.Name} = {usingName}";
                    }

                    return usingName;
                }).Distinct().OrderBy(v => v).ToArray();

                var typeInfo = new TypeInfo()
                {
                    Modifier = typeModifier,
                    Ancestors = ancestors,
                    NameSpace = firstType.GetNameSpace(),
                    Name = firstType.Identifier.ToString(),
                    Generics = generics,
                    Usings = usings,
                    Methods = new List<MethodInfo>(),
                };

                foreach (var type in typeGroup)
                {
                    var memoizeNames = type.GetTypeNames("Katuusagi.MemoizationForUnity", "Memoization").ToArray();
                    var methods = type.GetMethods();
                    foreach (var method in methods)
                    {
                        var memoize = memoizeNames.Select(v => method.GetAttribute(v)).FirstOrDefault(v => v != null);
                        if (memoize == null)
                        {
                            continue;
                        }

                        bool isPartial = type.IsPerfectPartial();
                        if (!isPartial)
                        {
                            ContextUtils.LogError("MEMOIZATION001", "Memoization failed.", "Memoization is partial type member only.", type);
                            continue;
                        }

                        memoize.TryGetArgumentValue("Modifier", -1, null, out string modifierLabel);
                        memoize.TryGetArgumentValue("MethodName", -1, null, out string memoizationMethodName);
                        memoize.TryGetArgumentValue("IsClearable", -1, false, out bool isClearable);
                        memoize.TryGetArgumentValue("IsThreadSafe", -1, false, out bool isThreadSafe);
                        memoize.TryGetArgumentValue("ThreadSafeType", -1, ThreadSafeType.None, out ThreadSafeType threadSafeType);
                        memoize.TryGetArgumentValue("CompareArrayElement", -1, false, out bool compareArrayElement);
                        memoize.TryGetArgumentValue("CacheComparer", -1, null, out string cacheComparer);
                        memoize.TryGetArgumentValue("InterruptCacheMethod", -1, null, out string interruptCacheMethod);
                        memoize.TryGetArgumentValue("OnCachedMethod", -1, null, out string onCachedMethod);
                        if (threadSafeType == ThreadSafeType.None && isThreadSafe)
                        {
                            threadSafeType = ThreadSafeType.Concurrent;
                        }

                        if (compareArrayElement)
                        {
                            if (cacheComparer != null)
                            {
                                ContextUtils.LogError("MEMOIZATION004", "Memoization failed.", "`CompareArrayElement` parameter and the `CacheComparer` parameter cannot be used simultaneously.", method);
                                continue;
                            }

                            if (method.ParameterList == null || !method.ParameterList.Parameters.Any(v => v.Type.ToString().EndsWith("[]")))
                            {
                                ContextUtils.LogError("MEMOIZATION005", "Memoization failed.", "`CompareArrayElement` parameter cannot be used if method parameters does not have an array type.", method);
                                continue;
                            }
                        }

                        var methodName = method.Identifier.ToString();

                        ModifierType methodModifier;
                        if (string.IsNullOrEmpty(modifierLabel))
                        {
                            methodModifier = ScriptGeneratorUtils.GetModifierType(method.Modifiers.ToString());
                        }
                        else
                        {
                            methodModifier = ScriptGeneratorUtils.GetModifierType(modifierLabel);
                        }

                        // asyncはTaskだけ返す
                        methodModifier &= ~ModifierType.Async;

                        bool isStruct = typeModifier.HasFlag(ModifierType.Struct);
                        bool isStatic = methodModifier.HasFlag(ModifierType.Static);
                        if (isStruct && !isStatic)
                        {
                            ContextUtils.LogError("MEMOIZATION002", "Memoization failed.", "Memoization is static member or class instance member only.", method);
                            continue;
                        }

                        if (threadSafeType == ThreadSafeType.ThreadStatic)
                        {
                            if (!isStatic)
                            {
                                ContextUtils.LogError("MEMOIZATION006", "Memoization failed.", "\"ThreadStatic\" is static member only.", method);
                            }
                            
                            if (isClearable)
                            {
                                ContextUtils.LogError("MEMOIZATION007", "Memoization failed.", "\"ThreadStatic\" is not clearable.", method);
                            }
                        }

                        if (string.IsNullOrEmpty(memoizationMethodName))
                        {
                            var rawMethodSuffix = "Raw";
                            if (methodName.Length > rawMethodSuffix.Length &&
                                methodName.EndsWith(rawMethodSuffix))
                            {
                                memoizationMethodName = methodName.Substring(0, methodName.Length - rawMethodSuffix.Length);
                            }
                            else
                            {
                                memoizationMethodName = $"{methodName}WithMemoization";
                            }
                        }

                        var methodInfo = new MethodInfo()
                        {
                            Id = Guid.NewGuid().ToString().Replace("-", string.Empty),
                            Attributes = method.AttributeLists.SelectMany(v => v?.Attributes.Where(v2 => v2 != memoize).Select(v2 => v2.ToFullString()) ?? Array.Empty<string>()).ToArray(),
                            Name = memoizationMethodName,
                            RawName = methodName,
                            ReturnType = method.ReturnType.ToString(),
                            Modifier = methodModifier,
                            CompareArrayElement = compareArrayElement,
                            CacheComparer = cacheComparer,
                            IsClearable = isClearable,
                            ThreadSafeType = threadSafeType,
                            InterruptCacheMethod = interruptCacheMethod,
                            OnCachedMethod = onCachedMethod,
                            GenericParameters = new List<GenericParameterInfo>(),
                            Parameters = new List<ParameterInfo>(),
                        };

                        if (method.TypeParameterList != null)
                        {
                            foreach (var parameter in method.TypeParameterList.Parameters)
                            {
                                var typeParameterName = parameter.Identifier.ToString();
                                var parameterInfo = new GenericParameterInfo()
                                {
                                    Attributes = parameter.AttributeLists.SelectMany(v => v?.Attributes.Select(v2 => v2.ToFullString()) ?? Array.Empty<string>()).ToArray(),
                                    Wheres = method.ConstraintClauses.Where(v => v.Name.ToString() == typeParameterName).SelectMany(v => v.Constraints).Select(v => v.ToString()).ToArray(),
                                    Type = typeParameterName,
                                };

                                methodInfo.GenericParameters.Add(parameterInfo);
                            }
                        }

                        methodInfo.HasGenericTypeInReturn = HasGenericTypes(method.ReturnType, methodInfo.GenericParameters.Select(v => v.Type));

                        if (method.ParameterList != null)
                        {
                            foreach (var parameter in method.ParameterList.Parameters)
                            {
                                var parameterInfo = new ParameterInfo()
                                {
                                    Attributes = parameter.AttributeLists.SelectMany(v => v?.Attributes.Select(v2 => v2.ToFullString()) ?? Array.Empty<string>()).ToArray(),
                                    Modifier = parameter.Modifiers.ToString(),
                                    Type = parameter.Type.ToString(),
                                    Name = parameter.Identifier.ToString(),
                                    Default = parameter.Default?.Value?.ToString() ?? string.Empty,
                                    HasGenericType = HasGenericTypes(parameter.Type, methodInfo.GenericParameters.Select(v => v.Type)),
                                };

                                parameterInfo.Init();
                                methodInfo.Parameters.Add(parameterInfo);
                            }
                        }

                        methodInfo.PreInit();
                        if (!methodInfo.OutputParameters.Any() && !methodInfo.HasReturnType)
                        {
                            ContextUtils.LogError("MEMOIZATION003", "Memoization failed.", "Memoization must always return. Or it must have an out or ref parameter.", method);
                            continue;
                        }

                        if (methodInfo.CompareArrayElement)
                        {
                            string parameterArrayFlag = string.Empty;
                            var args = methodInfo.KeyTypeArgs.ToArray();
                            bool[] flags = new bool[args.Length];
                            for (int i = 0; i < args.Length; ++i)
                            {
                                var arg = args[i];
                                if (arg.EndsWith("[]"))
                                {
                                    flags[i] = true;
                                    parameterArrayFlag += "1";
                                }
                                else
                                {
                                    flags[i] = false;
                                    parameterArrayFlag += "0";
                                }
                            }
                            if (methodInfo.InputParameters.Count() > 1)
                            {
                                result.ArrayElementComparers.Add(flags);
                            }
                            methodInfo.ParameterArrayFlag = parameterArrayFlag;
                        }
                        else
                        {
                            var keyCount = methodInfo.CacheKeyCount;
                            if (keyCount > 8 && methodInfo.CacheComparer == null)
                            {
                                if (methodInfo.HasParams)
                                {
                                    result.AdditionalParamsComparers.Add(keyCount);
                                }
                                else
                                {
                                    result.AdditionalDefaultComparers.Add(keyCount);
                                }
                            }
                        }

                        methodInfo.Init();
                        typeInfo.Methods.Add(methodInfo);
                    }
                }

                if (typeInfo.Methods.Any())
                {
                    typeInfo.Methods = typeInfo.Methods.OrderBy(v => v.Name).ToList();
                    typeInfo.Init();
                    result.TypeInfos.Add(typeInfo);
                }
            }

            return result;
        }

        private static bool HasGenericTypes(TypeSyntax type, IEnumerable<string> genericParameters)
        {
            if (type is GenericNameSyntax genericType)
            {
                return genericType.TypeArgumentList.Arguments
                        .Any(v => HasGenericTypes(v, genericParameters));
            }

            if (type is ArrayTypeSyntax arrayType)
            {
                return HasGenericTypes(arrayType.ElementType, genericParameters);
            }

            return genericParameters.Contains(type.ToString());
        }
    }
}
