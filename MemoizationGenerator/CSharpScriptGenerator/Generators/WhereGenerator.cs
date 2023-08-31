using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class WhereGenerator
    {
        public List<WhereData> Result { get; private set; } = new List<WhereData>();

        public void Generate(string where)
        {
            var whereData = new WhereData()
            {
                Where = where,
            };
            Result.Add(whereData);
        }
    }
}
