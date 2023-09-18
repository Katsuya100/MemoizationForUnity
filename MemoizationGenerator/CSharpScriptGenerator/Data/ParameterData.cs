using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class ParameterData
    {
        public ModifierType Modifier = ModifierType.None;
        public ITypeNameData Type = null;
        public string Name = string.Empty;
        public List<AttributeData> Attributes = new List<AttributeData>();
        public IStatementData Default = null;
    }
}
