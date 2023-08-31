using System;
using System.Collections.Generic;
using System.Linq;

namespace Katuusagi.MemoizationForUnity
{
    public sealed class ParamsEqualityComparer<T1> : IEqualityComparer<ValueTuple<T1[]>>
    {
        public static ParamsEqualityComparer<T1> Default = new ();
        bool IEqualityComparer<ValueTuple<T1[]>>.Equals(ValueTuple<T1[]> x, ValueTuple<T1[]> y)
        {
            return (x.Item1 ?? Array.Empty<T1>()).SequenceEqual(y.Item1 ?? Array.Empty<T1>());
        }

        int IEqualityComparer<ValueTuple<T1[]>>.GetHashCode(ValueTuple<T1[]> obj)
        {
            var hash = new HashCode();

            foreach (var elem in obj.Item1 ?? Array.Empty<T1>())
            {
                hash.Add(elem);
            }

            return hash.ToHashCode();
        }
    }

    public sealed class ParamsEqualityComparer<T1, T2> : IEqualityComparer<(T1, T2[])>
    {
        public static ParamsEqualityComparer<T1, T2> Default = new ();
        bool IEqualityComparer<(T1, T2[])>.Equals((T1, T2[]) x, (T1, T2[]) y)
        {
            return EqualityComparer<T1>.Default.Equals(x.Item1, y.Item1) &&
                   (x.Item2 ?? Array.Empty<T2>()).SequenceEqual(y.Item2 ?? Array.Empty<T2>());
        }

        int IEqualityComparer<(T1, T2[])>.GetHashCode((T1, T2[]) obj)
        {
            var hash = new HashCode();

            hash.Add(obj.Item1);
            foreach (var elem in obj.Item2 ?? Array.Empty<T2>())
            {
                hash.Add(elem);
            }

            return hash.ToHashCode();
        }
    }

    public sealed class ParamsEqualityComparer<T1, T2, T3> : IEqualityComparer<(T1, T2, T3[])>
    {
        public static ParamsEqualityComparer<T1, T2, T3> Default = new ();
        bool IEqualityComparer<(T1, T2, T3[])>.Equals((T1, T2, T3[]) x, (T1, T2, T3[]) y)
        {
            return EqualityComparer<T1>.Default.Equals(x.Item1, y.Item1) &&
                   EqualityComparer<T2>.Default.Equals(x.Item2, y.Item2) &&
                   (x.Item3 ?? Array.Empty<T3>()).SequenceEqual(y.Item3 ?? Array.Empty<T3>());
        }

        int IEqualityComparer<(T1, T2, T3[])>.GetHashCode((T1, T2, T3[]) obj)
        {
            var hash = new HashCode();

            hash.Add(obj.Item1);
            hash.Add(obj.Item2);
            foreach (var elem in obj.Item3 ?? Array.Empty<T3>())
            {
                hash.Add(elem);
            }

            return hash.ToHashCode();
        }
    }

    public sealed class ParamsEqualityComparer<T1, T2, T3, T4> : IEqualityComparer<(T1, T2, T3, T4[])>
    {
        public static ParamsEqualityComparer<T1, T2, T3, T4> Default = new ();
        bool IEqualityComparer<(T1, T2, T3, T4[])>.Equals((T1, T2, T3, T4[]) x, (T1, T2, T3, T4[]) y)
        {
            return EqualityComparer<T1>.Default.Equals(x.Item1, y.Item1) &&
                   EqualityComparer<T2>.Default.Equals(x.Item2, y.Item2) &&
                   EqualityComparer<T3>.Default.Equals(x.Item3, y.Item3) &&
                   (x.Item4 ?? Array.Empty<T4>()).SequenceEqual(y.Item4 ?? Array.Empty<T4>());
        }

        int IEqualityComparer<(T1, T2, T3, T4[])>.GetHashCode((T1, T2, T3, T4[]) obj)
        {
            var hash = new HashCode();

            hash.Add(obj.Item1);
            hash.Add(obj.Item2);
            hash.Add(obj.Item3);
            foreach (var elem in obj.Item4 ?? Array.Empty<T4>())
            {
                hash.Add(elem);
            }

            return hash.ToHashCode();
        }
    }

    public sealed class ParamsEqualityComparer<T1, T2, T3, T4, T5> : IEqualityComparer<(T1, T2, T3, T4, T5[])>
    {
        public static ParamsEqualityComparer<T1, T2, T3, T4, T5> Default = new ();
        bool IEqualityComparer<(T1, T2, T3, T4, T5[])>.Equals((T1, T2, T3, T4, T5[]) x, (T1, T2, T3, T4, T5[]) y)
        {
            return EqualityComparer<T1>.Default.Equals(x.Item1, y.Item1) &&
                   EqualityComparer<T2>.Default.Equals(x.Item2, y.Item2) &&
                   EqualityComparer<T3>.Default.Equals(x.Item3, y.Item3) &&
                   EqualityComparer<T4>.Default.Equals(x.Item4, y.Item4) &&
                   (x.Item5 ?? Array.Empty<T5>()).SequenceEqual(y.Item5 ?? Array.Empty<T5>());
        }

