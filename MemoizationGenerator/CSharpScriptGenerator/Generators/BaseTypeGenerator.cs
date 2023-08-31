using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class BaseTypeGenerator
    {
        public List<BaseTypeData> Result { get; private set; } = new List<BaseTypeData>();

        public void Generate(string name)
        {
            var baseType = new BaseTypeData()
            {
                Name = name,
            };
            Result.Add(baseType);
        }
    }
}
