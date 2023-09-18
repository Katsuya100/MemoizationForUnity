using System;
using System.Collections.Generic;
using System.Linq;

namespace Katuusagi.CSharpScriptGenerator
{
    public class FieldGenerator
    {
        public List<FieldData> Result { get; private set; } = new List<FieldData>();

        public void Generate(ModifierType modifier, string type, string name, string defaultValue = "")
        {
            Generate(modifier, name, g =>
            {
                g.Type.Generate(type);
                if (!string.IsNullOrEmpty(defaultValue))
                {
                    g.Default.Generate(defaultValue);
                }
            });
        }

        public void Generate(ModifierType modifier, string type, string name, Action<Children> scope)
        {
            Generate(modifier, name, g =>
            {
                g.Type.Generate(type);
                scope?.Invoke(g);
            });
        }

        public void Generate(ModifierType modifier, string name, Action<Children> scope)
        {
            var gen = new Children()
            {
                Type = new TypeNameGenerator(),
                Attribute = new AttributeGenerator(),
                Default = new StatementGenerator(),
            };
            scope?.Invoke(gen);

            var field = new FieldData()
            {
                Modifier = modifier,
                Type = gen.Type.Result.LastOrDefault(),
                Name = name,
                Attributes = gen.Attribute.Result,
                Default = gen.Default.Result.LastOrDefault(),
            };
            Result.Add(field);
        }

        public struct Children
        {
            public TypeNameGenerator Type;
            public AttributeGenerator Attribute;
            public StatementGenerator Default;
        }
    }
}
