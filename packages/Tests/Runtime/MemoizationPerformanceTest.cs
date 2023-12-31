using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using Unity.PerformanceTesting;

namespace Katuusagi.MemoizationForUnity.Tests
{
    public class MemoizationPerformanceTest
    {
        [Test]
        [Performance]
        public void CalcPrime_Memoized()
        {
            Measure.Method(() =>
            {
                TestFunctions.FindLargestPrime(1000);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void CalcPrime_Raw()
        {
            Measure.Method(() =>
            {
                TestFunctions.FindLargestPrimeRaw(1000);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void GetTypes_Memoized()
        {
            Measure.Method(() =>
            {
                TestFunctions.GetTypes();
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void GetTypes_Raw()
        {
            Measure.Method(() =>
            {
                TestFunctions.GetTypesRaw();
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void GetMethodInfo_Memoized()
        {
            Measure.Method(() =>
            {
                TestFunctions.GetMethodInfo(typeof(TestFunctions), "Dummy", BindingFlags.NonPublic | BindingFlags.Instance);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void GetMethodInfo_Raw()
        {
            Measure.Method(() =>
            {
                TestFunctions.GetMethodInfoRaw(typeof(TestFunctions), "Dummy", BindingFlags.NonPublic | BindingFlags.Instance);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void TryGetValue_BooleanCacheValue()
        {
            var table = new BooleanCacheValue<int>();
            table.Add(true, 1);
            table.Add(false, 0);
            Measure.Method(() =>
            {
                table.TryGetValue(true, out _);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void TryGetValue_BooleanDictionary()
        {
            var table = new Dictionary<bool, int>(2);
            table.Add(true, 1);
            table.Add(false, 0);
            Measure.Method(() =>
            {
                table.TryGetValue(true, out _);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }
    }
}
