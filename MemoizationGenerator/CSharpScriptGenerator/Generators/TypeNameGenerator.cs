using System;
using System.Collections.Generic;
using System.Linq;

namespace Katuusagi.CSharpScriptGenerator
{
    public struct NameSpaceNameChildren
    {
        public TypeNameGenerator Child;
    }

    public struct TypeNameChildren
    {
        public TypeNameGenerator NestedType;
        public TypeNameGenerator Parameter;
    }

    public struct TupleTypeNameChildren
    {
        public TupleParameterGenerator Parameter;
    }

    public class TypeNameGenerator
    {
        public List<ITypeNameData> Result { get; private set; } = new List<ITypeNameData>();

        public void Generate(string name, Action<NameSpaceNameChildren> scope)
        {
            var gen = new NameSpaceNameChildren()
            {
                Child = new TypeNameGenerator(),
            };
            scope?.Invoke(gen);

            var data = new NamespaceNameData()
            {
                Name = name,
                Child = gen.Child.Result.LastOrDefault(),
            };
            Result.Add(data);
        }

        public void Generate(string name, Action<TypeNameChildren> scope = null)
        {
            var gen = new TypeNameChildren()
            {
                NestedType = new TypeNameGenerator(),
                Parameter = new TypeNameGenerator(),
            };
            scope?.Invoke(gen);

            var data = new TypeNameData()
            {
                Name = name,
                NestedType = gen.NestedType.Result.LastOrDefault(),
                Parameters = gen.Parameter.Result,
            };
            Result.Add(data);
        }

        public void Generate(Action<TupleTypeNameChildren> scope)
        {
            var gen = new TupleTypeNameChildren()
            {
                Parameter = new TupleParameterGenerator(),
            };
            scope?.Invoke(gen);

            var data = new TupleTypeNameData()
            {
                Parameters = gen.Parameter.Result,
            };
            Result.Add(data);
        }
    }
}
