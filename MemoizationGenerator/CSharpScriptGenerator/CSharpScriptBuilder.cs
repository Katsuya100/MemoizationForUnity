using System.Collections.Generic;
using System.Linq;

namespace Katuusagi.CSharpScriptGenerator
{
    public class CSharpScriptBuilder : ScriptBuilderBase
    {
        private const ModifierType AccessorModifier = ModifierType.Private | ModifierType.Protected | ModifierType.Public | ModifierType.Internal;

        private const ModifierType ClassAllowedModifier = AccessorModifier | ModifierType.Unsafe | ModifierType.Sealed | ModifierType.Static | ModifierType.Abstract| ModifierType.Partial  | ModifierType.Class;
        private const ModifierType StructAllowedModifier = AccessorModifier | ModifierType.Unsafe| ModifierType.ReadOnly | ModifierType.Ref  | ModifierType.Partial | ModifierType.Struct;
        private const ModifierType InterfaceAllowedModifier = AccessorModifier | ModifierType.Unsafe | ModifierType.Partial | ModifierType.Interface;
        private const ModifierType RecordAllowedModifier = AccessorModifier | ModifierType.Unsafe | ModifierType.Sealed | ModifierType.Abstract | ModifierType.Partial | ModifierType.Record;
        private const ModifierType DelegateAllowedModifier = AccessorModifier | ModifierType.Unsafe;
        private const ModifierType EnumAllowedModifier = AccessorModifier;

        private const ModifierType EventAllowedModifier = AccessorModifier | ModifierType.Unsafe | ModifierType.Sealed | ModifierType.Static | ModifierType.Extern | ModifierType.Abstract | ModifierType.Virtual | ModifierType.Override | ModifierType.New;
        private const ModifierType FieldAllowedModifier = AccessorModifier | ModifierType.Unsafe | ModifierType.Static | ModifierType.Const | ModifierType.Volatile | ModifierType.New | ModifierType.ReadOnly;
        private const ModifierType PropertyAllowedModifier = AccessorModifier | ModifierType.Unsafe | ModifierType.Sealed | ModifierType.Static | ModifierType.Extern | ModifierType.Abstract | ModifierType.Virtual | ModifierType.Override | ModifierType.New | ModifierType.ReadOnly | ModifierType.Ref | ModifierType.ReturnReadOnly;
        private const ModifierType PropertyMethodAllowedModifier = AccessorModifier;
        private const ModifierType MethodAllowedModifier = AccessorModifier | ModifierType.Unsafe | ModifierType.Sealed | ModifierType.Static| ModifierType.Extern | ModifierType.Abstract | ModifierType.Virtual | ModifierType.Override | ModifierType.New | ModifierType.Async | ModifierType.ReadOnly | ModifierType.Partial;

        private const ModifierType DelegateReturnAllowedModifier = ModifierType.Ref | ModifierType.ReturnReadOnly;
        private const ModifierType MethodReturnAllowedModifier = ModifierType.Ref | ModifierType.ReturnReadOnly;
        private const ModifierType ParameterAllowedModifier = ModifierType.This | ModifierType.Ref | ModifierType.In | ModifierType.Out | ModifierType.Params;
        private const ModifierType GenericParameterAllowedModifier = ModifierType.In | ModifierType.Out;

        public void BuildAndNewLine(RootData data)
        {
            if (data == null)
            {
                return;
            }

            BuildAndNewLine(data.PreProcesses, null, false);

            foreach (var uzing in data.Usings)
            {
                BuildAndNewLine(uzing);
            }

            foreach (var namespase in data.Namespaces)
            {
                BuildAndNewLine(namespase);
            }

            foreach (var del in data.Delegates)
            {
                BuildAndNewLine(del);
            }

            foreach (var enu in data.Enums)
            {
                BuildAndNewLine(enu);
            }

            foreach (var type in data.Types)
            {
                BuildAndNewLine(type);
            }
        }

