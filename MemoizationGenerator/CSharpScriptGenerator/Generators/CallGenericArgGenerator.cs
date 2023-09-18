using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class CallGenericArgGenerator
    {
        public List<StatementGenericArgData> Result { get; private set; } = new List<StatementGenericArgData>();

        public void Generate(string arg)
        {
            Result.Add(new StatementGenericArgData()
            {
                Arg = arg,
            });
        }
    }
}
