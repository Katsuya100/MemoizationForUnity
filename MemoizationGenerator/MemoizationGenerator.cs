using Katuusagi.CSharpScriptGenerator;
using Katuusagi.MemoizationForUnity.SourceGenerator.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Katuusagi.MemoizationForUnity.SourceGenerator
{
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
        private struct RootInfo
        {
            public AncestorInfo[] Ancestors;
            public string[] Usings;
            public ModifierType Modifier;
            public string NameSpace;
            public string Name;
            public List<MethodInfo> Methods;

            public string FileName
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

                    return $"{result}.Generated";
                }
            }

            public string AncestorPath
            {
                get
                {
                    var ancestorPath = string.Concat(Ancestors.Select(v => $"{v.Name}-"));
                    if (!string.IsNullOrEmpty(ancestorPath))
                    {
                        ancestorPath = ancestorPath.Remove(ancestorPath.Length - 1);
                    }

                    return ancestorPath;
                }
            }

            public bool HasGenericClearableStaticTypeCache
            {
                get
                {
                    return Methods.Any(v => v.IsClearableGenericStaticTypeCache && !v.IsThreadSafe);
                }
            }

            public bool HasThreadSafeGenericClearableStaticTypeCache
            {
                get
                {
                    return Methods.Any(v => v.IsClearableGenericStaticTypeCache && v.IsThreadSafe);
                }

            }
        }

        private struct AncestorInfo
        {
            public ModifierType Modifier;
            public string Name;
        }

        private struct MethodInfo
        {
            public string Id;
            public string[] Attributes;
            public ModifierType Modifier;
            public string Name;
            public string RawName;
            public string CacheComparer;
            public bool IsClearable;
            public bool IsThreadSafe;
            public string ReturnType;
            public bool HasGenericTypeInReturn;
            public List<GenericParameterInfo> GenericParameters;
            public List<ParameterInfo> Parameters;

            public IEnumerable<ParameterInfo> OutputParameters
            {
                get
                {
                    return Parameters.Where(v => v.IsOutput);
                }
            }

            public IEnumerable<ParameterInfo> InputParameters
            {
                get
                {
                    return Parameters.Where(v => v.IsInput);
                }
            }

            public bool IsStatic
            {
                get
                {
                    return Modifier.HasFlag(ModifierType.Static);
                }
            }

            public bool IsUseStaticCache
            {
                get
                {
                    return IsStatic;
                }
            }

            public bool IsUseStaticTypeCache
            {
                get
                {
                    return IsUseStaticCache && (HasGenericParameter || !HasKey);
                }
            }

            public bool IsUseBaseTypeCache
            {
                get
                {
                    return !IsStatic && (HasGenericParameter && (HasGenericTypeInReturn || HasGenericTypeInParameters));
                }
            }

            public bool IsClearableGenericStaticTypeCache
            {
                get
                {
                    return IsClearable && IsUseStaticTypeCache && HasGenericParameter;
                }
            }

            public bool HasGenericParameter
            {
                get
                {
                    return GenericParameters.Any();
                }
            }

            public bool HasParameter
            {
                get
                {
                    return Parameters.Any();
                }
            }

            public bool HasReturnType
            {
                get
                {
                    return !string.IsNullOrEmpty(ReturnType) &&
                            ReturnType != "void" &&
                            ReturnType != "Void" &&
                            ReturnType != "System.Void";
                }
            }

            public bool HasKey
            {
                get
                {
                    if (HasGenericParameter && !IsUseStaticTypeCache)
                    {
                        return true;
                    }

                    if (HasParameter)
                    {
                        return true;
                    }

                    return false;
                }
            }

            public bool HasGenericTypeInParameters
            {
                get
                {
                    return Parameters.Any(v => v.HasGenericType);
                }
            }

            public int CacheKeyCount
            {
                get
                {
                    int parameterCount = InputParameters.Count();
                    if (IsUseStaticTypeCache)
                    {
                        return parameterCount;
                    }

                    return parameterCount + GenericParameters.Count;
                }
            }

            public CachingStyle CachingStyle
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

            public int ResultCount
            {
                get
                {
                    int result = 0;
                    if (HasReturnType)
                    {
                        ++result;
                    }

                    result += OutputParameters.Count();
                    return result;
                }
            }
            

            public ModifierType CacheFieldModifier
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

            public string StaticFieldWrappedType
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

            public string StaticFieldWrappedTypeFullName
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

            public string CacheValueName
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

            public string CacheValueTypeName
            {
                get
                {
                    switch (CachingStyle)
                    {
                        case CachingStyle.Direct:
                            return ReturnType;
                        case CachingStyle.NoneKey:
                            if (IsThreadSafe)
                            {
                                return $"Katuusagi.MemoizationForUnity.LockFreeCacheValue<{ResultType}>";
                            }

                            return $"Katuusagi.MemoizationForUnity.CacheValue<{ResultType}>";
                        case CachingStyle.SingleKey:
                        case CachingStyle.MultipleKey:
                            if (IsThreadSafe)
                            {
                                return $"System.Collections.Concurrent.ConcurrentDictionary<{KeyType}, {ResultType}>";
                            }

                            return $"System.Collections.Generic.Dictionary<{KeyType}, {ResultType}>";
                    }

                    return string.Empty;
                }
            }

            public string CacheValueTypeDeclarationName
            {
                get
                {
                    if (IsUseBaseTypeCache)
                    {
                        return $"System.Collections.IDictionary";
                    }

                    return CacheValueTypeName;
                }
            }

            public string CallRawMethod
            {
                get
                {
                    string genericArgs = string.Empty;
                    if (!string.IsNullOrEmpty(GenericArgs))
                    {
                        genericArgs = $"<{GenericArgs}>";
                    }

                    return $"{RawName}{genericArgs}({Args})";
                }
            }

            public string FieldInitializer
            {
                get
                {
                    if (IsUseBaseTypeCache)
                    {
                        return null;
                    }

                    return FieldValue;
                }
            }

            public string FieldValue
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

            public string GenericArgs
            {
                get
                {
                    string result = string.Empty;
                    foreach (var parameter in GenericParameters)
                    {
                        result = $"{result}{parameter.Type}, ";
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.Remove(result.Length - 2);
                    }

                    return result;
                }
            }

            public string Key
            {
                get
                {
                    string result = string.Empty;
                    if (!IsUseStaticTypeCache)
                    {
                        foreach (var parameter in GenericParameters)
                        {
                            result = $"{result}Katuusagi.MemoizationForUnity.Utils.MemoizationUtils.TypeId<{parameter.Type}>.Id, ";
                        }
                    }

                    foreach (var parameter in InputParameters)
                    {
                        result = $"{result}{parameter.Name}, ";
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.Remove(result.Length - 2);
                    }

                    if (result.Contains(','))
                    {
                        return $"({result})";
                    }
                    return $"System.ValueTuple.Create({result})";
                }
            }

            public string KeyType
            {
                get
                {
                    string result = string.Empty;
                    switch (CachingStyle)
                    {
                        case CachingStyle.SingleKey:
                            if (!IsUseStaticTypeCache)
                            {
                                foreach (var parameter in GenericParameters)
                                {
                                    result = $"{result}int, ";
                                }
                            }

                            foreach (var parameter in InputParameters)
                            {
                                result = $"{result}{parameter.Type}, ";
                            }

                            if (!string.IsNullOrEmpty(result))
                            {
                                result = result.Remove(result.Length - 2);
                            }
                            return $"System.ValueTuple<{result}>";
                        case CachingStyle.MultipleKey:
                            if (!IsUseStaticTypeCache)
                            {
                                foreach (var parameter in GenericParameters)
                                {
                                    result = $"{result}int {parameter.Type}, ";
                                }
                            }

                            foreach (var parameter in InputParameters)
                            {
                                result = $"{result}{parameter.Type} {parameter.Name}, ";
                            }

                            if (!string.IsNullOrEmpty(result))
                            {
                                result = result.Remove(result.Length - 2);
                            }

                            return $"({result})";
                    }

                    return result;
                }
            }

            public string Result
            {
                get
                {
                    string result = string.Empty;
                    if (HasReturnType)
                    {
                        result = $"{result}__result__, ";
                    }

                    foreach (var parameter in OutputParameters)
                    {
                        result = $"{result}{parameter.Name}, ";
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.Remove(result.Length - 2);
                    }

                    if (result.Contains(','))
                    {
                        return $"({result})";
                    }
                    return result;
                }
            }

            public string ResultType
            {
                get
                {
                    string result = string.Empty;
                    if (ResultCount > 1)
                    {
                        if (HasReturnType)
                        {
                            result = $"{result}{ReturnType} __return__, ";
                        }

                        foreach (var parameter in OutputParameters)
                        {
                            result = $"{result}{parameter.Type} {parameter.Name}, ";
                        }

                        if (!string.IsNullOrEmpty(result))
                        {
                            result = result.Remove(result.Length - 2);
                        }
                        
                        return $"({result})";
                    }

                    if (HasReturnType)
                    {
                        result = $"{result}{ReturnType}, ";
                    }

                    foreach (var parameter in OutputParameters)
                    {
                        result = $"{result}{parameter.Type}, ";
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.Remove(result.Length - 2);
                    }

                    return result;
                }
            }

            public string KeyTypeArgs
            {
                get
                {
                    string result = string.Empty;
                    if (!IsUseStaticTypeCache)
                    {
                        foreach (var parameter in GenericParameters)
                        {
                            result = $"{result}int, ";
                        }
                    }

                    foreach (var parameter in InputParameters)
                    {
                        result = $"{result}{parameter.Type}, ";
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.Remove(result.Length - 2);
                    }

                    return result;
                }
            }

            public string Args
            {
                get
                {
                    string result = string.Empty;
                    foreach (var parameter in Parameters)
                    {
                        result = $"{result}{parameter.ModifieredName}, ";
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.Remove(result.Length - 2);
                    }

                    return result;
                }
            }

            public string[] ResultReturnCodes
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

            public string Comparer
            {
                get
                {
                    if (CacheComparer == null)
                    {
                        var args = KeyTypeArgs;
                        if (Parameters.LastOrDefault().Modifier?.Contains("params") ?? false)
                        {
                            return $"Katuusagi.MemoizationForUnity.ParamsEqualityComparer<{args.Remove(args.Length - 2, 2)}>.Default";
                        }

                        if (CachingStyle == CachingStyle.SingleKey)
                        {
                            return string.Empty;
                        }

                        return $"Katuusagi.MemoizationForUnity.MemoizationEqualityComparer<{args}>.Default";
                    }

                    return CacheComparer;
                }
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

            public string ModifieredTypeName
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

            public string ModifieredName
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

            public bool IsOutput
            {
                get
                {
                    return Modifier.Contains("ref") || Modifier.Contains("out");
                }
            }

            public bool IsInput
            {
                get
                {
                    return !Modifier.Contains("out");
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            using (ContextUtils.LogScope(context))
            {
                try
                {
                    var rootInfos = CreateMemoizationRootInfos(context);
                    foreach (var rootInfo in rootInfos)
                    {
                        var root = new RootGenerator();
                        root.Generate(rg =>
                        {
                            foreach (var @using in rootInfo.Usings)
                            {
                                rg.Using.Generate(@using);
                            }

                            if (string.IsNullOrEmpty(rootInfo.NameSpace))
                            {
                                GenerateAncestor(rg.Type, rootInfo, rootInfo.Ancestors);
                                return;
                            }

                            rg.Namespace.Generate(rootInfo.NameSpace, ng =>
                            {
                                GenerateAncestor(ng.Type, rootInfo, rootInfo.Ancestors);
                            });
                        });
                        var builder = new CSharpScriptBuilder();
                        builder.BuildAndNewLine(root.Result);
                        context.AddSource($"{rootInfo.FileName}.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
                    }
                }
                catch (Exception e)
                {
                    ContextUtils.Log(e);
                }
            }
        }

        private void GenerateAncestor(TypeGenerator typeGenerator, RootInfo rootInfo, IEnumerable<AncestorInfo> ancestors)
        {
            if (!ancestors.Any())
            {
                GenerateType(typeGenerator, rootInfo);
                return;
            }

            var ancestor = ancestors.First();
            typeGenerator.Generate(ancestor.Modifier, ancestor.Name, tg =>
            {
                GenerateAncestor(tg.Type, rootInfo, ancestors.Skip(1));
            });
        }

        private void GenerateType(TypeGenerator typeGenerator, RootInfo rootInfo)
        {
            var guid = Guid.NewGuid().ToString().Replace("-", string.Empty);
            var cacheValues = $"__MemoizationCacheValues_{guid}__";
            var concurrentCacheValues = $"__MemoizationThreadSafeCacheValues_{guid}__";
            typeGenerator.Generate(rootInfo.Modifier, rootInfo.Name, tg =>
            {
                if (rootInfo.HasGenericClearableStaticTypeCache)
                {
                    tg.Field.Generate(ModifierType.Private | ModifierType.Static, "System.Collections.Generic.Stack<System.Collections.IDictionary>", cacheValues, "new System.Collections.Generic.Stack<System.Collections.IDictionary>()");
                }

                if (rootInfo.HasThreadSafeGenericClearableStaticTypeCache)
                {
                    tg.Field.Generate(ModifierType.Private | ModifierType.Static, "System.Collections.Concurrent.ConcurrentStack<System.Collections.IDictionary>", concurrentCacheValues, "new System.Collections.Concurrent.ConcurrentStack<System.Collections.IDictionary>()");
                }

                foreach (var method in rootInfo.Methods)
                {
                    var cacheValueTypeDeclarationName = method.CacheValueTypeDeclarationName;
                    var cacheValueType = method.CacheValueTypeName;
                    var cachingStyle = method.CachingStyle;
                    var comparer = method.Comparer;

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

                            ttg.Field.Generate(ModifierType.Public | ModifierType.Static, cacheValueTypeDeclarationName, "Cache", method.FieldInitializer);
                            if (cachingStyle != CachingStyle.Direct)
                            {
                                if (method.IsClearable && method.HasGenericParameter)
                                {
                                    ttg.Method.Generate(ModifierType.Static, string.Empty, method.StaticFieldWrappedType, cg =>
                                    {
                                        if (method.IsThreadSafe)
                                        {
                                            cg.Code.Generate($"{concurrentCacheValues}.Push(Cache);");
                                        }
                                        else
                                        {
                                            cg.Code.Generate($"{cacheValues}.Push(Cache);");
                                        }
                                    });
                                }
                            }
                        });
                    }
                    else
                    {
                        tg.Field.Generate(method.CacheFieldModifier, cacheValueTypeDeclarationName, method.CacheValueName, method.FieldInitializer);
                    }

                    GenerateCacheMethod(tg.Method, method);
                }

                var clearableInstanceMethods = rootInfo.Methods.Where(v => v.IsClearable && !v.IsUseStaticCache).ToArray();
                if (clearableInstanceMethods.Any())
                {
                    tg.Method.Generate(ModifierType.Public, "void", "ClearInstanceMemoizationCache", mg =>
                    {
                        foreach (var method in clearableInstanceMethods)
                        {
                            if (method.IsUseBaseTypeCache)
                            {
                                mg.Code.Generate($"{method.CacheValueName}?.Clear();");
                            }
                            else
                            {
                                mg.Code.Generate($"{method.CacheValueName}.Clear();");
                            }
                        }
                    });
                }

                var clearableStaticMethods = rootInfo.Methods.Where(v => v.IsClearable && v.IsUseStaticCache && !v.HasGenericParameter).ToArray();
                if (clearableStaticMethods.Any() || rootInfo.HasGenericClearableStaticTypeCache)
                {
                    tg.Method.Generate(ModifierType.Public | ModifierType.Static, "void", "ClearStaticMemoizationCache", mg =>
                    {
                        if (rootInfo.HasGenericClearableStaticTypeCache)
                        {
                            mg.Code.Generate($"foreach (var value in {cacheValues})", () =>
                            {
                                mg.Code.Generate($"value.Clear();");
                            });
                        }

                        if (rootInfo.HasThreadSafeGenericClearableStaticTypeCache)
                        {
                            mg.Code.Generate($"foreach (var value in {concurrentCacheValues})", () =>
                            {
                                mg.Code.Generate($"value.Clear();");
                            });
                        }

                        foreach (var method in clearableStaticMethods)
                        {
                            mg.Code.Generate($"{method.CacheValueName}.Clear();");
                        }
                    });
                }
            });
        }

        private void GenerateCacheMethod(MethodGenerator methodGenerator, MethodInfo method)
        {
            var key = method.Key;
            var result = method.Result;
            var cacheValue = method.CacheValueName;

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
                    mg.Code.Generate($"{cacheValueTypeName} __table__;");
                    mg.Code.Generate($"if ({cacheValue} == null)", () =>
                    {
                        mg.Code.Generate($"__table__ = {method.FieldValue};");
                        mg.Code.Generate($"{cacheValue} = __table__;");
                    });
                    mg.Code.Generate($"else", () =>
                    {
                        mg.Code.Generate($"__table__ = {cacheValue} as {cacheValueTypeName};");
                    });

                    cacheValue = "__table__";
                }

                switch (method.CachingStyle)
                {
                    case CachingStyle.Direct:
                        mg.Code.Generate($"return {cacheValue};");
                        return;
                    case CachingStyle.NoneKey:
                        mg.Code.Generate($"if ({cacheValue}.IsCached)", () =>
                        {
                            mg.Code.Generate($"var __cache__ = {cacheValue}.Result;");
                            foreach (var code in method.ResultReturnCodes)
                            {
                                mg.Code.Generate(code);
                            }
                        });

                        if (method.HasReturnType)
                        {
                            mg.Code.Generate($"var __result__ = {method.CallRawMethod};");
                        }
                        else
                        {
                            mg.Code.Generate($"{method.CallRawMethod};");
                        }
                        mg.Code.Generate($"{cacheValue}.Set({result});");

                        if (method.HasReturnType)
                        {
                            mg.Code.Generate($"return __result__;");
                        }
                        return;
                    case CachingStyle.SingleKey:
                    case CachingStyle.MultipleKey:
                        mg.Code.Generate($"var __key__ = {key};");
                        mg.Code.Generate($"if ({cacheValue}.TryGetValue(__key__, out var __cache__))", () =>
                        {
                            foreach (var code in method.ResultReturnCodes)
                            {
                                mg.Code.Generate(code);
                            }
                        });

                        if (method.HasReturnType)
                        {
                            mg.Code.Generate($"var __result__ = {method.CallRawMethod};");
                        }
                        else
                        {
                            mg.Code.Generate($"{method.CallRawMethod};");
                        }

                        if (method.IsThreadSafe)
                        {
                            mg.Code.Generate($"{cacheValue}.TryAdd(__key__, {result});");
                        }
                        else
                        {
                            mg.Code.Generate($"{cacheValue}.Add(__key__, {result});");
                        }

                        if (method.HasReturnType)
                        {
                            mg.Code.Generate($"return __result__;");
                        }
                        return;
                }
            });
        }

        private List<RootInfo> CreateMemoizationRootInfos(GeneratorExecutionContext context)
        {
            var result = new List<RootInfo>();
            var typeGroups = context.Compilation.SyntaxTrees
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
                    return new AncestorInfo()
                    {
                        Modifier = ScriptGeneratorUtils.GetModifierType(v.Keyword.Text) | ScriptGeneratorUtils.GetModifierType(v.Modifiers.ToString()),
                        Name = v.Identifier.ToString(),
                    };
                }).ToArray();

                var usings = firstType.GetAncestorUsings().Select(v =>
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
                }).ToArray();

                var rootInfo = new RootInfo()
                {
                    Modifier = typeModifier,
                    Ancestors = ancestors,
                    NameSpace = firstType.GetNameSpace(),
                    Name = firstType.Identifier.ToString(),
                    Usings = usings,
                    Methods = new List<MethodInfo>()
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
                            context.Error("MEMOIZATION001", "Memoization failed.", "Memoization is partial type member only.", type.GetLocation());
                            continue;
                        }

                        var methodName = method.Identifier.ToString();
                        ModifierType methodModifier;
                        var modifierLabel= memoize.GetArgument("Modifier")?.Expression?.ToString()?.Replace("\"", string.Empty);
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
                            context.Error("MEMOIZATION002", "Memoization failed.", "Memoization is static member or class instance member only.", method.GetLocation());
                            continue;
                        }

                        var nameofMatch = new Regex("^nameof\\(.*\\)$");
                        var memoizationMethodName = memoize.GetArgument("MethodName")?.Expression?.ToString()?.Replace("\"", string.Empty);
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
                        else if (nameofMatch.IsMatch(memoizationMethodName))
                        {
                            memoizationMethodName = memoizationMethodName.Substring(7, memoizationMethodName.Length - 8).Split('.').Last();
                        }

                        var cacheComparer = memoize.GetArgument("CacheComparer")?.Expression?.ToString()?.Replace("\"", string.Empty);
                        var isClearable = memoize.GetArgument("IsClearable")?.Expression?.ToString() == "true";
                        var isThreadSafe = memoize.GetArgument("IsThreadSafe")?.Expression?.ToString() == "true";

                        var methodInfo = new MethodInfo()
                        {
                            Id = Guid.NewGuid().ToString().Replace("-", string.Empty),
                            Attributes = method.AttributeLists.SelectMany(v => v?.Attributes.Where(v2 => v2 != memoize).Select(v2 => v2.ToFullString()) ?? Array.Empty<string>()).ToArray(),
                            Name = memoizationMethodName,
                            RawName = methodName,
                            ReturnType = method.ReturnType.ToString(),
                            Modifier = methodModifier,
                            CacheComparer = cacheComparer,
                            IsClearable = isClearable,
                            IsThreadSafe = isThreadSafe,
                            GenericParameters = new List<GenericParameterInfo>(),
                            Parameters = new List<ParameterInfo>(),
                        };

                        if (method.TypeParameterList != null &&
                            method.ParameterList != null)
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

                            methodInfo.Parameters.Add(parameterInfo);
                        }

                        if (!methodInfo.OutputParameters.Any() && !methodInfo.HasReturnType)
                        {
                            context.Error("MEMOIZATION003", "Memoization failed.", "Memoization must always return. Or it must have an out or ref parameter.", method.GetLocation());
                            continue;
                        }

                        if (methodInfo.CacheKeyCount > 8)
                        {
                            context.Error("MEMOIZATION004", "Memoization failed.", "Memoization does not allow more than 8 inputs.");
                            continue;
                        }

                        rootInfo.Methods.Add(methodInfo);
                    }
                }

                if (rootInfo.Methods.Any())
                {
                    result.Add(rootInfo);
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
