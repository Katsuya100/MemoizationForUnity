using System;
using System.Collections.Generic;

namespace Katuusagi.MemoizationForUnity
{
    public sealed class MemoizationEqualityComparer<T1> : IEqualityComparer<ValueTuple<T1>>
    {
        public static readonly MemoizationEqualityComparer<T1> Default= new ();
        bool IEqualityComparer<ValueTuple<T1>>.Equals(ValueTuple<T1> x, ValueTuple<T1> y)
        {
            return x.Equals(y);
        }

        int IEqualityComparer<ValueTuple<T1>>.GetHashCode(ValueTuple<T1> obj)
        {
            return HashCode.Combine(obj.Item1);
        }
    }

    public sealed class MemoizationEqualityComparer<T1, T2> : IEqualityComparer<(T1, T2)>
    {
        public static readonly MemoizationEqualityComparer<T1, T2> Default= new ();
        bool IEqualityComparer<(T1, T2)>.Equals((T1, T2) x, (T1, T2) y)
        {
            return x.Equals(y);
        }

        int IEqualityComparer<(T1, T2)>.GetHashCode((T1, T2) obj)
        {
            return HashCode.Combine(obj.Item1, obj.Item2);
        }
    }

    public sealed class MemoizationEqualityComparer<T1, T2, T3> : IEqualityComparer<(T1, T2, T3)>
    {
        public static readonly MemoizationEqualityComparer<T1, T2, T3> Default= new ();
        bool IEqualityComparer<(T1, T2, T3)>.Equals((T1, T2, T3) x, (T1, T2, T3) y)
        {
            return x.Equals(y);
        }

        int IEqualityComparer<(T1, T2, T3)>.GetHashCode((T1, T2, T3) obj)
        {
            return HashCode.Combine(obj.Item1, obj.Item2, obj.Item3);
        }
    }

    public sealed class MemoizationEqualityComparer<T1, T2, T3, T4> : IEqualityComparer<(T1, T2, T3, T4)>
    {
        public static readonly MemoizationEqualityComparer<T1, T2, T3, T4> Default= new ();
        bool IEqualityComparer<(T1, T2, T3, T4)>.Equals((T1, T2, T3, T4) x, (T1, T2, T3, T4) y)
        {
            return x.Equals(y);
        }

        int IEqualityComparer<(T1, T2, T3, T4)>.GetHashCode((T1, T2, T3, T4) obj)
        {
            return HashCode.Combine(obj.Item1, obj.Item2, obj.Item3, obj.Item4);
        }
    }

    public sealed class MemoizationEqualityComparer<T1, T2, T3, T4, T5> : IEqualityComparer<(T1, T2, T3, T4, T5)>
    {
        public static readonly MemoizationEqualityComparer<T1, T2, T3, T4, T5> Default= new ();
        bool IEqualityComparer<(T1, T2, T3, T4, T5)>.Equals((T1, T2, T3, T4, T5) x, (T1, T2, T3, T4, T5) y)
        {
            return x.Equals(y);
        }

        int IEqualityComparer<(T1, T2, T3, T4, T5)>.GetHashCode((T1, T2, T3, T4, T5) obj)
        {
            return HashCode.Combine(obj.Item1, obj.Item2, obj.Item3, obj.Item4, obj.Item5);
        }
    }

    public sealed class MemoizationEqualityComparer<T1, T2, T3, T4, T5, T6> : IEqualityComparer<(T1, T2, T3, T4, T5, T6)>
    {
        public static readonly MemoizationEqualityComparer<T1, T2, T3, T4, T5, T6> Default= new ();
        bool IEqualityComparer<(T1, T2, T3, T4, T5, T6)>.Equals((T1, T2, T3, T4, T5, T6) x, (T1, T2, T3, T4, T5, T6) y)
        {
            return x.Equals(y);
        }

        int IEqualityComparer<(T1, T2, T3, T4, T5, T6)>.GetHashCode((T1, T2, T3, T4, T5, T6) obj)
        {
            return HashCode.Combine(obj.Item1, obj.Item2, obj.Item3, obj.Item4, obj.Item5, obj.Item6);
        }
    }

    public sealed class MemoizationEqualityComparer<T1, T2, T3, T4, T5, T6, T7> : IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7)>
    {
        public static readonly MemoizationEqualityComparer<T1, T2, T3, T4, T5, T6, T7> Default= new ();
        bool IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7)>.Equals((T1, T2, T3, T4, T5, T6, T7) x, (T1, T2, T3, T4, T5, T6, T7) y)
        {
            return x.Equals(y);
        }

        int IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7)>.GetHashCode((T1, T2, T3, T4, T5, T6, T7) obj)
        {
            return HashCode.Combine(obj.Item1, obj.Item2, obj.Item3, obj.Item4, obj.Item5, obj.Item6, obj.Item7);
        }
    }

    public sealed class MemoizationEqualityComparer<T1, T2, T3, T4, T5, T6, T7, T8> : IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7, T8)>
    {
        public static readonly MemoizationEqualityComparer<T1, T2, T3, T4, T5, T6, T7, T8> Default= new ();
        bool IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7, T8)>.Equals((T1, T2, T3, T4, T5, T6, T7, T8) x, (T1, T2, T3, T4, T5, T6, T7, T8) y)
        {
            return x.Equals(y);
        }

        int IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7, T8)>.GetHashCode((T1, T2, T3, T4, T5, T6, T7, T8) obj)
        {
            return HashCode.Combine(obj.Item1, obj.Item2, obj.Item3, obj.Item4, obj.Item5, obj.Item6, obj.Item7, obj.Item8);
        }
    }
}
