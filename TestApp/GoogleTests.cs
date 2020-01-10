using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestApp.Google
{
    [TestClass]
    public class GoogleTests: BaseTest
    {
        [TestMethod]
        [TestCategory("QA"), TestCategory("Google")]
        public async Task SampleTestCaseToVerifyGoogleSearch()
        {
            await RunTest(@"{'Plugin':'Google','Header':'GoogleHeaders'}");
        }
    }
}
