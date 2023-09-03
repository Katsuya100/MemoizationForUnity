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
    }
}
