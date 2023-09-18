using System;
using System.Collections.Generic;
using System.Linq;

namespace Katuusagi.CSharpScriptGenerator
{
    public class ParameterGenerator
    {
        public List<ParameterData> Result { get; private set; } = new List<ParameterData>();

        public void Generate(string type, string name, string defaultValue)
        {
            Generate(ModifierType.None, type, name, defaultValue);
        }

        public void Generate(string type, string name, Action<Children> scope = null)
        {
            Generate(ModifierType.None, type, name, scope);
        }

        public void Generate( string name, Action<Children> scope = null)
        {
            Generate(ModifierType.None, name, scope);
        }

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
                Default = new StatementGenerator(),
            };
            scope?.Invoke(gen);

            var parameter = new ParameterData()
            {
                Modifier = modifier,
                Type = gen.Type.Result.LastOrDefault(),
                Name = name,
                Attributes = gen.Attribute.Result,
                Default = gen.Default.Result.LastOrDefault(),
            };
            Result.Add(parameter);
        }

        public struct Children
        {
            public TypeNameGenerator Type;
            public AttributeGenerator Attribute;
            public StatementGenerator Default;
        }
    }
}
