using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class DelegateData
    {
        public ModifierType Modifier = ModifierType.None;
        public ReturnTypeData ReturnType = null;
        public string Name = string.Empty;
        public List<AttributeData> Attributes = new List<AttributeData>();
        public List<GenericParameterData> GenericParams = new List<GenericParameterData>();
        public List<ParameterData> Params = new List<ParameterData>();
    }
}
