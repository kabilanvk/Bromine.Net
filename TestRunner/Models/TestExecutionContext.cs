using Bromine.Automation.Core.Models;
using Bromine.TestRunner.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bromine.TestRunner.Models
{
  public class TestExecutionContext
  {
    public TestExecutionContext(TestStorage storage, WrapperHelper wrapper, TestContext testContext)
    {
      Storage = storage;
      Wrapper = wrapper;
      TestContext = testContext;
    }

    public TestStorage Storage { get; set; }

    public WrapperHelper Wrapper { get; }

    public TestContext TestContext { get; }
  }
}