        int IEqualityComparer<(T1, T2, T3, T4, T5[])>.GetHashCode((T1, T2, T3, T4, T5[]) obj)
        {
            var hash = new HashCode();

            hash.Add(obj.Item1);
            hash.Add(obj.Item2);
            hash.Add(obj.Item3);
            hash.Add(obj.Item4);
            foreach (var elem in obj.Item5 ?? Array.Empty<T5>())
            {
                hash.Add(elem);
            }

            return hash.ToHashCode();
        }
    }

    public sealed class ParamsEqualityComparer<T1, T2, T3, T4, T5, T6> : IEqualityComparer<(T1, T2, T3, T4, T5, T6[])>
    {
        public static ParamsEqualityComparer<T1, T2, T3, T4, T5, T6> Default = new ();
        bool IEqualityComparer<(T1, T2, T3, T4, T5, T6[])>.Equals((T1, T2, T3, T4, T5, T6[]) x, (T1, T2, T3, T4, T5, T6[]) y)
        {
            return EqualityComparer<T1>.Default.Equals(x.Item1, y.Item1) &&
                   EqualityComparer<T2>.Default.Equals(x.Item2, y.Item2) &&
                   EqualityComparer<T3>.Default.Equals(x.Item3, y.Item3) &&
                   EqualityComparer<T4>.Default.Equals(x.Item4, y.Item4) &&
                   EqualityComparer<T5>.Default.Equals(x.Item5, y.Item5) &&
                   (x.Item6 ?? Array.Empty<T6>()).SequenceEqual(y.Item6 ?? Array.Empty<T6>());
        }

        int IEqualityComparer<(T1, T2, T3, T4, T5, T6[])>.GetHashCode((T1, T2, T3, T4, T5, T6[]) obj)
        {
            var hash = new HashCode();

            hash.Add(obj.Item1);
            hash.Add(obj.Item2);
            hash.Add(obj.Item3);
            hash.Add(obj.Item4);
            hash.Add(obj.Item5);
            foreach (var elem in obj.Item6 ?? Array.Empty<T6>())
            {
                hash.Add(elem);
            }

            return hash.ToHashCode();
        }
    }

    public sealed class ParamsEqualityComparer<T1, T2, T3, T4, T5, T6, T7> : IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7[])>
    {
        public static ParamsEqualityComparer<T1, T2, T3, T4, T5, T6, T7> Default = new ();
        bool IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7[])>.Equals((T1, T2, T3, T4, T5, T6, T7[]) x, (T1, T2, T3, T4, T5, T6, T7[]) y)
        {
            return EqualityComparer<T1>.Default.Equals(x.Item1, y.Item1) &&
                   EqualityComparer<T2>.Default.Equals(x.Item2, y.Item2) &&
                   EqualityComparer<T3>.Default.Equals(x.Item3, y.Item3) &&
                   EqualityComparer<T4>.Default.Equals(x.Item4, y.Item4) &&
                   EqualityComparer<T5>.Default.Equals(x.Item5, y.Item5) &&
                   EqualityComparer<T6>.Default.Equals(x.Item6, y.Item6) &&
                   (x.Item7 ?? Array.Empty<T7>()).SequenceEqual(y.Item7 ?? Array.Empty<T7>());
        }

        int IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7[])>.GetHashCode((T1, T2, T3, T4, T5, T6, T7[]) obj)
        {
            var hash = new HashCode();

            hash.Add(obj.Item1);
            hash.Add(obj.Item2);
            hash.Add(obj.Item3);
            hash.Add(obj.Item4);
            hash.Add(obj.Item5);
            hash.Add(obj.Item6);
            foreach (var elem in obj.Item7 ?? Array.Empty<T7>())
            {
                hash.Add(elem);
            }

            return hash.ToHashCode();
        }
    }

    public sealed class ParamsEqualityComparer<T1, T2, T3, T4, T5, T6, T7, T8> : IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7, T8[])>
    {
        public static ParamsEqualityComparer<T1, T2, T3, T4, T5, T6, T7, T8> Default = new ();
        bool IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7, T8[])>.Equals((T1, T2, T3, T4, T5, T6, T7, T8[]) x, (T1, T2, T3, T4, T5, T6, T7, T8[]) y)
        {
            return EqualityComparer<T1>.Default.Equals(x.Item1, y.Item1) &&
                   EqualityComparer<T2>.Default.Equals(x.Item2, y.Item2) &&
                   EqualityComparer<T3>.Default.Equals(x.Item3, y.Item3) &&
                   EqualityComparer<T4>.Default.Equals(x.Item4, y.Item4) &&
                   EqualityComparer<T5>.Default.Equals(x.Item5, y.Item5) &&
                   EqualityComparer<T6>.Default.Equals(x.Item6, y.Item6) &&
                   EqualityComparer<T7>.Default.Equals(x.Item7, y.Item7) &&
                   (x.Item8 ?? Array.Empty<T8>()).SequenceEqual(y.Item8 ?? Array.Empty<T8>());
        }

        int IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7, T8[])>.GetHashCode((T1, T2, T3, T4, T5, T6, T7, T8[]) obj)
        {
            var hash = new HashCode();

            hash.Add(obj.Item1);
            hash.Add(obj.Item2);
            hash.Add(obj.Item3);
            hash.Add(obj.Item4);
            hash.Add(obj.Item5);
            hash.Add(obj.Item6);
            hash.Add(obj.Item7);
            foreach (var elem in obj.Item8 ?? Array.Empty<T8>())
            {
                hash.Add(elem);
            }

            return hash.ToHashCode();
        }
    }
}
