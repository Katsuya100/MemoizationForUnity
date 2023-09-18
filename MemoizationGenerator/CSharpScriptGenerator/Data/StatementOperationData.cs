namespace Katuusagi.CSharpScriptGenerator
{
    public enum StatementOperation
    {
        Nop,
        StartScope,
        EndScope,
    }

    public class StatementOperationData : IStatementData
    {
        public StatementOperation Operation = StatementOperation.Nop;
    }
}
