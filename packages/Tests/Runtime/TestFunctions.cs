using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Katuusagi.MemoizationForUnity.Tests
{
    public partial class TestFunctions
    {
        public partial struct StructTest
        {
            [Memoization]
            public static int FindLargestPrimeRaw(int n)
            {
                bool[] isPrime = new bool[n + 1];
                for (int i = 2; i <= n; i++)
                {
                    isPrime[i] = true;
                }

                for (int p = 2; p * p <= n; p++)
                {
                    if (!isPrime[p])
                    {
                        continue;
                    }

                    for (int i = p * p; i <= n; i += p)
                    {
                        isPrime[i] = false;
                    }
                }

                for (int i = n; i >= 2; i--)
                {
                    if (isPrime[i])
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        [Memoization]
        public static bool TryTableGetValueRaw(Dictionary<int, int> table, int key, out int result)
        {
            return table.TryGetValue(key, out result);
        }

        [Memoization]
        public static bool TryTableGetValueRefRaw(Dictionary<int, int> table, ref int keyAndResult)
        {
            return table.TryGetValue(keyAndResult, out keyAndResult);
        }

        [Memoization]
        public static void TableGetValueRaw(Dictionary<int, int> table, int key, out int y)
        {
            table.TryGetValue(key, out y);
        }

        [Memoization]
        public static int GetLengthRaw(string str)
        {
            return str?.Length ?? 0;
        }

        [Memoization]
        public static int GetCountRaw(params byte[] ps)
        {
            return ps?.Length ?? 0;
        }

        [Memoization]
        public static int TableGetValueRaw(Dictionary<string, int> table, params byte[] ps)
        {
            return table[Encoding.UTF8.GetString(ps)];
        }

        [Memoization(IsThreadSafe = true)]
        public static float GetLengthRaw(in Vector3 value)
        {
            return value.magnitude;
        }

        [Memoization(IsThreadSafe = true, IsClearable = true)]
        public static string GetTypeFullNameRaw<T>()
        {
            return typeof(T).FullName;
        }

        [Memoization]
        public static int FindLargestPrimeRaw(int n)
        {
            bool[] isPrime = new bool[n + 1];
            for (int i = 2; i <= n; i++)
            {
                isPrime[i] = true;
            }

            for (int p = 2; p * p <= n; p++)
            {
                if (!isPrime[p])
                {
                    continue;
                }

                for (int i = p * p; i <= n; i += p)
                {
                    isPrime[i] = false;
                }
            }

            for (int i = n; i >= 2; i--)
            {
                if (isPrime[i])
                {
                    return i;
                }
            }

            return -1;
        }

        [Memoization]
        public static Type[] GetTypesRaw()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(v => v.GetTypes()).ToArray();
        }

        [Memoization]
        public static MethodInfo GetMethodInfoRaw(Type type, string name, BindingFlags bindingFlags)
        {
            return type.GetMethod(name, bindingFlags);
        }

        private void Dummy()
        {
        }

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
