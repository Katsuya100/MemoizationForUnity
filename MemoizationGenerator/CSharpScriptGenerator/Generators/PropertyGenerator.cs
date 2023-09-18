using System;
using System.Collections.Generic;
using System.Linq;

namespace Katuusagi.CSharpScriptGenerator
{
    public class PropertyGenerator
    {
        public List<PropertyData> Result { get; private set; } = new List<PropertyData>();

        public void Generate(ModifierType modifier, string type, string name, string defaultValue = "")
        {
            Generate(modifier, name, g =>
            {
                g.Type.Generate(type);
                g.Get.Generate(ModifierType.None, null);
                g.Set.Generate(ModifierType.Private, null);
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
                Param = new ParameterGenerator(),
                Get = new PropertyGetMethodGenerator(),
                Set = new PropertySetMethodGenerator(),
                Default = new StatementGenerator(),
            };
            scope?.Invoke(gen);

            var property = new PropertyData()
            {
                Modifier = modifier,
                Name = name,
                Type = gen.Type.Result.LastOrDefault(),
                Attributes = gen.Attribute.Result,
                Params = gen.Param.Result,
                Get = gen.Get.Result,
                Set = gen.Set.Result,
                Default = gen.Default.Result.LastOrDefault(),
            };

            Result.Add(property);
        }

        public struct Children
        {
            public TypeNameGenerator Type;
            public AttributeGenerator Attribute;
            public ParameterGenerator Param;
            public PropertyGetMethodGenerator Get;
            public PropertySetMethodGenerator Set;
            public StatementGenerator Default;
        }
    }
}
