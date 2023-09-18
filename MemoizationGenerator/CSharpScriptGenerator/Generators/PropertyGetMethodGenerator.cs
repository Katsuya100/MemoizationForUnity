using System;
using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class PropertyGetMethodGenerator
    {
        public PropertyMethodData Result { get; private set; } = null;

        public void Generate(ModifierType modifier, Action<Children> scope = null)
        {
            var gen = new Children()
            {
                Attribute = new AttributeGenerator(),
                ReturnAttribute = new AttributeGenerator(),
                Statement = new StatementGenerator(),
            };
            scope?.Invoke(gen);

            Result = new PropertyMethodData()
            {
                Modifier = modifier,
                Attributes = gen.Attribute.Result,
                ParamAttributes = new List<AttributeData>(),
                ReturnAttributes = gen.ReturnAttribute.Result,
                Statements = gen.Statement.Result,
            };
        }

        public struct Children
        {
            public AttributeGenerator Attribute;
            public AttributeGenerator ReturnAttribute;
            public StatementGenerator Statement;
        }
    }
}
