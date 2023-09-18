using System;
using System.Collections.Generic;
using System.Linq;

namespace Katuusagi.CSharpScriptGenerator
{
    public class ReturnTypeGenerator
    {
        public List<ReturnTypeData> Result { get; private set; } = new List<ReturnTypeData>();

        public void Generate(string type)
        {
            Generate(g =>
            {
                g.Type.Generate(type);
            });
        }

        public void Generate(Action<Children> scope)
        {
            var gen = new Children()
            {
                Type = new TypeNameGenerator(),
                Attribute = new AttributeGenerator(),
            };
            scope?.Invoke(gen);

            var type = new ReturnTypeData()
            {
                Type = gen.Type.Result.LastOrDefault(),
                Attributes = gen.Attribute.Result,
            };
            Result.Add(type);
        }

        public struct Children
        {
            public TypeNameGenerator Type;
            public AttributeGenerator Attribute;
        }
    }
}
