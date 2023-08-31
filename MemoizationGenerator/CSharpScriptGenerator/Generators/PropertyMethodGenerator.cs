using System;

namespace Katuusagi.CSharpScriptGenerator
{
    public class PropertyMethodGenerator
    {
        public PropertyMethodData Result { get; private set; } = null;

        public void Generate(ModifierType modifier, Action<Children> scope = null)
        {
            var gen = new Children()
            {
                Attribute = new AttributeGenerator(),
                Code = new CodeGenerator(),
            };
            scope?.Invoke(gen);

            Result = new PropertyMethodData()
            {
                Modifier = modifier,
                Attributes = gen.Attribute.Result,
                Code = gen.Code.Result,
            };
        }

        public struct Children
        {
            public AttributeGenerator Attribute;
            public CodeGenerator Code;
        }
    }
}
