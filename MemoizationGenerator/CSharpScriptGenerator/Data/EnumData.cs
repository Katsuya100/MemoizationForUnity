using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class EnumData
    {
        public ModifierType Modifier = ModifierType.None;
        public string Name = string.Empty;
        public List<PreProcessData> PreProcesses = new List<PreProcessData>();
        public List<AttributeData> Attributes = new List<AttributeData>();
        public List<BaseTypeData> BaseTypes = new List<BaseTypeData>();
        public List<EnumValueData> EnumValues = new List<EnumValueData>();
    }
}