        public void BuildAndNewLine(PreProcessData data, string parentName, bool isStructMember)
        {
            if (data == null)
            {
                return;
            }

            BuildAndNewLine(data.PreProcesses, parentName, isStructMember);

            foreach (var uzing in data.Usings)
            {
                BuildAndNewLine(uzing);
            }

            foreach (var namespase in data.Namespaces)
            {
                BuildAndNewLine(namespase);
            }

            foreach (var eve in data.Events)
            {
                BuildAndNewLine(eve);
            }

            foreach (var field in data.Fields)
            {
                BuildAndNewLine(field, !isStructMember);
            }

            foreach (var property in data.Properties)
            {
                BuildAndNewLine(property);
            }

            foreach (var method in data.Methods)
            {
                if (method.Name == parentName && isStructMember)
                {
                    BuildAndNewLine(method, data.Fields);
                    continue;
                }

                BuildAndNewLine(method, null);
            }

            foreach (var enumValue in data.EnumValues)
            {
                BuildAndNewLine(enumValue);
            }

            foreach (var del in data.Delegates)
            {
                BuildAndNewLine(del);
            }

            foreach (var enu in data.Enums)
            {
                BuildAndNewLine(enu);
            }

            foreach (var type in data.Types)
            {
                BuildAndNewLine(type);
            }
        }

        public void BuildAndNewLine(UsingData data)
        {
            if (data == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(data.NameSpace))
            {
                Append("using ");
                Append(data.NameSpace);
                AppendLine(";");
            }
        }

        public void BuildAndNewLine(NamespaceData data)
        {
            if (data == null)
            {
                return;
            }

            Append("namespace ");
            AppendLine(data.Name);
            StartScope();

            BuildAndNewLine(data.PreProcesses, data.Name, false);

            foreach (var @using in data.Usings)
            {
                BuildAndNewLine(@using);
            }

            foreach (var @namespace in data.Namespaces)
            {
                BuildAndNewLine(@namespace);
            }

            foreach (var del in data.Delegates)
            {
                BuildAndNewLine(del);
            }

            foreach (var enu in data.Enums)
            {
                BuildAndNewLine(enu);
            }

            foreach (var type in data.Types)
            {
                BuildAndNewLine(type);
            }

            EndScope();
        }

        public void BuildAndNewLine(DelegateData data)
        {
            if (data == null)
            {
                return;
            }

            foreach (var attribute in data.Attributes)
            {
                BuildAndNewLine(attribute);
            }

            BuildAndNewLineAttribute(data.ReturnType);
            var mod = data.Modifier & DelegateAllowedModifier;
            Append(mod.GetModifierLabel());
            Append("delegate ");
            mod = data.Modifier & DelegateReturnAllowedModifier;
            Append(mod.GetModifierLabel());
            Build(data.ReturnType);
            Append(data.Name);

            Build(data.GenericParams);
            Build(data.Params);
            AppendLine(";");
        }

        public void BuildAndNewLine(EnumData data)
        {
            if (data == null)
            {
                return;
            }

            foreach (var attribute in data.Attributes)
            {
                BuildAndNewLine(attribute);
            }

            var mod = data.Modifier & EnumAllowedModifier;
            Append(mod.GetModifierLabel());
            Append("enum ");
            Append(data.Name);
            switch (data.BaseType)
            {
                case EnumBaseType.SByte:
                    Append(": sbyte");
                    break;
                case EnumBaseType.Byte:
                    Append(": byte");
                    break;
                case EnumBaseType.Short:
                    Append(": short");
                    break;
                case EnumBaseType.UShort:
                    Append(": ushort");
                    break;
                case EnumBaseType.Int:
                    Append(": int");
                    break;
                case EnumBaseType.UInt:
                    Append(": uint");
                    break;
                case EnumBaseType.Long:
                    Append(": long");
                    break;
                case EnumBaseType.ULong:
                    Append(": ulong");
                    break;
            }

            AppendLine();

            StartScope();

            BuildAndNewLine(data.PreProcesses, data.Name, false);

            foreach (var enumValue in data.EnumValues)
            {
                BuildAndNewLine(enumValue);
            }

            EndScope();
        }

