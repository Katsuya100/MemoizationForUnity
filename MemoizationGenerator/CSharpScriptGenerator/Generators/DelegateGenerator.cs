using System;
using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class DelegateGenerator
    {
        public List<DelegateData> Result { get; private set; } = new List<DelegateData>();

        public void Generate(ModifierType modifier, string type, string name, Action<Children> scope)
        {
            var gen = new Children()
            {
                Attribute = new AttributeGenerator(),
                GenericParam = new GenericParameterGenerator(),
                Param = new ParameterGenerator(),
            };
            scope?.Invoke(gen);

            var method = new DelegateData()
            {
                Modifier = modifier,
                Type = type,
                Name = name,
                Attributes = gen.Attribute.Result,
                GenericParams = gen.GenericParam.Result,
                Params = gen.Param.Result,
            };
            Result.Add(method);
        }

        public struct Children
        {
            public AttributeGenerator Attribute;
            public GenericParameterGenerator GenericParam;
            public ParameterGenerator Param;
        }
    }
}
