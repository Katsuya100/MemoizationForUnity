using System;
using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class PropertySetMethodGenerator
    {
        public PropertyMethodData Result { get; private set; } = null;

        public void Generate(ModifierType modifier, Action<Children> scope = null)
        {
            var gen = new Children()
            {
                Attribute = new AttributeGenerator(),
                ParamAttribute = new AttributeGenerator(),
                Statement = new StatementGenerator(),
            };
            scope?.Invoke(gen);

            Result = new PropertyMethodData()
            {
                Modifier = modifier,
                Attributes = gen.Attribute.Result,
                ParamAttributes = gen.ParamAttribute.Result,
                ReturnAttributes = new List<AttributeData>(),
                Statements = gen.Statement.Result,
            };
        }

        public struct Children
        {
            public AttributeGenerator Attribute;
            public AttributeGenerator ParamAttribute;
            public StatementGenerator Statement;
        }
    }
}
