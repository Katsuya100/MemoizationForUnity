using System;
using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class EnumValueGenerator
    {
        public List<EnumValueData> Result { get; private set; } = new List<EnumValueData>();

        public void Generate(string name, string defaultValue)
        {
            Generate(name, dg =>
            {
                if (!string.IsNullOrEmpty(defaultValue))
                {
                    dg.Default.Generate(defaultValue);
                }
            });
        }

        public void Generate(string name, Action<Children> scope)
        {
            var gen = new Children()
            {
                Attribute = new AttributeGenerator(),
                Default = new CodeGenerator(),
            };
            scope?.Invoke(gen);

            var enu = new EnumValueData()
            {
                Name = name,
                Attributes = gen.Attribute.Result,
                Default = gen.Default.Result,
            };
            Result.Add(enu);
        }

        public struct Children
        {
            public AttributeGenerator Attribute;
            public CodeGenerator Default;
        }
    }
}
