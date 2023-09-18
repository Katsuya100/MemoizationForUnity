using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public enum EnumBaseType
    {
        None,
        SByte,
        Byte,
        Short,
        UShort,
        Int,
        UInt,
        Long,
        ULong,
    }

    public class EnumData
    {
        public ModifierType Modifier = ModifierType.None;
        public string Name = string.Empty;
        public List<PreProcessData> PreProcesses = new List<PreProcessData>();
        public List<AttributeData> Attributes = new List<AttributeData>();
        public EnumBaseType BaseType = EnumBaseType.None;
        public List<EnumValueData> EnumValues = new List<EnumValueData>();
    }
}
