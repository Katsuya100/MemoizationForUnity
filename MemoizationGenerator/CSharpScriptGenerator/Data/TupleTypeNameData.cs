using System.Collections.Generic;
using System.Linq;

namespace Katuusagi.CSharpScriptGenerator
{
    public class TupleTypeNameData : ITypeNameData
    {
        public List<TupleParameterData> Parameters = new List<TupleParameterData>();

        public bool IsEmpty => !Parameters.Any();
    }
}
