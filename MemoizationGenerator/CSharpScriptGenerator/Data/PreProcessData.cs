using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class PreProcessData
    {
        public PreProcessType PreProcessType = PreProcessType.If;
        public string Symbol = string.Empty;
        public List<PreProcessData> PreProcesses = new List<PreProcessData>();
        public List<UsingData> Usings = new List<UsingData>();
        public List<NamespaceData> Namespaces = new List<NamespaceData>();
        public List<EnumValueData> EnumValues = new List<EnumValueData>();
        public List<EventData> Events = new List<EventData>();
        public List<FieldData> Fields = new List<FieldData>();
        public List<PropertyData> Properties = new List<PropertyData>();
        public List<MethodData> Methods = new List<MethodData>();
        public List<DelegateData> Delegates = new List<DelegateData>();
        public List<EnumData> Enums = new List<EnumData>();
        public List<TypeData> Types = new List<TypeData>();
    }
}
