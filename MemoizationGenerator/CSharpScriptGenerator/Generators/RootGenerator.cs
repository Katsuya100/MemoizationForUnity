using System;

namespace Katuusagi.CSharpScriptGenerator
{
    public class RootGenerator
    {
        public RootData Result { get; private set; } = new RootData();

        public void Generate(Action<Children> scope)
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

            Result = new RootData()
            {
                PreProcesses = gen.PreProcess.Result,
                Usings = gen.Using.Result,
                Namespaces = gen.Namespace.Result,
                Delegates = gen.Delegate.Result,
                Enums = gen.Enum.Result,
                Types = gen.Type.Result,
            };
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
