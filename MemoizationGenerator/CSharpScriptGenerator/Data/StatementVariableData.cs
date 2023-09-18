namespace Katuusagi.CSharpScriptGenerator
{
    public class StatementVariableData : IStatementData
    {
        public string Name = string.Empty;
        public ITypeNameData Type = null;
        public IStatementData SetValue = null;
    }
}
