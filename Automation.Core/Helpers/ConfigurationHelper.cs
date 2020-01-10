using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bromine.Automation.Core.Helpers
{
    public static class ConfigurationHelper
    {
        private static readonly IConfigurationRoot Config;
       
        static ConfigurationHelper()
        {
            ReadEnvironmentVariables();
            Config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("App_Data\\Configs\\appSettings.json", false, true)
                .AddJsonFile($"App_Data\\Configs\\appSettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            if (Config == null) throw new FileNotFoundException("Common config not found");


        }

        public static T GetConfig<T>(string key)
        {
            var configValue = GetConfig(key);
            return string.IsNullOrEmpty(configValue) ? default(T) : (T)Convert.ChangeType(configValue, typeof(T));
        }

        public static string GetConfig(string key)
        {
            return Config.GetSection($"appSettings:{key}").Value;
        }

        public static IConfigurationSection GetSection(string key)
        {
            return Config.GetSection(key);
        }

        public static T GetSection<T>(string key)
        {
            var configValue = GetSection(key).Get(typeof(T));
            return configValue == null ? default(T) : (T)configValue;
        }


        private static void ReadEnvironmentVariables()
        {
            using var launchSettings = File.OpenText("Properties\\launchSettings.json");
            var reader = new JsonTextReader(launchSettings);
            var jObject = JObject.Load(reader);

            var variables = jObject
                .GetValue("profiles")
                //select a proper profile here
                .SelectMany(profiles => profiles.Children())
                .SelectMany(profile => profile.Children<JProperty>())
                .Where(prop => prop.Name == "environmentVariables")
                .SelectMany(prop => prop.Value.Children<JProperty>())
                .ToList();

            foreach (var variable in variables)
            {
                Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
            }
        }
    }
}
