using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class ParameterData
    {
        public string Type = string.Empty;
        public string Name = string.Empty;
        public List<AttributeData> Attributes = new List<AttributeData>();
        public CodeData Default = null;
    }
}
