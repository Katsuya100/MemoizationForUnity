using System;

namespace Katuusagi.CSharpScriptGenerator
{
    [Flags]
    public enum ModifierType : uint
    {
        None = 0,
        Private     = 1 << 0,
        Protected   = 1 << 1,
        Public      = 1 << 2,
        Internal    = 1 << 3,
        Partial     = 1 << 4,
        Static      = 1 << 5,
        Sealed      = 1 << 6,
        Virtual     = 1 << 7,
        Abstract    = 1 << 8,
        Override    = 1 << 9,
        Class       = 1 << 10,
        Struct      = 1 << 11,
        Record      = 1 << 12,
        Interface   = 1 << 13,
        Const       = 1 << 14,
        ReadOnly    = 1 << 15,
        Ref         = 1 << 16,
        Unsafe      = 1 << 17,
        Async       = 1 << 18,
    }
}
