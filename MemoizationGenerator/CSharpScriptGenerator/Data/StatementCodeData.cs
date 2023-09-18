using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class StatementCodeData : IStatementData
    {
        public string Line = string.Empty;
        public List<IStatementData> Statements = new List<IStatementData>();
    }
}