        public void BuildAndNewLine(EnumValueData data)
        {
            if (data == null)
            {
                return;
            }

            foreach (var attribute in data.Attributes)
            {
                BuildAndNewLine(attribute);
            }

            Append(data.Name);
            if (data.Default != null)
            {
                Append(" = ");
                Build(data.Default);
            }

            AppendLine(",");
        }

        public void BuildAndNewLine(TypeData data)
        {
            if (data == null)
            {
                return;
            }
            var isClass = data.Modifier.HasFlag(ModifierType.Class);
            var isStruct = !isClass && data.Modifier.HasFlag(ModifierType.Struct);
            var isInterface = !isClass && !isStruct && data.Modifier.HasFlag(ModifierType.Struct);
            var isRecord = !isClass && !isStruct && !isInterface && data.Modifier.HasFlag(ModifierType.Record);
            var isPartial = data.Modifier.HasFlag(ModifierType.Partial);
            if (isPartial && isStruct)
            {
                AppendLine("#pragma warning disable CS0282");
            }

            foreach (var attribute in data.Attributes)
            {
                BuildAndNewLine(attribute);
            }

            var mod = data.Modifier;
            if (isClass)
            {
                mod &= ClassAllowedModifier;
            }
            else if (isStruct)
            {
                mod &= StructAllowedModifier;
            }
            else if (isInterface)
            {
                mod &= InterfaceAllowedModifier;
            }
            else if (isRecord)
            {
                mod &= RecordAllowedModifier;
            }

            Append(mod.GetModifierLabel());
            Append(data.Name);
            Build(data.GenericParams);
            if (data.BaseTypes.Any())
            {
                Append(":");
                for (int i = 0; i < data.BaseTypes.Count; ++i)
                {
                    var baseType = data.BaseTypes[i];
                    if (!(baseType?.IsEmpty ?? true))
                    {
                        Build(baseType);
                        Append(", ");
                    }
                }
                RemoveBack(2);
            }

            AppendLine();

            foreach (var genericParam in data.GenericParams)
            {
                BuildAndNewLineWhere(genericParam);
            }

            if (isPartial && isStruct)
            {
                AppendLine("#pragma warning restore CS0282");
            }

            StartScope();

            BuildAndNewLine(data.PreProcesses, data.Name, isStruct);

            foreach (var eve in data.Events)
            {
                BuildAndNewLine(eve);
            }

            foreach (var field in data.Fields)
            {
                BuildAndNewLine(field, !isStruct);
            }

            foreach (var prop in data.Properties)
            {
                BuildAndNewLine(prop);
            }

            foreach (var method in data.Methods)
            {
                if (method.Name == data.Name && isStruct)
                {
                    BuildAndNewLine(method, data.Fields);
                    continue;
                }

                BuildAndNewLine(method, null);
            }

            foreach (var del in data.Delegates)
            {
                BuildAndNewLine(del);
            }

            foreach (var enu in data.Enums)
            {
                BuildAndNewLine(enu);
            }

            foreach (var type in data.Types)
            {
                BuildAndNewLine(type);
            }
            EndScope();
        }

        public void Build(GenericParameterData data)
        {
            if (data == null)
            {
                return;
            }

            foreach (var attribute in data.Attributes)
            {
                Build(attribute);
            }

            var mod = data.Modifier & GenericParameterAllowedModifier;
            Append(mod.GetModifierLabel());
            Append(data.Name);
        }


        public void BuildWhere(GenericParameterData data)
        {
            if (data == null ||
                !data.Wheres.Any())
            {
                return;
            }

            Append("where ");
            Append(data.Name);
            Append(": ");

            foreach (var where in data.Wheres)
            {
                Build(where);
                Append(", ");
            }

            RemoveBack(2);
        }

