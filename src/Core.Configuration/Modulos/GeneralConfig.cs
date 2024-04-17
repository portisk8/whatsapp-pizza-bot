using Microsoft.Extensions.Configuration;
using Pizza.Core.Configuration.Modulos.Interfaces;
using System.Reflection;

namespace Pizza.Core.Configuration.Modulos
{
    public class GeneralConfig : IGeneralConfig
    {
        public string ConnectionString { get; set; }

        public string InformationalVersion { get; set; }

        public string Version { get; set; }

        public GeneralConfig()
        {
        }

        public static IGeneralConfig Build(IConfiguration configuration)
        {
            Assembly executingAssembly = Assembly.GetEntryAssembly();

            return new GeneralConfig()
            {
                ConnectionString = configuration["ConnectionStrings:DBConnectionString"],
                Version = executingAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version,
                InformationalVersion = executingAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion
            };
        }
    }
}