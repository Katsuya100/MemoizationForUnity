using System;
using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class PropertyGenerator
    {
        public List<PropertyData> Result { get; private set; } = new List<PropertyData>();

        public void Generate(ModifierType modifier, string type, string name, string defaultValue = "")
        {
            Generate(modifier, type, name, pg =>
            {
                pg.Get.Generate(ModifierType.None, null);
                pg.Set.Generate(ModifierType.Private, null);
                if (!string.IsNullOrEmpty(defaultValue))
                {
                    pg.Default.Generate(defaultValue);
                }
            });
        }

        public void Generate(ModifierType modifier, string type, string name, Action<Children> paramScope)
        {
            var gen = new Children()
            {
                Attribute = new AttributeGenerator(),
                Param = new ParameterGenerator(),
                Get = new PropertyMethodGenerator(),
                Set = new PropertyMethodGenerator(),
                Default = new CodeGenerator(),
            };
            paramScope?.Invoke(gen);

            var property = new PropertyData()
            {
                Modifier = modifier,
                Name = name,
                Type = type,
                Attributes = gen.Attribute.Result,
                Params = gen.Param.Result,
                Default = gen.Default.Result,
            };

            property.Get = gen.Get.Result;
            if (property.Get != null)
            {
                property.Get.Name = "get";
            }

            property.Set = gen.Set.Result;
            if (property.Set != null)
            {
                property.Set.Name = "set";
            }

            Result.Add(property);
        }

        public struct Children
        {
            public AttributeGenerator Attribute;
            public ParameterGenerator Param;
            public PropertyMethodGenerator Get;
            public PropertyMethodGenerator Set;
            public CodeGenerator Default;
        }
    }
}
