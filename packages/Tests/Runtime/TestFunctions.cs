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
        public static async Task<int> WaitThrewRaw(int a)
        {
            await Task.Yield();
            return a;
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
    }
}
