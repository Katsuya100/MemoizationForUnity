using System;
using System.Collections.Generic;
using System.Linq;

namespace Katuusagi.MemoizationForUnity
{
    public sealed class ArrayElementEqualityComparer1<T1> : IEqualityComparer<ValueTuple<T1[]>>
    {
        public static readonly ArrayElementEqualityComparer1<T1> Default = new ();
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
}