        public void BuildAndNewLineWhere(GenericParameterData data)
        {
            if (data == null ||
                !data.Wheres.Any())
            {
                return;
            }

            BuildWhere(data);
            AppendLine();
        }

        public void Build(WhereData data)
        {
            if (data == null)
            {
                return;
            }

            Append(data.Where);
        }

        public void BuildAndNewLine(AttributeData data, string target = null)
        {
            if (data == null)
            {
                return;
            }

            Build(data, target);
            AppendLine();
        }

        public void Build(AttributeData data, string target = null)
        {
            if (data == null)
            {
                return;
            }

            if (!(data.Type?.IsEmpty ?? true))
            {
                Append("[");
                if (!string.IsNullOrEmpty(target))
                {
                    Append(target);
                    Append(": ");
                }
                Build(data.Type);
                if (data.Args.Any())
                {
                    Append("(");
                    foreach (var arg in data.Args)
                    {
                        Build(arg);
                        Append(", ");
                    }
                    RemoveBack(2);
                    Append(")");
                }
                Append("]");
            }
        }

        public void BuildAndNewLine(EventData data)
        {
            if (data == null)
            {
                return;
            }

            foreach (var attribute in data.Attributes)
            {
                BuildAndNewLine(attribute);
            }

            var mod = data.Modifier & EventAllowedModifier;
            Append(mod.GetModifierLabel());
            Append("event ");
            if (!(data.Type?.IsEmpty ?? true))
            {
                Build(data.Type);
                Append(" ");
            }
            Append(data.Name);


            if (data.Default != null)
            {
                Append(" = ");
                Build(data.Default);
                AppendLine(";");
            }
            else if (data.Add != null || data.Remove != null)
            {
                AppendLine();
                StartScope();
                BuildAndNewLine("add", data.Add);
                BuildAndNewLine("remove", data.Remove);
                EndScope();
            }
            else
            {
                AppendLine(";");
            }
        }

        public void BuildAndNewLine(FieldData data, bool enableDefaultValue)
        {
            if (data == null)
            {
                return;
            }

            if (data.Modifier.HasFlag(ModifierType.Static) ||
                data.Modifier.HasFlag(ModifierType.Const))
            {
                enableDefaultValue = true;
            }

            foreach (var attribute in data.Attributes)
            {
                BuildAndNewLine(attribute);
            }

            var mod = data.Modifier & FieldAllowedModifier;
            Append(mod.GetModifierLabel());
            if (!(data.Type?.IsEmpty ?? true))
            {
                Build(data.Type);
                Append(" ");
            }
            Append(data.Name);
            if (enableDefaultValue &&
                data.Default != null)
            {
                Append(" = ");
                Build(data.Default);
            }

            AppendLine(";");
        }

        public void BuildAndNewLine(PropertyData data)
        {
            if (data == null)
            {
                return;
            }

            foreach (var attribute in data.Attributes)
            {
                BuildAndNewLine(attribute);
            }

            var mod = data.Modifier & PropertyAllowedModifier;
            Append(mod.GetModifierLabel());
            if (!(data.Type?.IsEmpty ?? true))
            {
                Build(data.Type);
                Append(" ");
            }
            Append(data.Name);
            BuildIndexer(data.Params);

            AppendLine();

            StartScope();
            BuildAndNewLine("get", data.Get);
            BuildAndNewLine("set", data.Set);
            EndScope();

            if (data.Default != null)
            {
                Append(" = ");
                Build(data.Default);
                AppendLine(";");
            }
        }

        public void BuildAndNewLine(string name, PropertyMethodData data)
        {
            if (data == null)
            {
                return;
            }

            foreach (var attribute in data.Attributes)
            {
                BuildAndNewLine(attribute);
            }

            foreach (var attribute in data.ParamAttributes)
            {
                BuildAndNewLine(attribute, "param");
            }

            foreach (var attribute in data.ReturnAttributes)
            {
                BuildAndNewLine(attribute, "return");
            }

            var mod = data.Modifier & PropertyMethodAllowedModifier;
            Append(mod.GetModifierLabel());
            Append(name);

            if (data.Statements.Any())
            {
                AppendLine(string.Empty);
                StartScope();
                foreach (var statement in data.Statements)
                {
                    BuildAndNewLine(statement);
                }
                EndScope();
            }
            else
            {
                AppendLine(";");
            }
        }

