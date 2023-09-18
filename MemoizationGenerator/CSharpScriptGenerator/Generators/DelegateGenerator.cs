using System;
using System.Collections.Generic;
using System.Linq;

namespace Katuusagi.CSharpScriptGenerator
{
    public class DelegateGenerator
    {
        public List<DelegateData> Result { get; private set; } = new List<DelegateData>();

        public void Generate(ModifierType modifier, string type, string name, Action<Children> scope)
        {
            Generate(modifier, name, g =>
            {
                g.ReturnType.Generate(type);
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
            };
            scope?.Invoke(gen);

            var method = new DelegateData()
            {
                Modifier = modifier,
                ReturnType = gen.ReturnType.Result.LastOrDefault(),
                Name = name,
                Attributes = gen.Attribute.Result,
                GenericParams = gen.GenericParam.Result,
                Params = gen.Param.Result,
            };
            Result.Add(method);
        }

        public struct Children
        {
            public ReturnTypeGenerator ReturnType;
            public AttributeGenerator Attribute;
            public GenericParameterGenerator GenericParam;
            public ParameterGenerator Param;
        }
    }
}
