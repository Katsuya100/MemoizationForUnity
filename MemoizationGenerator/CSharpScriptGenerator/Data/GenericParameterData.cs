using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class GenericParameterData
    {
        public string Name = string.Empty;
        public List<AttributeData> Attributes = new List<AttributeData>();
        public List<WhereData> Wheres = new List<WhereData>();
    }
}
