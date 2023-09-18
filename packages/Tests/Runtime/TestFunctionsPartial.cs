using System.Collections.Generic;

namespace Katuusagi.MemoizationForUnity.Tests
{
    public partial class TestFunctions
    {
        private static int _staticCounter = 0;

        [Memoization]
        public static int StaticSimple()
        {
            return 100;
        }

        [Memoization]
        public static int StaticReturnOnlyRaw()
        {
            return 100;
        }

        [Memoization]
        public static int StaticAddCounterRaw()
        {
            return ++_staticCounter;
        }

        [Memoization]
        public static int StaticAddCounterRaw(int count)
        {
            return _staticCounter += count;
        }

        [Memoization]
        public static int StaticAddCounterRaw(int count1, int count2)
        {
            return _staticCounter += count1 + count2;
        }

        [Memoization(IsClearable = true)]
        public static int StaticAddCounterClearableRaw()
        {
            return ++_staticCounter;
        }

        [Memoization(IsClearable = true)]
        public static int StaticAddCounterClearableRaw(int count)
        {
            return _staticCounter += count;
        }

        [Memoization(IsClearable = true)]
        public static int StaticAddCounterClearableRaw(int count1, int count2)
        {
            return _staticCounter += count1 + count2;
        }

        [Memoization(Modifier = "public static")]
        private static int StaticPrivateMethodRaw()
        {
            return 100;
        }

        [Memoization(MethodName = nameof(StaticChangedMethod))]
        public static int StaticChangeableMethod()
        {
            return 100;
        }

        [Memoization]
        public static int StaticSimple<T>()
        {
            return 100;
        }

        [Memoization]
        public static int StaticReturnOnlyRaw<T>()
        {
            return 100;
        }

        [Memoization]
        public static int StaticAddCounterRaw<T>()
        {
            return ++_staticCounter;
        }

        [Memoization]
        public static int StaticAddCounterRaw<T>(int count)
        {
            return _staticCounter += count;
        }

        [Memoization]
        public static int StaticAddCounterRaw<T>(int count1, int count2)
        {
            return _staticCounter += count1 + count2;
        }

        [Memoization(IsClearable = true)]
        public static int StaticAddCounterClearableRaw<T>()
        {
            return ++_staticCounter;
        }

        [Memoization(IsClearable = true)]
        public static int StaticAddCounterClearableRaw<T>(int count)
        {
            return _staticCounter += count;
        }

        [Memoization(IsClearable = true)]
        public static int StaticAddCounterClearableRaw<T>(int count1, int count2)
        {
            return _staticCounter += count1 + count2;
        }

        [Memoization(Modifier = "public static")]
        private static int StaticPrivateMethodRaw<T>()
        {
            return 100;
        }

        [Memoization(MethodName = nameof(StaticChangedMethod))]
        public static int StaticChangeableMethod<T>()
        {
            return 100;
        }

        [Memoization]
        public static TValue StaticGetTableRaw<TKey, TValue>(Dictionary<TKey, TValue> table, TKey key)
        {
            return table[key];
        }

        [Memoization(IsClearable = true)]
        public static TValue StaticClearableGetTableRaw<TKey, TValue>(Dictionary<TKey, TValue> table, TKey key)
        {
            return table[key];
        }

        [Memoization]
        public static int StaticParamsRaw(params int[] a9)
        {
            return default;
        }

        [Memoization]
        public static int StaticManyParameterRaw(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9)
        {
            return default;
        }

        [Memoization]
        public static int StaticManyGenericParameterRaw<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9)
        {
            return default;
        }

        [Memoization]
        public static int StaticManyParameterAndParamsRaw(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, params int[] a9)
        {
            return default;
        }

        [Memoization(CompareArrayElement = true)]
        public static int StaticArrayElementRaw(int[] a1, int a2, int[] a3)
        {
            var result = 0;
            foreach (var e in a1)
            {
                result += e;
            }

            result += a2;
            foreach (var e in a3)
            {
                result += e;
            }

            ++_staticCounter;
            return result + _staticCounter;
        }

        private int _instanceCounter = 0;

        [Memoization]
        public int InstanceSimple()
        {
            return 100;
        }

