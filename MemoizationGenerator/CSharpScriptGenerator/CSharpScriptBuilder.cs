using System.Collections.Generic;
using System.Linq;

namespace Katuusagi.CSharpScriptGenerator
{
    public class CSharpScriptBuilder : ScriptBuilderBase
    {
        private const ModifierType AccessorModifier = ModifierType.Private | ModifierType.Protected | ModifierType.Public | ModifierType.Internal;

        private const ModifierType ClassAllowedModifier = AccessorModifier | ModifierType.Static | ModifierType.Unsafe | ModifierType.Partial | ModifierType.Sealed | ModifierType.Abstract | ModifierType.Record | ModifierType.Class;
        private const ModifierType StructAllowedModifier = AccessorModifier | ModifierType.Partial | ModifierType.ReadOnly | ModifierType.Ref | ModifierType.Record | ModifierType.Struct;
        private const ModifierType InterfaceAllowedModifier = AccessorModifier | ModifierType.Unsafe | ModifierType.Partial | ModifierType.Interface;
        private const ModifierType DelegateAllowedModifier = AccessorModifier | ModifierType.Unsafe;
        private const ModifierType EnumAllowedModifier = AccessorModifier;

        private const ModifierType EventAllowedModifier = AccessorModifier | ModifierType.Static;
        private const ModifierType FieldAllowedModifier = AccessorModifier | ModifierType.Static | ModifierType.Const;
        private const ModifierType PropertyAllowedModifier = AccessorModifier | ModifierType.Static | ModifierType.Unsafe | ModifierType.Sealed | ModifierType.Virtual | ModifierType.Abstract | ModifierType.Override;
        private const ModifierType PropertyMethodAllowedModifier = AccessorModifier;
        private const ModifierType MethodAllowedModifier = AccessorModifier | ModifierType.Static | ModifierType.Unsafe | ModifierType.Partial | ModifierType.Sealed | ModifierType.Virtual | ModifierType.Abstract | ModifierType.Override;

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
                AppendLine($"using {data.NameSpace};");
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

            var mod = data.Modifier & DelegateAllowedModifier;
            Append(mod.GetModifierLabel());
            Append("delegate ");
            Append(data.Type);
            if (!string.IsNullOrEmpty(data.Type))
            {
                Append(" ");
            }
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
            if (data.BaseTypes.Any())
            {
                Append(":");
                Build(data.BaseTypes.First());
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
            if (!(data.Default?.IsEmpty ?? true))
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

            Append(mod.GetModifierLabel());
            Append(data.Name);
            Build(data.GenericParams);
            if (data.BaseTypes.Any())
            {
                Append(":");
                for (int i = 0; i < data.BaseTypes.Count; ++i)
                {
                    var baseType = data.BaseTypes[i];
                    Build(baseType);
                    Append(", ");
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

        public void Build(BaseTypeData data)
        {
            if (data == null)
            {
                return;
            }

            Append(data.Name);
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

            Append(data.Name);
        }

        public void BuildAndNewLineWhere(GenericParameterData data)
        {
            if (data == null ||
                !data.Wheres.Any())
            {
                return;
            }

            Append($"where {data.Name}: ");
            foreach (var where in data.Wheres)
            {
                Build(where);
                Append(", ");
            }

            RemoveBack(2);
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

        public void BuildAndNewLine(AttributeData data)
        {
            if (data == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(data.Attribute))
            {
                AppendLine($"[{data.Attribute}]");
            }
        }

        public void Build(AttributeData data)
        {
            if (data == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(data.Attribute))
            {
                Append($"[{data.Attribute}]");
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
            Append(data.Type);
            Append(" ");
            Append(data.Name);
            if (!(data.Default?.IsEmpty ?? true))
            {
                Append(" = ");
                Build(data.Default);
            }

            AppendLine(";");
        }

        public void BuildAndNewLine(FieldData data, bool enableDefaultValue)
        {
            if (data == null)
            {
                return;
            }

            if (data.Modifier.HasFlag(ModifierType.Static))
            {
                enableDefaultValue = true;
            }

            foreach (var attribute in data.Attributes)
            {
                BuildAndNewLine(attribute);
            }

            var mod = data.Modifier & FieldAllowedModifier;
            Append(mod.GetModifierLabel());
            Append(data.Type);
            Append(" ");
            Append(data.Name);
            if (enableDefaultValue &&
                !(data.Default?.IsEmpty ?? true))
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
            Append(data.Type);
            Append(" ");
            Append(data.Name);
            BuildIndexer(data.Params);

            AppendLine();

            StartScope();
            BuildAndNewLine(data.Get);
            BuildAndNewLine(data.Set);
            EndScope();

            if (!(data.Default?.IsEmpty ?? true))
            {
                Append(" = ");
                Build(data.Default);
                AppendLine(";");
            }
        }

        public void BuildAndNewLine(PropertyMethodData data)
        {
            if (data == null)
            {
                return;
            }

            foreach (var attribute in data.Attributes)
            {
                BuildAndNewLine(attribute);
            }

            var mod = data.Modifier & PropertyMethodAllowedModifier;
            Append(mod.GetModifierLabel());
            Append(data.Name);

            if (!(data.Code?.IsEmpty ?? true))
            {
                AppendLine(string.Empty);
                StartScope();
                BuildAndNewLine(data.Code);
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

            var mod = data.Modifier & MethodAllowedModifier;
            Append(mod.GetModifierLabel());
            Append(data.Type);
            if (!string.IsNullOrEmpty(data.Type))
            {
                Append(" ");
            }
            Append(data.Name);

            Build(data.GenericParams);
            Build(data.Params);

            if ((!data.Code?.IsEmpty ?? true) ||
                (mod != ModifierType.None &&
                !mod.HasFlag(ModifierType.Abstract)))
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
                        if (initField.Default?.IsEmpty ?? false)
                        {
                            continue;
                        }

                        AppendLine($"{initField.Name} = {initField.Default.Lines.FirstOrDefault()};");
                    }
                }
                BuildAndNewLine(data.Code);
                EndScope();
            }
            else
            {
                AppendLine(";");
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

            Append(data.Type);
            Append(" ");
            Append(data.Name);
            if (!(data.Default?.IsEmpty ?? true))
            {
                Append(" = ");
                Build(data.Default);
            }
        }

        public void BuildAndNewLine(CodeData data)
        {
            if (data == null)
            {
                return;
            }

            foreach (var line in data.Lines)
            {
                AppendLine(line);
            }
        }

        public void Build(CodeData data)
        {
            if (data == null)
            {
                return;
            }

            if (data.Lines.Any())
            {
                Append(data.Lines.First());
            }
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

                            AppendLine($"#if {current.Symbol}");
                        }
                        break;
                    case PreProcessType.ElseIf:
                        {
                            if (existSymbol)
                            {
                                AppendLine($"#elif {current.Symbol}");
                            }
                            else
                            {
                                AppendLine($"#else");
                            }
                        }
                        break;
                }

                BuildAndNewLine(current, parentName, !isStructMember);

                var nextType = i + 1 < data.Count ? data[i + 1].PreProcessType : PreProcessType.If;
                if (nextType == PreProcessType.If ||
                    (currentType == PreProcessType.ElseIf && !existSymbol))
                {
                    AppendLine($"#endif");
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
