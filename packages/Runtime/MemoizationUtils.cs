using System.Threading;

namespace Katuusagi.MemoizationForUnity.Utils
{
    public class MemoizationUtils
    {
        public static class TypeId<T>
        {
            public static readonly int Id = Interlocked.Increment(ref _counter);
        }

        private static int _counter = int.MinValue;
    }
}
