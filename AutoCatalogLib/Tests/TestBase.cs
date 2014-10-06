using System;

namespace AutoCatalogLib.Tests
{
    public abstract class TestBase : ITest
    {
        public abstract void Run();

        protected static void Assert(bool expression)
        {
            if (!expression)
                throw new AssertationException();
        }

        protected static void AssertFalse(bool expression)
        {
            Assert(!expression);
        }

        protected static void Log(object obj)
        {
            Console.WriteLine("{0:T}: {1}", DateTime.Now, obj);
        }
    }
}