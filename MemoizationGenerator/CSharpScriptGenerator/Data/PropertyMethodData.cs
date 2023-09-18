using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class PropertyMethodData
    {
        public ModifierType Modifier = ModifierType.None;
        public List<AttributeData> Attributes = new List<AttributeData>();
        public List<AttributeData> ParamAttributes = new List<AttributeData>();
        public List<AttributeData> ReturnAttributes = new List<AttributeData>();
        public List<IStatementData> Statements = null;
    }
}
