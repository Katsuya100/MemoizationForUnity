using System;
using System.Collections.Generic;
using System.Linq;

namespace Katuusagi.CSharpScriptGenerator
{
    public class TupleParameterGenerator
    {
        public List<TupleParameterData> Result { get; private set; } = new List<TupleParameterData>();

        public void Generate(string type, string name = null)
        {
            Generate(name, g =>
            {
                g.Type.Generate(type);
            });
        }

        public void Generate(Action<Children> scope)
        {
            Generate(null, scope);
        }

        public void Generate(string name, Action<Children> scope)
        {
            var gen = new Children()
            {
                Type = new TypeNameGenerator(),
            };
            scope?.Invoke(gen);

            var data = new TupleParameterData()
            {
                Type = gen.Type.Result.LastOrDefault(),
                Name = name,
            };
            Result.Add(data);
        }

        public struct Children
        {
            public TypeNameGenerator Type;
        }
    }
}