        public void BuildAndNewLine(MethodData data, IEnumerable<FieldData> initFields)
        {
            if (data == null)
            {
                return;
            }

            foreach (var attribute in data.Attributes)
            {
                BuildAndNewLine(attribute);
            }

            BuildAndNewLineAttribute(data.ReturnType);

            var mod = data.Modifier & MethodAllowedModifier;
            Append(mod.GetModifierLabel());
            mod = data.Modifier & MethodReturnAllowedModifier;
            Append(mod.GetModifierLabel());
            Build(data.ReturnType);
            Append(data.Name);

            Build(data.GenericParams);
            Build(data.Params);
            if (data.Modifier.HasFlag(ModifierType.Abstract) ||
                data.Modifier.HasFlag(ModifierType.Extern) ||
                (!data.Statements.Any() && (data.Modifier.HasFlag(ModifierType.Partial) || data.Modifier == ModifierType.None)))
            {
                if (data.GenericParams.Any(v => v.Wheres.Any()))
                {
                    AppendLine("");
                    for (int i = 0; i < data.GenericParams.Count - 1; ++i)
                    {
                        BuildAndNewLineWhere(data.GenericParams[i]);
                    }

                    BuildWhere(data.GenericParams.Last());
                }
                AppendLine(";");
            }
            else
            {
                AppendLine(string.Empty);
                foreach (var genericParam in data.GenericParams)
                {
                    BuildAndNewLineWhere(genericParam);
                }

                StartScope();
                if (initFields != null)
                {
                    foreach (var initField in initFields)
                    {
                        if (initField.Default == null)
                        {
                            continue;
                        }

                        Append(initField.Name);
                        Append(" = ");
                        Build(initField.Default);
                        AppendLine(";");
                    }
                }

                foreach (var statement in data.Statements)
                {
                    BuildAndNewLine(statement);
                }
                EndScope();
            }
        }

        public void BuildAndNewLineAttribute(ReturnTypeData data)
        {
            if (data == null)
            {
                return;
            }

            foreach (var attribute in data.Attributes)
            {
                BuildAndNewLine(attribute, "return");
            }

        }

        public void Build(ReturnTypeData data)
        {
            if (data == null)
            {
                return;
            }

            if (!(data.Type?.IsEmpty ?? true))
            {
                Build(data.Type);
                Append(" ");
            }
        }

        public void Build(ParameterData data)
        {
            if (data == null)
            {
                return;
            }

            foreach (var attribute in data.Attributes)
            {
                Build(attribute);
            }

            var mod = data.Modifier & ParameterAllowedModifier;
            Append(mod.GetModifierLabel());

            if (!(data.Type?.IsEmpty ?? true))
            {
                Build(data.Type);
                Append(" ");
            }
            Append(data.Name);
            if (data.Default != null)
            {
                Append(" = ");
                Build(data.Default);
            }
        }

        public void Build(ITypeNameData data)
        {
            if (data?.IsEmpty ?? true)
            {
                return;
            }

            if (data is NamespaceNameData @namespace)
            {
                Build(@namespace);
                return;
            }

            if (data is TypeNameData typeName)
            {
                Build(typeName);
                return;
            }

            if (data is TupleTypeNameData tupleTypeName)
            {
                Build(tupleTypeName);
                return;
            }
        }

        public void Build(NamespaceNameData data)
        {
            if (data?.IsEmpty ?? true)
            {
                return;
            }

            Append(data.Name);
            Append(".");
            Build(data.Child);
        }

