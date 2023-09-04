using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Katuusagi.MemoizationForUnity.Tests
{
    public class MemoizationTest
    {
        [SetUp]
        public void Init()
        {
            TestFunctions.ClearStaticMemoizationCache();
        }

        [Test]
        public void StaticAutoNaming()
        {
            TestFunctions.StaticSimpleWithMemoization();
            TestFunctions.StaticReturnOnly();
        }

        [Test]
        public void StaticNonArgs()
        {
            var l = TestFunctions.StaticAddCounter();
            var r = TestFunctions.StaticAddCounter();
            Assert.AreEqual(l, r);
        }

        [Test]
        public void StaticArg1()
        {
            var l = TestFunctions.StaticAddCounter(10);
            var r = TestFunctions.StaticAddCounter(10);
            Assert.AreEqual(l, r);
        }

        [Test]
        public void StaticArg2()
        {
            var l = TestFunctions.StaticAddCounter(10, 20);
            var r = TestFunctions.StaticAddCounter(10, 20);
            Assert.AreEqual(l, r);
        }

        [Test]
        public void StaticClear()
        {
            var l = TestFunctions.StaticAddCounterClearable();
            var r = TestFunctions.StaticAddCounterClearable();
            Assert.AreEqual(l, r);

            TestFunctions.ClearStaticMemoizationCache();
            r = TestFunctions.StaticAddCounterClearable();
            Assert.AreNotEqual(l, r);

            l = TestFunctions.StaticAddCounterClearable(10);
            r = TestFunctions.StaticAddCounterClearable(10);
            Assert.AreEqual(l, r);

            TestFunctions.ClearStaticMemoizationCache();
            r = TestFunctions.StaticAddCounterClearable(10);
            Assert.AreNotEqual(l, r);

            l = TestFunctions.StaticAddCounterClearable(10, 20);
            r = TestFunctions.StaticAddCounterClearable(10, 20);
            Assert.AreEqual(l, r);

            TestFunctions.ClearStaticMemoizationCache();
            r = TestFunctions.StaticAddCounterClearable(10, 20);
            Assert.AreNotEqual(l, r);
        }

        [Test]
        public void StaticChangeModifier()
        {
            TestFunctions.StaticPrivateMethod();
        }

        [Test]
        public void StaticChangeName()
        {
            TestFunctions.StaticChangedMethod();
        }

        [Test]
        public void StaticGenericAutoNaming()
        {
            TestFunctions.StaticSimpleWithMemoization<int>();
            TestFunctions.StaticReturnOnly<int>();
        }

        [Test]
        public void StaticGenericNonArgs()
        {
            var l = TestFunctions.StaticAddCounter<int>();
            var r = TestFunctions.StaticAddCounter<int>();
            Assert.AreEqual(l, r);
        }

        [Test]
        public void StaticGenericArg1()
        {
            var l = TestFunctions.StaticAddCounter<int>(10);
            var r = TestFunctions.StaticAddCounter<int>(10);
            Assert.AreEqual(l, r);
        }

        [Test]
        public void StaticGenericArg2()
        {
            var l = TestFunctions.StaticAddCounter<int>(10, 20);
            var r = TestFunctions.StaticAddCounter<int>(10, 20);
            Assert.AreEqual(l, r);
        }

        [Test]
        public void StaticGenericClear()
        {
            var l = TestFunctions.StaticAddCounterClearable<int>();
            var r = TestFunctions.StaticAddCounterClearable<int>();
            Assert.AreEqual(l, r);

            TestFunctions.ClearStaticMemoizationCache();
            r = TestFunctions.StaticAddCounterClearable<int>();
            Assert.AreNotEqual(l, r);

            l = TestFunctions.StaticAddCounterClearable<int>(10);
            r = TestFunctions.StaticAddCounterClearable<int>(10);
            Assert.AreEqual(l, r);

            TestFunctions.ClearStaticMemoizationCache();
            r = TestFunctions.StaticAddCounterClearable<int>(10);
            Assert.AreNotEqual(l, r);

            l = TestFunctions.StaticAddCounterClearable<int>(10, 20);
            r = TestFunctions.StaticAddCounterClearable<int>(10, 20);
            Assert.AreEqual(l, r);

            TestFunctions.ClearStaticMemoizationCache();
            r = TestFunctions.StaticAddCounterClearable<int>(10, 20);
            Assert.AreNotEqual(l, r);
        }

        [Test]
        public void StaticGenericChangeModifier()
        {
            TestFunctions.StaticPrivateMethod<int>();
        }

        [Test]
        public void StaticGenericChangeName()
        {
            TestFunctions.StaticChangedMethod<int>();
        }

        [Test]
        public void StaticGenericArgType()
        {
            var dic = new Dictionary<int, int>() { { 10, 10 } };
            var l = TestFunctions.StaticGetTable(dic, 10);
            dic[10] = 100;
            var r = TestFunctions.StaticGetTable(dic, 10);
            Assert.AreEqual(l, r);

            var dic2 = new Dictionary<int, long>() { { 10, 10 } };
            var l2 = TestFunctions.StaticGetTable(dic2, 10);
            dic[10] = 100;
            var r2 = TestFunctions.StaticGetTable(dic2, 10);
            Assert.AreEqual(l2, r2);
        }

        [Test]
        public void StaticGenericClearArgType()
        {
            var dic = new Dictionary<int, int>() { { 10, 10 } };
            var l = TestFunctions.StaticClearableGetTable(dic, 10);
            dic[10] = 100;
            var r = TestFunctions.StaticClearableGetTable(dic, 10);
            Assert.AreEqual(l, r);

            TestFunctions.ClearStaticMemoizationCache();
            r = TestFunctions.StaticClearableGetTable(dic, 10);
            Assert.AreNotEqual(l, r);
        }

        [Test]
        public void StaticManyParameter()
        {
            TestFunctions.StaticManyParameter(default, default, default, default, default, default, default, default, default);
            TestFunctions.StaticManyGenericParameter(default(float), default(float), default(float), default(float), default(float), default(float), default(float), default(float), default(float));
        }

        [Test]
        public void InstanceAutoNaming()
        {
            var funcs = new TestFunctions();
            funcs.InstanceSimpleWithMemoization();
            funcs.InstanceReturnOnly();
        }

        [Test]
        public void InstanceNonArgs()
        {
            var funcs = new TestFunctions();
            var l = funcs.InstanceAddCounter();
            var r = funcs.InstanceAddCounter();
            Assert.AreEqual(l, r);
        }

        [Test]
        public void InstanceArg1()
        {
            var funcs = new TestFunctions();
            var l = funcs.InstanceAddCounter(10);
            var r = funcs.InstanceAddCounter(10);
            Assert.AreEqual(l, r);
        }

        [Test]
        public void InstanceArg2()
        {
            var funcs = new TestFunctions();
            var l = funcs.InstanceAddCounter(10, 20);
            var r = funcs.InstanceAddCounter(10, 20);
            Assert.AreEqual(l, r);
        }

        [Test]
        public void InstanceClear()
        {
            var funcs = new TestFunctions();
            var l = funcs.InstanceAddCounterClearable();
            var r = funcs.InstanceAddCounterClearable();
            Assert.AreEqual(l, r);

            funcs.ClearInstanceMemoizationCache();
            r = funcs.InstanceAddCounterClearable();
            Assert.AreNotEqual(l, r);

            l = funcs.InstanceAddCounterClearable(10);
            r = funcs.InstanceAddCounterClearable(10);
            Assert.AreEqual(l, r);

            funcs.ClearInstanceMemoizationCache();
            r = funcs.InstanceAddCounterClearable(10);
            Assert.AreNotEqual(l, r);

            l = funcs.InstanceAddCounterClearable(10, 20);
            r = funcs.InstanceAddCounterClearable(10, 20);
            Assert.AreEqual(l, r);

            funcs.ClearInstanceMemoizationCache();
            r = funcs.InstanceAddCounterClearable(10, 20);
            Assert.AreNotEqual(l, r);
        }

        [Test]
        public void InstanceChangeModifier()
        {
            var funcs = new TestFunctions();
            funcs.InstancePrivateMethod();
        }

        [Test]
        public void InstanceChangeName()
        {
            var funcs = new TestFunctions();
            funcs.InstanceChangedMethod();
        }

        [Test]
        public void InstanceGenericAutoNaming()
        {
            var funcs = new TestFunctions();
            funcs.InstanceSimpleWithMemoization<int>();
            funcs.InstanceReturnOnly<int>();
        }

        [Test]
        public void InstanceGenericNonArgs()
        {
            var funcs = new TestFunctions();
            var l = funcs.InstanceAddCounter<int>();
            var r = funcs.InstanceAddCounter<int>();
            Assert.AreEqual(l, r);
        }

        [Test]
        public void InstanceGenericArg1()
        {
            var funcs = new TestFunctions();
            var l = funcs.InstanceAddCounter<int>(10);
            var r = funcs.InstanceAddCounter<int>(10);
            Assert.AreEqual(l, r);
        }

        [Test]
        public void InstanceGenericArg2()
        {
            var funcs = new TestFunctions();
            var l = funcs.InstanceAddCounter<int>(10, 20);
            var r = funcs.InstanceAddCounter<int>(10, 20);
            Assert.AreEqual(l, r);
        }

        [Test]
        public void InstanceGenericClear()
        {
            var funcs = new TestFunctions();
            var l = funcs.InstanceAddCounterClearable<int>();
            var r = funcs.InstanceAddCounterClearable<int>();
            Assert.AreEqual(l, r);

            funcs.ClearInstanceMemoizationCache();
            r = funcs.InstanceAddCounterClearable<int>();
            Assert.AreNotEqual(l, r);

            l = funcs.InstanceAddCounterClearable<int>(10);
            r = funcs.InstanceAddCounterClearable<int>(10);
            Assert.AreEqual(l, r);

            funcs.ClearInstanceMemoizationCache();
            r = funcs.InstanceAddCounterClearable<int>(10);
            Assert.AreNotEqual(l, r);

            l = funcs.InstanceAddCounterClearable<int>(10, 20);
            r = funcs.InstanceAddCounterClearable<int>(10, 20);
            Assert.AreEqual(l, r);

            funcs.ClearInstanceMemoizationCache();
            r = funcs.InstanceAddCounterClearable<int>(10, 20);
            Assert.AreNotEqual(l, r);
        }

        [Test]
        public void InstanceGenericChangeModifier()
        {
            var funcs = new TestFunctions();
            funcs.InstancePrivateMethod<int>();
        }

        [Test]
        public void InstanceGenericChangeName()
        {
            var funcs = new TestFunctions();
            funcs.InstanceChangedMethod<int>();
        }

        [Test]
        public void InstanceGenericArgType()
        {
            var funcs = new TestFunctions();
            var dic = new Dictionary<int, int>() { { 10, 10 } };
            var l = funcs.InstanceGetTable(dic, 10);
            dic[10] = 100;
            var r = funcs.InstanceGetTable(dic, 10);
            Assert.AreEqual(l, r);

            var dic2 = new Dictionary<int, long>() { { 10, 10 } };
            var l2 = funcs.InstanceGetTable(dic2, 10);
            dic[10] = 100;
            var r2 = funcs.InstanceGetTable(dic2, 10);
            Assert.AreEqual(l2, r2);
        }

        [Test]
        public void InstanceGenericClearArgType()
        {
            var funcs = new TestFunctions();
            var dic = new Dictionary<int, int>() { { 10, 10 } };
            var l = funcs.InstanceClearableGetTable(dic, 10);
            dic[10] = 100;
            var r = funcs.InstanceClearableGetTable(dic, 10);
            Assert.AreEqual(l, r);
            funcs.ClearInstanceMemoizationCache();
            r = funcs.InstanceClearableGetTable(dic, 10);
            Assert.AreNotEqual(l, r);
        }

        [Test]
        public void InstanceManyParameter()
        {
            var funcs = new TestFunctions();
            funcs.InstanceManyParameter(default, default, default, default, default, default, default, default, default);
            funcs.InstanceManyGenericParameter(default(float), default(float), default(float), default(float), default(float), default(float), default(float), default(float), default(float));
        }

        [Test]
        public void Struct()
        {
            TestFunctions.StructTest.FindLargestPrime(100);
            Assert.AreEqual(TestFunctions.StructTest.FindLargestPrime(100), TestFunctions.StructTest.FindLargestPrimeRaw(100));
        }

        [Test]
        public void OutParameter()
        {
            var dic = new Dictionary<int, int>() { { 10, 20 } };
            var lResult = TestFunctions.TryTableGetValue(dic, 10, out var lValue);
            dic.Remove(10);
            var rResult = TestFunctions.TryTableGetValue(dic, 10, out var rValue);
            Assert.AreEqual(lResult, rResult);
            Assert.AreEqual(lValue, rValue);
        }

        [Test]
        public void RefParameter()
        {
            var dic = new Dictionary<int, int>() { { 10, 20 } };
            var lValue = 10;
            var lResult = TestFunctions.TryTableGetValueRef(dic, ref lValue);
            dic.Remove(10);
            var rValue = 10;
            var rResult = TestFunctions.TryTableGetValueRef(dic, ref rValue);
            Assert.AreEqual(lResult, rResult);
            Assert.AreEqual(lValue, rValue);
        }

        [Test]
        public void OutOnly()
        {
            var dic = new Dictionary<int, int>() { { 10, 20 } };
            TestFunctions.TableGetValue(dic, 10, out var lValue);
            dic.Remove(10);
            TestFunctions.TableGetValue(dic, 10, out var rValue);
            Assert.AreEqual(lValue, rValue);
        }

        [Test]
        public void NullInput()
        {
            var lLength = TestFunctions.GetLength(null);
            var rLength = TestFunctions.GetLength(null);
            Assert.AreEqual(lLength, rLength);
        }

        [Test]
        public void Params()
        {
            var lCount = TestFunctions.GetCount(null);
            var rCount = TestFunctions.GetCount();
            Assert.AreEqual(lCount, rCount);
            var value = TestFunctions.GetCount((byte)'a', (byte)'b', (byte)'c');
            Assert.AreEqual(value, 3);

            var dic = new Dictionary<string, int>() { { "abc", 20 } };
            var lResult = TestFunctions.TableGetValue(dic, (byte)'a', (byte)'b', (byte)'c');
            dic.Remove("abc");
            var rResult = TestFunctions.TableGetValue(dic, (byte)'a', (byte)'b', (byte)'c');
            Assert.AreEqual(lResult, rResult);
        }

        [Test]
        public void Parallel_()
        {
            var result = Parallel.ForEach(Enumerable.Range(0, 10000).Select(_ => UnityEngine.Random.insideUnitSphere).ToArray(), (v) =>
            {
                TestFunctions.GetLength(v);
            });

            var wait = new SpinWait();
            while (!result.IsCompleted)
            {
                wait.SpinOnce();
            }

            result = Parallel.For(0, 10000, (i) =>
            {
                if (i % 2 == 0)
                {
                    TestFunctions.GetTypeFullName<int>();
                }
                else
                {
                    TestFunctions.ClearStaticMemoizationCache();
                }
            });

            while (!result.IsCompleted)
            {
                wait.SpinOnce();
            }
        }
    }
}
