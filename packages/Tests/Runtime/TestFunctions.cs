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

        [Memoization(InterruptCacheMethod = nameof(InterruptMethodInfoCache), OnCachedMethod = nameof(OnCachedMethodInfo))]
        public static MethodInfo GetMethodInfoRaw(Type type, string name)
        {
            return type.GetMethod(name);
        }

        private static partial void OnCachedMethodInfo((Type type, string name) key, MethodInfo result)
        {
            InterruptMethodInfoCache((key.type, key.name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public), result);
        }


        [Memoization(InterruptCacheMethod = nameof(InterruptMethodInfoCache), OnCachedMethod = nameof(OnCachedMethodInfo))]
        public static MethodInfo GetMethodInfoRaw(Type type, string name, BindingFlags bindingFlags)
        {
            return type.GetMethod(name, bindingFlags);
        }

        private static partial void OnCachedMethodInfo((Type type, string name, BindingFlags bindingFlags) key, MethodInfo result)
        {
            InterruptMethodInfoCache((key.type, key.name), result);
        }

        [Memoization(InterruptCacheMethod = nameof(InterruptMethodInfoCache), OnCachedMethod = nameof(OnCachedMethodInfo))]
        public static MethodInfo GetMethodInfoRaw<T>(string name, BindingFlags bindingFlags)
        {
            return typeof(T).GetMethod(name, bindingFlags);
        }

        private static partial void OnCachedMethodInfo<T>((string name, BindingFlags bindingFlags) key, MethodInfo result)
        {
            InterruptMethodInfoCache((typeof(T), key.name), result);
            InterruptMethodInfoCache((typeof(T), key.name, key.bindingFlags), result);

            InterruptMethodInfoCache<T>(ValueTuple.Create(key.Item1), result);
        }

        [Memoization(InterruptCacheMethod = nameof(InterruptMethodInfoCache), OnCachedMethod = nameof(OnCachedMethodInfo))]
        public static MethodInfo GetMethodInfoRaw<T>(string name)
        {
            return typeof(T).GetMethod(name);
        }

        private static partial void OnCachedMethodInfo<T>(ValueTuple<string> key, MethodInfo result)
        {
            InterruptMethodInfoCache((typeof(T), key.Item1), result);
            InterruptMethodInfoCache((typeof(T), key.Item1, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public), result);

            InterruptMethodInfoCache<T>((key.Item1, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public), result);
        }

        [Memoization(InterruptCacheMethod = nameof(InterruptMethodInfosCache), OnCachedMethod = nameof(OnCachedMethodInfos))]
        public static MethodInfo[] GetMethodInfosRaw(Type type)
        {
            return type.GetMethods();
        }

        private static partial void OnCachedMethodInfos(ValueTuple<Type> key, MethodInfo[] result)
        {
            foreach (var method in result)
            {
                InterruptMethodInfoCache((key.Item1, method.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public), method);
                InterruptMethodInfoCache((key.Item1, method.Name), method);
            }

            InterruptMethodInfosCache((key.Item1, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public), result);
        }

        [Memoization(InterruptCacheMethod = nameof(InterruptMethodInfosCache), OnCachedMethod = nameof(OnCachedMethodInfos))]
        public static MethodInfo[] GetMethodInfosRaw(Type type, BindingFlags bindingFlags)
        {
            return type.GetMethods(bindingFlags);
        }

        private static partial void OnCachedMethodInfos((Type type, BindingFlags bindingFlags) key, MethodInfo[] result)
        {
            foreach (var method in result)
            {
                InterruptMethodInfoCache((key.type, method.Name, key.bindingFlags), method);
                InterruptMethodInfoCache((key.type, method.Name), method);
            }

            InterruptMethodInfosCache(ValueTuple.Create(key.type), result);
        }

        [Memoization(InterruptCacheMethod = nameof(InterruptMethodInfosCache), OnCachedMethod = nameof(OnCachedMethodInfos))]
        public static MethodInfo[] GetMethodInfosRaw<T>()
        {
            return typeof(T).GetMethods();
        }

        private static partial void OnCachedMethodInfos<T>(MethodInfo[] result)
        {
            foreach (var method in result)
            {
                InterruptMethodInfoCache((typeof(T), method.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public), method);
                InterruptMethodInfoCache((typeof(T), method.Name), method);

                InterruptMethodInfoCache<T>((method.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public), method);
                InterruptMethodInfoCache<T>(ValueTuple.Create(method.Name), method);
            }

            InterruptMethodInfosCache(ValueTuple.Create(typeof(T)), result);
            InterruptMethodInfosCache((typeof(T), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public), result);

            InterruptMethodInfosCache<T>(ValueTuple.Create(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public), result);
        }

        [Memoization(InterruptCacheMethod = nameof(InterruptMethodInfosCache), OnCachedMethod = nameof(OnCachedMethodInfos))]
        public static MethodInfo[] GetMethodInfosRaw<T>(BindingFlags bindingFlags)
        {
            return typeof(T).GetMethods(bindingFlags);
        }

        private static partial void OnCachedMethodInfos<T>(ValueTuple<BindingFlags> key, MethodInfo[] result)
        {
            foreach (var method in result)
            {
                InterruptMethodInfoCache((typeof(T), method.Name, key.Item1), method);
                InterruptMethodInfoCache((typeof(T), method.Name), method);

                InterruptMethodInfoCache<T>((method.Name, key.Item1), method);
                InterruptMethodInfoCache<T>(ValueTuple.Create(method.Name), method);
            }

            InterruptMethodInfosCache(ValueTuple.Create(typeof(T)), result);
            InterruptMethodInfosCache((typeof(T), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public), result);

            InterruptMethodInfosCache<T>(result);
        }


        private void Dummy()
        {
        }
    }
}
