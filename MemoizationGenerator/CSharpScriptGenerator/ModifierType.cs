using System;

namespace Katuusagi.CSharpScriptGenerator
{
    [Flags]
    public enum ModifierType : uint
    {
        None = 0,
        Public          = 1 << 0,
        Private         = 1 << 1,
        Protected       = 1 << 2,
        Internal        = 1 << 3,
        Unsafe          = 1 << 4,
        Sealed          = 1 << 5,
        Static          = 1 << 6,
        Const           = 1 << 7,
        Volatile        = 1 << 8,
        Extern          = 1 << 9,
        Abstract        = 1 << 10,
        Virtual         = 1 << 11,
        Override        = 1 << 12,
        New             = 1 << 13,
        Async           = 1 << 14,
        ReadOnly        = 1 << 15,
        This            = 1 << 16,
        Ref             = 1 << 17,
        In              = 1 << 18,
        Out             = 1 << 19,
        Params          = 1 << 20,
        ReturnReadOnly  = 1 << 21,
        Partial         = 1 << 22,
        Record          = 1 << 23,
        Class           = 1 << 24,
        Struct          = 1 << 25,
        Interface       = 1 << 26,
    }
}