        public void Build(TypeNameData data)
        {
            if (data?.IsEmpty ?? true)
            {
                return;
            }

            Append(data.Name);

            if (data.Parameters.Any())
            {
                Append("<");
                foreach (var parameter in data.Parameters)
                {
                    if (!(parameter?.IsEmpty ?? true))
                    {
                        Build(parameter);
                        Append(", ");
                    }
                }

                RemoveBack(2);
                Append(">");
            }
        }

        public void Build(TupleTypeNameData data)
        {
            if (data?.IsEmpty ?? true)
            {
                return;
            }

            if (data.Parameters.Any())
            {
                Append("(");
                foreach (var parameter in data.Parameters)
                {
                    Build(parameter);
                    Append(", ");
                }

                RemoveBack(2);
                Append(")");
            }
        }

        public void Build(TupleParameterData data)
        {
            if (data == null)
            {
                return;
            }

            Build(data.Type);
            if (!string.IsNullOrEmpty(data.Name))
            {
                Append(" ");
                Append(data.Name);
            }
        }

        public void BuildAndNewLine(IStatementData data)
        {
            if (data == null)
            {
                return;
            }

            if (data is StatementCodeData code)
            {
                BuildAndNewLine(code);
            }
            else if (data is StatementTypeData type)
            {
                BuildAndNewLine(type);
            }
            else if (data is StatementPropertyData property)
            {
                BuildAndNewLine(property);
            }
            else if (data is StatementVariableData variable)
            {
                BuildAndNewLine(variable);
            }
            else if (data is StatementMethodData method)
            {
                BuildAndNewLine(method);
            }
            else if (data is StatementOperationData operation)
            {
                switch (operation.Operation)
                {
                    case StatementOperation.StartScope:
                        StartScope();
                        break;
                    case StatementOperation.EndScope:
                        EndScope();
                        break;
                }
            }
        }

        public void Build(IStatementData data)
        {
            if (data == null)
            {
                return;
            }

            if (data is StatementCodeData code)
            {
                Build(code);
                return;
            }

            if (data is StatementTypeData type)
            {
                Build(type);
                return;
            }

            if (data is StatementVariableData variable)
            {
                Build(variable);
                return;
            }

            if (data is StatementPropertyData property)
            {
                Build(property);
                return;
            }

            if (data is StatementMethodData method)
            {
                Build(method);
                return;
            }

            if (data is StatementOperationData operation)
            {
                switch (operation.Operation)
                {
                    case StatementOperation.StartScope:
                        Append("{ ");
                        return;
                    case StatementOperation.EndScope:
                        Append("} ");
                        return;
                }
                return;
            }
        }

        public void BuildAndNewLine(StatementCodeData data)
        {
            if (data == null)
            {
                return;
            }

            AppendLine(data.Line);
            if (!data.Statements.Any())
            {
                return;
            }

            StartScope();
            foreach (var statement in data.Statements)
            {
                BuildAndNewLine(statement);
            }
            EndScope();
        }

        public void Build(StatementCodeData data)
        {
            if (data == null)
            {
                return;
            }

            Append(data.Line);
            if (!data.Statements.Any())
            {
                return;
            }

            Append("{ ");
            foreach (var code in data.Statements)
            {
                Build(code);
            }
            Append(" }");
        }

        public void BuildAndNewLine(StatementTypeData data)
        {
            if (data == null)
            {
                return;
            }

            Build(data);
            AppendLine(";");
        }

        public void Build(StatementTypeData data)
        {
            if (data == null)
            {
                return;
            }

            if (!(data.Type?.IsEmpty ?? true))
            {
                Build(data.Type);
            }

            if (data.Statement != null)
            {
                Append(".");
                Build(data.Statement);
            }
        }

        public void BuildAndNewLine(StatementVariableData data)
        {
            if (data == null)
            {
                return;
            }

            Build(data);
            AppendLine(";");
        }

