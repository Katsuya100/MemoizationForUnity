using System.Collections.Generic;

namespace Katuusagi.SourceGeneratorCommon
{
    public static class ParameterUtils
    {
        public static string JoinParameters(this IEnumerable<string> self)
        {
            return string.Join(", ", self);
        }
    }
}
