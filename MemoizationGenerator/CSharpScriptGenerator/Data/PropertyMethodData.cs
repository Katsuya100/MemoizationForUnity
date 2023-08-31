using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class PropertyMethodData
    {
        public ModifierType Modifier = ModifierType.None;
        public string Name = string.Empty;
        public List<AttributeData> Attributes = new List<AttributeData>();
        public CodeData Code = null;
    }
}