        public void Build(StatementVariableData data)
        {
            if (data == null)
            {
                return;
            }

            if (!(data.Type?.IsEmpty ?? true))
            {
                Build(data.Type);
                Append(" ");
            }

            Append(data.Name);

            if (data.SetValue != null)
            {
                Append(" = ");
                Build(data.SetValue);
            }
        }

        public void BuildAndNewLine(StatementPropertyData data)
        {
            if (data == null)
            {
                return;
            }

            Build(data);
            AppendLine(";");
        }

        public void Build(StatementPropertyData data)
        {
            if (data == null)
            {
                return;
            }

            if (data.Result != null)
            {
                Build(data.Result);
            }

            Append(data.Name);
            Append("[");
            foreach (var arg in data.Args)
            {
                Build(arg);
                Append(", ");
            }
            RemoveBack(2);
            Append("]");

            if (data.SetValue != null)
            {
                Append(" = ");
                Build(data.SetValue);
            }
        }

        public void BuildAndNewLine(StatementMethodData data)
        {
            if (data == null)
            {
                return;
            }

            Build(data);
            AppendLine(";");
        }

        public void Build(StatementMethodData data)
        {
            if (data == null)
            {
                return;
            }

            if (data.Result != null)
            {
                Build(data.Result);
            }

            Append(data.Name);

            if (data.GenericArgs.Any())
            {
                Append("<");
                foreach (var arg in data.GenericArgs)
                {
                    Build(arg);
                    Append(", ");
                }
                RemoveBack(2);
                Append(">");
            }

            Append("(");
            foreach (var arg in data.Args)
            {
                Build(arg);
                Append(", ");
            }
            RemoveBack(2);
            Append(")");
        }

        public void Build(StatementGenericArgData data)
        {
            if (data == null)
            {
                return;
            }

            Append(data.Arg);
        }

        public void Build(List<GenericParameterData> data)
        {
            if (data == null ||
                !data.Any())
            {
                return;
            }

            Append("<");
            foreach (var param in data)
            {
                Build(param);
                Append(", ");
            }

            RemoveBack(2);
            Append(">");
        }

        public void Build(List<ParameterData> data)
        {
            Append("(");
            if (data != null &&
                data.Any())
            {
                foreach (var param in data)
                {
                    Build(param);
                    Append(", ");
                }

                RemoveBack(2);
            }

            Append(")");
        }

        public void BuildIndexer(List<ParameterData> data)
        {
            if (data == null ||
                !data.Any())
            {
                return;
            }

            Append("[");
            foreach (var param in data)
            {
                Build(param);
                Append(", ");
            }

            RemoveBack(2);
            Append("]");
        }

        public void BuildAndNewLine(List<PreProcessData> data, string parentName, bool isStructMember)
        {
            if (data == null ||
                !data.Any())
            {
                return;
            }

            PreProcessData prev = null;
            for (int i = 0; i < data.Count; ++i)
            {
                var current = data[i];

                var currentType = current.PreProcessType;
                if (prev == null)
                {
                    currentType = PreProcessType.If;
                }

                var existSymbol = !string.IsNullOrEmpty(current.Symbol);
                switch (currentType)
                {
                    case PreProcessType.If:
                        {
                            if (!existSymbol)
                            {
                                continue;
                            }

                            Append("#if ");
                            AppendLine(current.Symbol);
                        }
                        break;
                    case PreProcessType.ElseIf:
                        {
                            if (existSymbol)
                            {
                                Append("#elif ");
                                AppendLine(current.Symbol);
                            }
                            else
                            {
                                AppendLine("#else");
                            }
                        }
                        break;
                }

                BuildAndNewLine(current, parentName, !isStructMember);

                var nextType = i + 1 < data.Count ? data[i + 1].PreProcessType : PreProcessType.If;
                if (nextType == PreProcessType.If ||
                    (currentType == PreProcessType.ElseIf && !existSymbol))
                {
                    AppendLine("#endif");
                    prev = null;
                }
                else
                {
                    prev = current;
                }
            }
        }
    }
}
