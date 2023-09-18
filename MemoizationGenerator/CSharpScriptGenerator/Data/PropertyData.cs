using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class PropertyData
    {
        public ModifierType Modifier = ModifierType.None;
        public string Name = string.Empty;
        public ITypeNameData Type = null;
        public List<AttributeData> Attributes = new List<AttributeData>();
        public List<ParameterData> Params = new List<ParameterData>();
        public PropertyMethodData Get = null;
        public PropertyMethodData Set = null;
        public IStatementData Default = null;
    }
}
