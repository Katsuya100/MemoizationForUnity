namespace Katuusagi.CSharpScriptGenerator
{
    public class NamespaceNameData : ITypeNameData
    {
        public string Name = string.Empty;
        public ITypeNameData Child = null;

        public bool IsEmpty => string.IsNullOrEmpty(Name);
    }
}
