using System.Collections.Generic;

namespace Bromine.Automation.Core.Models
{
    public class HeaderConfiguration
    {
        public List<HeaderSet> HeaderSets { get; set; }
        public List<HeaderProperty> HeaderProperties { get; set; }
    }

    //public class HeaderSet : BrowserHeader
    public class HeaderSet : BrowserHeader
    {
        public string Key { get; set; }
        public List<string> ExcludedHeaders { get; set; }
    }

    public class HeaderProperty
    {
        public string Key { get; set; }
        public string HeaderName { get; set; }
        public bool AutoLoaded { get; set; }
        public string DefaultAppConfigKey { get; set; }
    }

    public class RegionConfiguration
    {
        public string Key { get; set; }
        public string Ip { get; set; }
    }
    
    public class TestConfiguration
    {
        public List<RegionConfiguration> RegionConfiguration { get; set; }
        public HeaderConfiguration HeaderConfiguration { get; set; }
    }
}