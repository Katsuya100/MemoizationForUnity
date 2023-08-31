using System;
using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class MethodGenerator
    {
        public List<MethodData> Result { get; private set; } = new List<MethodData>();

        public void Generate(ModifierType modifier, string type, string name, Action<Children> scope)
        {
            var gen = new Children()
            {
                Attribute = new AttributeGenerator(),
                GenericParam = new GenericParameterGenerator(),
                Param = new ParameterGenerator(),
                Code = new CodeGenerator(),
            };
            scope?.Invoke(gen);

            var method = new MethodData()
            {
                Modifier = modifier,
                Type = type,
                Name = name,
                Attributes = gen.Attribute.Result,
                GenericParams = gen.GenericParam.Result,
                Params = gen.Param.Result,
                Code = gen.Code.Result,
            };
            Result.Add(method);
        }

        public struct Children
        {
            public AttributeGenerator Attribute;
            public GenericParameterGenerator GenericParam;
            public ParameterGenerator Param;
            public CodeGenerator Code;
        }
    }
}
