using System;
using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class ParameterGenerator
    {
        public List<ParameterData> Result { get; private set; } = new List<ParameterData>();

        public void Generate(string type, string name, string defaultValue)
        {
            Generate(type, name, dg =>
            {
                if (!string.IsNullOrEmpty(defaultValue))
                {
                    dg.Default.Generate(defaultValue);
                }
            });
        }

        public void Generate(string type, string name, Action<Children> scope = null)
        {
            var gen = new Children()
            {
                Attribute = new AttributeGenerator(),
                Default = new CodeGenerator(),
            };
            scope?.Invoke(gen);

            var parameter = new ParameterData()
            {
                Type = type,
                Name = name,
                Attributes = gen.Attribute.Result,
                Default = gen.Default.Result,
            };
            Result.Add(parameter);
        }

        public struct Children
        {
            public AttributeGenerator Attribute;
            public CodeGenerator Default;
        }
    }
}
