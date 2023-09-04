using System.Threading;

namespace Katuusagi.MemoizationForUnity.Utils
{
    public static class MemoizationUtils
    {
        public class TypeId<T0>
        {
            public static readonly int Id = Interlocked.Increment(ref _counter);
        }

        public class TypeId<T0, T1>
        {
            public static readonly int Id = Interlocked.Increment(ref _counter);
        }

        public class TypeId<T0, T1, T2>
        {
            public static readonly int Id = Interlocked.Increment(ref _counter);
        }

        public class TypeId<T0, T1, T2, T3>
        {
            public static readonly int Id = Interlocked.Increment(ref _counter);
        }

        public class TypeId<T0, T1, T2, T3, T4>
        {
            public static readonly int Id = Interlocked.Increment(ref _counter);
        }

        public class TypeId<T0, T1, T2, T3, T4, T5>
        {
            public static readonly int Id = Interlocked.Increment(ref _counter);
        }

        public class TypeId<T0, T1, T2, T3, T4, T5, T6>
        {
            public static readonly int Id = Interlocked.Increment(ref _counter);
        }

        public class TypeId<T0, T1, T2, T3, T4, T5, T6, T7>
        {
            public static readonly int Id = Interlocked.Increment(ref _counter);
        }

        private static int _counter = int.MinValue;
    }
}
