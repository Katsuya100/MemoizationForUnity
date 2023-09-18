using System;
using System.Collections.Generic;
using System.Linq;

namespace Katuusagi.CSharpScriptGenerator
{
    public struct CodeChildren
    {
        public StatementGenerator Statement;
    }

    public struct TypeChildren
    {
        public TypeNameGenerator Type;
        public StatementGenerator Statement;
    }

    public struct VariableChildren
    {
        public TypeNameGenerator Type;
        public StatementGenerator SetValue;
    }

    public struct PropertyChildren
    {
        public StatementGenerator Result;
        public StatementGenerator Arg;
        public StatementGenerator SetValue;
    }

    public struct MethodChildren
    {
        public StatementGenerator Result;
        public StatementGenerator Arg;
        public CallGenericArgGenerator GenericArg;
    }

    public class StatementGenerator
    {
        public List<IStatementData> Result { get; private set; } = new List<IStatementData>();

        public void Generate(string line, Action scope)
        {
            Generate(line);
            Generate(StatementOperation.StartScope);
            scope?.Invoke();
            Generate(StatementOperation.EndScope);

        }

        public void Generate(string line, Action<CodeChildren> scope = null)
        {
            var gen = new CodeChildren()
            {
                Statement = new StatementGenerator(),
            };
            scope?.Invoke(gen);

            var data = new StatementCodeData()
            {
                Line = line,
                Statements = gen.Statement.Result,
            };
            Result.Add(data);
        }

        public void Generate(Action<TypeChildren> scope)
        {
            var gen = new TypeChildren()
            {
                Type = new TypeNameGenerator(),
                Statement = new StatementGenerator(),
            };
            scope?.Invoke(gen);

            var data = new StatementTypeData()
            {
                Type = gen.Type.Result.LastOrDefault(),
                Statement = gen.Statement.Result.LastOrDefault(),
            };
            Result.Add(data);
        }

        public void Generate(string name, Action<VariableChildren> scope)
        {
            var gen = new VariableChildren()
            {
                Type = new TypeNameGenerator(),
                SetValue = new StatementGenerator(),
            };
            scope?.Invoke(gen);

            var data = new StatementVariableData()
            {
                Type = gen.Type.Result.LastOrDefault(),
                Name = name,
                SetValue = gen.SetValue.Result.LastOrDefault(),
            };
            Result.Add(data);
        }

        public void Generate(string name, Action<PropertyChildren> scope)
        {
            var gen = new PropertyChildren()
            {
                Result = new StatementGenerator(),
                Arg = new StatementGenerator(),
                SetValue = new StatementGenerator(),
            };
            scope?.Invoke(gen);

            var data = new StatementPropertyData()
            {
                Result = gen.Result.Result.LastOrDefault(),
                Name = name,
                Args = gen.Arg.Result,
                SetValue = gen.SetValue.Result.LastOrDefault(),
            };
            Result.Add(data);
        }

        public void Generate(string name, Action<MethodChildren> scope)
        {
            var gen = new MethodChildren()
            {
                Result = new StatementGenerator(),
                Arg = new StatementGenerator(),
                GenericArg = new CallGenericArgGenerator(),
            };
            scope?.Invoke(gen);

            var data = new StatementMethodData()
            {
                Result = gen.Result.Result.LastOrDefault(),
                Name = name,
                Args = gen.Arg.Result,
                GenericArgs = gen.GenericArg.Result,
            };
            Result.Add(data);
        }

        private void Generate(StatementOperation op)
        {
            Result.Add(new StatementOperationData() { Operation = op });
        }
    }
}
