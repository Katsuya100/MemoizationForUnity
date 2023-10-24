namespace Katuusagi.MemoizationForUnity.Tests
{
    public partial class TestFunctionsGeneric<T>
    {
        public partial struct TestFunctionsGenericStruct<T2>
        {
            [Memoization]
            public static int ThrewRaw(int a)
            {
                return a;
            }
        }
    }
}
