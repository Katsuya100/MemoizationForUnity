using System;
using System.Collections.Generic;
using System.Linq;

namespace Katuusagi.CSharpScriptGenerator
{
    public class MethodGenerator
    {
        public List<MethodData> Result { get; private set; } = new List<MethodData>();

        public void Generate(ModifierType modifier, string returnType, string name, Action<Children> scope)
        {
            Generate(modifier, name, g =>
            {
                g.ReturnType.Generate(returnType);
                scope?.Invoke(g);
            });
        }

        public void Generate(ModifierType modifier, string name, Action<Children> scope)
        {
            var gen = new Children()
            {
                ReturnType = new ReturnTypeGenerator(),
                Attribute = new AttributeGenerator(),
                GenericParam = new GenericParameterGenerator(),
                Param = new ParameterGenerator(),
                Statement = new StatementGenerator(),
            };
            scope?.Invoke(gen);

            var method = new MethodData()
            {
                Modifier = modifier,
                ReturnType = gen.ReturnType.Result.LastOrDefault(),
                Name = name,
                Attributes = gen.Attribute.Result,
                GenericParams = gen.GenericParam.Result,
                Params = gen.Param.Result,
                Statements = gen.Statement.Result,
            };
            Result.Add(method);
        }

        public struct Children
        {
            public ReturnTypeGenerator ReturnType;
            public AttributeGenerator Attribute;
            public GenericParameterGenerator GenericParam;
            public ParameterGenerator Param;
            public StatementGenerator Statement;
        }
    }
}
