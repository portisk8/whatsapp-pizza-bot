using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feature.Core.Config
{
    public class GeneralConfig: ConfigBase
    {
        public string[] CorsUrls { get; set; }
        public string InformationalVersion { get; set; }
        public string Version { get; set; }

        public static GeneralConfig Build(IConfiguration configuration)
        {
            Assembly executingAssembly = Assembly.GetEntryAssembly();

            return new GeneralConfig()
            {
                CorsUrls = configuration["GeneralConfig:CorsUrls"]?.Split(";"),
                ConnectionString = configuration["GeneralConfig:ConnectionString"],
                Version = executingAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version,
                InformationalVersion = executingAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion
            };
        }
    }
}
