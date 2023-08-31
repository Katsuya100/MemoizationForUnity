using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class FieldData
    {
        public ModifierType Modifier = ModifierType.None;
        public string Type = string.Empty;
        public string Name = string.Empty;
        public CodeData Default = null;
        public List<AttributeData> Attributes = null;
    }
}