        [Memoization]
        public int InstanceReturnOnlyRaw()
        {
            return 100;
        }

        [Memoization]
        public int InstanceAddCounterRaw()
        {
            return ++_instanceCounter;
        }

        [Memoization]
        public int InstanceAddCounterRaw(int count)
        {
            return _instanceCounter += count;
        }

        [Memoization]
        public int InstanceAddCounterRaw(int count1, int count2)
        {
            return _instanceCounter += count1 + count2;
        }

        [Memoization(IsClearable = true)]
        public int InstanceAddCounterClearableRaw()
        {
            return ++_instanceCounter;
        }

        [Memoization(IsClearable = true)]
        public int InstanceAddCounterClearableRaw(int count)
        {
            return _instanceCounter += count;
        }

        [Memoization(IsClearable = true)]
        public int InstanceAddCounterClearableRaw(int count1, int count2)
        {
            return _instanceCounter += count1 + count2;
        }

        [Memoization(Modifier = "public")]
        private int InstancePrivateMethodRaw()
        {
            return 100;
        }

        [Memoization(MethodName = nameof(InstanceChangedMethod))]
        public int InstanceChangeableMethod()
        {
            return 100;
        }

        [Memoization]
        public int InstanceSimple<T>()
        {
            return 100;
        }

        [Memoization]
        public int InstanceReturnOnlyRaw<T>()
        {
            return 100;
        }

        [Memoization]
        public int InstanceAddCounterRaw<T>()
        {
            return ++_instanceCounter;
        }

        [Memoization]
        public int InstanceAddCounterRaw<T>(int count)
        {
            return _instanceCounter += count;
        }

        [Memoization]
        public int InstanceAddCounterRaw<T>(int count1, int count2)
        {
            return _instanceCounter += count1 + count2;
        }

        [Memoization(IsClearable = true)]
        public int InstanceAddCounterClearableRaw<T>()
        {
            return ++_instanceCounter;
        }

        [Memoization(IsClearable = true)]
        public int InstanceAddCounterClearableRaw<T>(int count)
        {
            return _instanceCounter += count;
        }

        [Memoization(IsClearable = true)]
        public int InstanceAddCounterClearableRaw<T>(int count1, int count2)
        {
            return _instanceCounter += count1 + count2;
        }

        [Memoization(Modifier = "public")]
        private int InstancePrivateMethodRaw<T>()
        {
            return 100;
        }

        [Memoization(MethodName = nameof(InstanceChangedMethod))]
        public int InstanceChangeableMethod<T>()
        {
            return 100;
        }

        [Memoization]
        public TValue InstanceGetTableRaw<TKey, TValue>(Dictionary<TKey, TValue> table, TKey key)
        {
            return table[key];
        }

        [Memoization(IsClearable = true)]
        public TValue InstanceClearableGetTableRaw<TKey, TValue>(Dictionary<TKey, TValue> table, TKey key)
        {
            return table[key];
        }

        [Memoization]
        public int InstanceParamsRaw(params int[] a9)
        {
            return default;
        }

        [Memoization]
        public int InstanceManyParameterRaw(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9)
        {
            return default;
        }

        [Memoization]
        public int InstanceManyGenericParameterRaw<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9)
        {
            return default;
        }

        [Memoization]
        public int InstanceManyParameterAndParamsRaw(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, params int[] a9)
        {
            return default;
        }

        [Memoization(CompareArrayElement = true)]
        public int InstanceArrayElementRaw(int[] a1, int a2, int[] a3)
        {
            var result = 0;
            foreach (var e in a1)
            {
                result += e;
            }

            result += a2;
            foreach (var e in a3)
            {
                result += e;
            }

            ++_instanceCounter;
            return result + _instanceCounter;
        }

        [Memoization(CompareArrayElement = true)]
        public long InstanceArrayElementRaw(long[] a1, int a2, int[] a3)
        {
            long result = 0;
            foreach (var e in a1)
            {
                result += e;
            }

            result += a2;
            foreach (var e in a3)
            {
                result += e;
            }

            ++_instanceCounter;
            return result + _instanceCounter;
        }
    }
}
