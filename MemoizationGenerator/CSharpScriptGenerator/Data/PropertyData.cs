using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class PropertyData
    {
        public ModifierType Modifier = ModifierType.None;
        public string Name = string.Empty;
        public string Type = string.Empty;
        public List<AttributeData> Attributes = new List<AttributeData>();
        public List<ParameterData> Params = new List<ParameterData>();
        public PropertyMethodData Get = null;
        public PropertyMethodData Set = null;
        public CodeData Default = null;
    }
}
