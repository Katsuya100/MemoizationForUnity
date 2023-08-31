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
                BaseType = new BaseTypeGenerator(),
                EnumValue = new EnumValueGenerator(),
            };
            scope?.Invoke(gen);

            var enu = new EnumData()
            {
                Modifier = modifier,
                Name = name,
                PreProcesses = gen.PreProcess.Result,
                Attributes = gen.Attribute.Result,
                BaseTypes = gen.BaseType.Result,
                EnumValues = gen.EnumValue.Result,
            };
            Result.Add(enu);
        }

        public struct Children
        {
            public PreProcessEnumGenerator PreProcess;
            public AttributeGenerator Attribute;
            public BaseTypeGenerator BaseType;
            public EnumValueGenerator EnumValue;
        }
    }
}
