using System;
using System.Collections.Generic;
using System.Linq;

namespace Katuusagi.CSharpScriptGenerator
{
    public class EventGenerator
    {
        public List<EventData> Result { get; private set; } = new List<EventData>();

        public void Generate(ModifierType modifier, string type, string name, string defaultValue)
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

        public void Generate(ModifierType modifier, string type, string name, Action<Children> scope = null)
        {
            Generate(modifier, name, g =>
            {
                g.Type.Generate(type);
                scope?.Invoke(g);
            });
        }

        public void Generate(ModifierType modifier, string name, Action<Children> scope = null)
        {
            var gen = new Children()
            {
                Type = new TypeNameGenerator(),
                Attribute = new AttributeGenerator(),
                Add = new PropertySetMethodGenerator(),
                Remove = new PropertySetMethodGenerator(),
                Default = new StatementGenerator(),
            };
            scope?.Invoke(gen);

            var data = new EventData()
            {
                Modifier = modifier,
                Type = gen.Type.Result.LastOrDefault(),
                Name = name,
                Add = gen.Add.Result,
                Remove = gen.Remove.Result,
                Attributes = gen.Attribute.Result,
                Default = gen.Default.Result.LastOrDefault(),
            };

            Result.Add(data);
        }

        public struct Children
        {
            public TypeNameGenerator Type;
            public AttributeGenerator Attribute;
            public PropertySetMethodGenerator Add;
            public PropertySetMethodGenerator Remove;
            public StatementGenerator Default;
        }
    }
}
