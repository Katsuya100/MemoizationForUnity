using System;
using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class EnumGenerator
    {
        public List<EnumData> Result { get; private set; } = new List<EnumData>();

        public void Generate(ModifierType modifier, string name, Action<Children> scope)
        {
            var gen = new Children()
            {
                PreProcess = new PreProcessEnumGenerator(),
                Attribute = new AttributeGenerator(),
                EnumValue = new EnumValueGenerator(),
            };
            scope?.Invoke(gen);

            var enu = new EnumData()
            {
                Modifier = modifier,
                Name = name,
                PreProcesses = gen.PreProcess.Result,
                Attributes = gen.Attribute.Result,
                BaseType = EnumBaseType.None,
                EnumValues = gen.EnumValue.Result,
            };
            Result.Add(enu);
        }

        public void Generate(ModifierType modifier, string name, EnumBaseType enumBaseType, Action<Children> scope)
        {
            var gen = new Children()
            {
                PreProcess = new PreProcessEnumGenerator(),
                Attribute = new AttributeGenerator(),
                EnumValue = new EnumValueGenerator(),
            };
            scope?.Invoke(gen);

            var enu = new EnumData()
            {
                Modifier = modifier,
                Name = name,
                PreProcesses = gen.PreProcess.Result,
                Attributes = gen.Attribute.Result,
                BaseType = enumBaseType,
                EnumValues = gen.EnumValue.Result,
            };
            Result.Add(enu);
        }

        public struct Children
        {
            public PreProcessEnumGenerator PreProcess;
            public AttributeGenerator Attribute;
            public EnumValueGenerator EnumValue;
        }
    }
}
