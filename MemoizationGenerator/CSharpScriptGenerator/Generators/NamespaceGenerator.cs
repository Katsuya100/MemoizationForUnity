using System;
using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class NamespaceGenerator
    {
        public List<NamespaceData> Result { get; private set; } = new List<NamespaceData>();

        public void Generate(string name, Action<Children> scope)
        {
            var gen = new Children()
            {
                PreProcess = new PreProcessNameSpaceGenerator(),
                Using = new UsingGenerator(),
                Namespace = new NamespaceGenerator(),
                Delegate = new DelegateGenerator(),
                Enum = new EnumGenerator(),
                Type = new TypeGenerator(),
            };
            scope?.Invoke(gen);

            var namespase = new NamespaceData()
            {
                Name = name,
                PreProcesses = gen.PreProcess.Result,
                Usings = gen.Using.Result,
                Namespaces = gen.Namespace.Result,
                Delegates = gen.Delegate.Result,
                Enums = gen.Enum.Result,
                Types = gen.Type.Result,
            };
            Result.Add(namespase);
        }

        public struct Children
        {
            public PreProcessNameSpaceGenerator PreProcess;
            public UsingGenerator Using;
            public NamespaceGenerator Namespace;
            public DelegateGenerator Delegate;
            public EnumGenerator Enum;
            public TypeGenerator Type;
        }
    }
}
