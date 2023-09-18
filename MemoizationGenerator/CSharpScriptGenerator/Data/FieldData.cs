using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class FieldData
    {
        public ModifierType Modifier = ModifierType.None;
        public ITypeNameData Type = null;
        public string Name = string.Empty;
        public IStatementData Default = null;
        public List<AttributeData> Attributes = null;
    }
}
