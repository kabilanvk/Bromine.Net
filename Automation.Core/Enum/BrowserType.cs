using System;
using Bromine.Automation.Core.Helpers;

namespace Bromine.Automation.Core.Enum
{
    [Flags]
    public enum BrowserType
    {
        [Value("Unknown")]
        Unknown = 0,
        [Value("FF")]
        FireFox = 1,
        [Value("CR")]
        Chrome = 2,
        [Value("RemoteFirefox")]
        RemoteFirefox = 4,
        [Value("IE")]
        InternetExplorer = 8,
        [Value("SF")]
        Safari = 16,
        [Value("O")]
        Opera = 32,
        [Value("RemoteIE")]
        RemoteIE = 64,
        [Value("RemoteChrome")]
        RemoteChrome = 128,        
        [Value("PT")]
        PhantomJs = 256
    }
}
