using System;
using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class EventGenerator
    {
        public List<EventData> Result { get; private set; } = new List<EventData>();

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

            var eve = new EventData()
            {
                Modifier = modifier,
                Type = type,
                Name = name,
                Attributes = gen.Attribute.Result,
                Default = gen.Default.Result,
            };
            Result.Add(eve);
        }

        public struct Children
        {
            public AttributeGenerator Attribute;
            public CodeGenerator Default;
        }
    }
}
