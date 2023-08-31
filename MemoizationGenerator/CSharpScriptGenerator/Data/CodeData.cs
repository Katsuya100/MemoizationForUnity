using System.Collections.Generic;

namespace Katuusagi.CSharpScriptGenerator
{
    public class CodeData
    {
        public List<string> Lines = new List<string>();
        public bool IsEmpty => Lines.Count <= 0;
    }
}
