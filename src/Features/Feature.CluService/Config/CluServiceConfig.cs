using Feature.Core.Config;
using Microsoft.Extensions.Configuration;

namespace Feature.CluService.Config
{
    public class CluServiceConfig : ConfigBase
    {
        public string ProjectName { get; set; }
        public string DeploymentName { get; set; }
        public string APIKey { get; set; }
        public string HostName { get; set; }
        public string Language { get; set; }

        public static CluServiceConfig Build(IConfiguration configuration)
        {
            var prefix = "CluService";
            var config = new CluServiceConfig();
            configuration.GetSection(prefix).Bind(config);
            return config;
        }
    }
}
