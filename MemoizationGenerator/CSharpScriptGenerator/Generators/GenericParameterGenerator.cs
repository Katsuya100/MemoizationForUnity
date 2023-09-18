using System;
using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class GenericParameterGenerator
    {
        public List<GenericParameterData> Result { get; private set; } = new List<GenericParameterData>();

        public void Generate(string name, Action<Children> scope = null)
        {
            Generate(ModifierType.None, name, scope);
        }

        public void Generate(ModifierType modifier, string name, Action<Children> scope = null)
        {
            var gen = new Children()
            {
                Where = new WhereGenerator(),
                Attribute = new AttributeGenerator(),
            };
            scope?.Invoke(gen);

            var parameter = new GenericParameterData()
            {
                Modifier = modifier,
                Name = name,
                Wheres = gen.Where.Result,
                Attributes = gen.Attribute.Result,
            };
            Result.Add(parameter);
        }

        public struct Children
        {
            public AttributeGenerator Attribute;
            public WhereGenerator Where;
        }
    }
}
