using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class TypeData
    {
        public ModifierType Modifier = ModifierType.None;
        public string Name = string.Empty;
        public List<PreProcessData> PreProcesses = new List<PreProcessData>();
        public List<AttributeData> Attributes = new List<AttributeData>();
        public List<GenericParameterData> GenericParams = null;
        public List<ITypeNameData> BaseTypes = new List<ITypeNameData>();
        public List<EventData> Events = new List<EventData>();
        public List<FieldData> Fields = new List<FieldData>();
        public List<PropertyData> Properties = new List<PropertyData>();
        public List<MethodData> Methods = new List<MethodData>();
        public List<DelegateData> Delegates = new List<DelegateData>();
        public List<EnumData> Enums = new List<EnumData>();
        public List<TypeData> Types = new List<TypeData>();
    }
}
