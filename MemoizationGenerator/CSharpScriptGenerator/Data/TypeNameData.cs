using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class TypeNameData : ITypeNameData
    {
        public string Name = string.Empty;
        public ITypeNameData NestedType = null;
        public List<ITypeNameData> Parameters = new List<ITypeNameData>();

        public bool IsEmpty => string.IsNullOrEmpty(Name);
    }
}
