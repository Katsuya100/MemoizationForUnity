using System;

namespace Katuusagi.MemoizationForUnity
{
    public class Memoization : Attribute
    {
        public string Modifier { get; set; } = null;
        public string MethodName { get; set; } = null;
        public bool IsClearable { get; set; } = false;
        [Obsolete("Replaced \"ThreadSafeType\".")]
        public bool IsThreadSafe { get; set; } = false;
        public ThreadSafeType ThreadSafeType { get; set; } = ThreadSafeType.None;
        public bool CompareArrayElement { get; set; } = false;
        public string CacheComparer { get; set; } = null;
        public string InterruptCacheMethod { get; set; } = null;
        public string OnCachedMethod { get; set; } = null;
    }
}
