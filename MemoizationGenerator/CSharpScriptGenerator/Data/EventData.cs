using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class EventData
    {
        public ModifierType Modifier = ModifierType.None;
        public ITypeNameData Type = null;
        public string Name = string.Empty;
        public PropertyMethodData Add = null;
        public PropertyMethodData Remove = null;
        public IStatementData Default = null;
        public List<AttributeData> Attributes = null;
    }
}
