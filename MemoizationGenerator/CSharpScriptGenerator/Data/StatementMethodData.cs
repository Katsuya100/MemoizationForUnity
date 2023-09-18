using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class StatementMethodData : IStatementData
    {
        public IStatementData Result = null;
        public string Name = string.Empty;
        public List<IStatementData> Args = new List<IStatementData>();
        public List<StatementGenericArgData> GenericArgs = new List<StatementGenericArgData>();
    }
}
