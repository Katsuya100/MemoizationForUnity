using System;
using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class PreProcessEnumGenerator
    {
        public List<PreProcessData> Result { get; private set; } = new List<PreProcessData>();

        public void Generate(ModifierType modifier, string name, Action<Children> scope)
        {
            var gen = new Children()
            {
                PreProcess = new PreProcessEnumGenerator(),
                EnumValue = new EnumValueGenerator(),
            };
            scope?.Invoke(gen);

            var preProcess = new PreProcessData()
            {
                PreProcesses = gen.PreProcess.Result,
                EnumValues = gen.EnumValue.Result,
            };
            Result.Add(preProcess);
        }

        public struct Children
        {
            public PreProcessEnumGenerator PreProcess;
            public EnumValueGenerator EnumValue;
        }
    }
}
