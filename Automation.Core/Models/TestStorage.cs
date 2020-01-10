namespace Bromine.Automation.Core.Models
{
    public class TestStorage
    {
        public TestStorage()
        {
            Summary= new Summary();
            Cache= new TestCache();
        }

        public TestCache Cache { get; set; }
        public Summary Summary { get; set; }
    }
}
