using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class AttributeGenerator
    {
        public List<AttributeData> Result { get; private set; } = new List<AttributeData>();

        public void Generate(string attribute)
        {
            var attr = new AttributeData()
            {
                Attribute = attribute
            };
            Result.Add(attr);
        }
    }
}
