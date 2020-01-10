using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestApp
{
    /// <summary>
    /// Its a limitation of MSTest to have a AssemblyInitialize in the same assembly
    /// </summary>
    public class BaseTest : Bromine.TestRunner.BaseTest
    {
        [AssemblyInitialize]
        public static void Init(TestContext testContext)
        {
            Initialize(testContext);
        }

        [AssemblyCleanup]
        public static void Clean()
        {
           Cleanup();
        }
    }
}
