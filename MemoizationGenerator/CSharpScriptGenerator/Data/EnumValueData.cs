using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class EnumValueData
    {
        public string Name = string.Empty;
        public CodeData Default = null;
        public List<AttributeData> Attributes = null;
    }
}
