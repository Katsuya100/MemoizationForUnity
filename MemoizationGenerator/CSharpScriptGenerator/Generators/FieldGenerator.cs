using System;
using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class FieldGenerator
    {
        public List<FieldData> Result { get; private set; } = new List<FieldData>();

        public void Generate(ModifierType modifier, string type, string name, string defaultValue)
        {
            Generate(modifier, type, name, dg =>
            {
                if (!string.IsNullOrEmpty(defaultValue))
                {
                    dg.Default.Generate(defaultValue);
                }
            });
        }

        public void Generate(ModifierType modifier, string type, string name, Action<Children> scope = null)
        {
            var gen = new Children()
            {
                Attribute = new AttributeGenerator(),
                Default = new CodeGenerator(),
            };
            scope?.Invoke(gen);

            var field = new FieldData()
            {
                Modifier = modifier,
                Type = type,
                Name = name,
                Attributes = gen.Attribute.Result,
                Default = gen.Default.Result,
            };
            Result.Add(field);
        }

        public struct Children
        {
            public AttributeGenerator Attribute;
            public CodeGenerator Default;
        }
    }
}
