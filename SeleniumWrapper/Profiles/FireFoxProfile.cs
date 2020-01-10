using System.Collections.Generic;
using Bromine.Automation.Core.Common;
using OpenQA.Selenium.Firefox;

namespace Bromine.SeleniumWrapper.Profiles
{
    public class FireFoxProfile : FirefoxProfile
    {
        public FireFoxProfile(Dictionary<string, string> headers, string language)
        {
            Initialize(headers, language);
        }

        private void Initialize(Dictionary<string, string> headers, string language)
        {
            //set no proxy for firefox
            SetPreference("network.proxy.type", 0);
            SetPreference("security.cert_pinning.enforcement_level", 0);
            SetPreference("security.ssl.warn_missing_rfc5746", 0);
            SetPreference("security.OCSP.enabled", 0);
            SetPreference("security.ssl.enable_ocsp_stapling", false);
            SetPreference("devtools.webconsole.persistlog", true);
            SetPreference("devtools.toolbox.host", "left");
            SetPreference("devtools.theme", "dark");
            SetPreference("devtools.browserconsole.filter.jswarn", false);
            SetPreference("devtools.webconsole.filter.jswarn", false);
            SetPreference("devtools.webconsole.filter.warn", false);
            SetPreference("devtools.browserconsole.filter.secwarn", false);
            SetPreference("devtools.webconsole.filter.secwarn", false);
            SetPreference("network.warnOnAboutNetworking", false);
            SetPreference("capability.policy.default.Window.QueryInterface", "allAccess");
            SetPreference("capability.policy.default.Window.frameElement.get", "allAc‌​cess");
            SetPreference("dom.allow_scripts_to_close_windows", true);
            SetPreference("marionette", true);
            SetPreference("intl.accept_languages", language);
            SetPreference("devtools.toolbox.selectedTool", "webconsole");
            // Adding http headers
            if (headers == null || headers.Count <= 0) return;
            if (headers.ContainsKey(Constants.UserAgent))
            {
                SetPreference("general.useragent.override", headers[Constants.UserAgent]);
            }
        }
    }
}
