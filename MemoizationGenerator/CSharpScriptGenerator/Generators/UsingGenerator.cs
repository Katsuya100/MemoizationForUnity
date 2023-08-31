using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class UsingGenerator
    {
        public List<UsingData> Result { get; private set; } = new List<UsingData>();

        public void Generate(string namespase)
        {
            var uzing = new UsingData()
            {
                NameSpace = namespase
            };
            Result.Add(uzing);
        }
    }
}
