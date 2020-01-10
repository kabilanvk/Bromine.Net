using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Bromine.Automation.Core.Common;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace Bromine.SeleniumWrapper.Profiles
{
    internal class FirefoxDriverSetup
    {

        internal static FirefoxDriverService SetupFirefoxDriverService()
        {
            var driverService = FirefoxDriverService.CreateDefaultService(Path.Combine(Directory.GetCurrentDirectory(), Constants.AppConfig.GeckoDriverDir));
            // Hack until selenium fixes the firefox driver for .Net core
            driverService.Port = GetAvailablePort();
            driverService.Start();
            // End of Hack
            return driverService;
        }

        private static int GetAvailablePort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }

    internal class FirefoxDriver : RemoteWebDriver
    {
        //public FirefoxDriver(FirefoxDriverService service, FirefoxOptions options, TimeSpan commandTimeout)
        //    : base(service, options, commandTimeout)
        //{
        //    CommandExecutor.CommandInfoRepository.TryAddCommand("moz-install-web-ext",
        //        new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/moz/addon/install"));
        //}

        // Hack until selenium fixes the firefox driver for .Net core
        public FirefoxDriver(Uri uri, FirefoxOptions options)
            : base(uri, options)
        {
            CommandExecutor.CommandInfoRepository.TryAddCommand("moz-install-web-ext",
                new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/moz/addon/install"));
        }
        // End of hack

        public void InstallWebExtension(string path)
        {
            if (!File.Exists(path)) return;
            var extLocation = Path.GetFullPath(path);
            var param = new Dictionary<string, object> { { "path", extLocation }, { "temporary", true } };
            Execute("moz-install-web-ext", param);
        }
    }

}
