using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class StatementPropertyData : IStatementData
    {
        public IStatementData Result = null;
        public string Name = string.Empty;
        public List<IStatementData> Args = new List<IStatementData>();
        public IStatementData SetValue = null;
    }
}
