using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class MethodData
    {
        public ModifierType Modifier = ModifierType.None;
        public string Type = string.Empty;
        public string Name = string.Empty;
        public List<AttributeData> Attributes = new List<AttributeData>();
        public List<GenericParameterData> GenericParams = new List<GenericParameterData>();
        public List<ParameterData> Params = new List<ParameterData>();
        public CodeData Code = null;
    }
}
