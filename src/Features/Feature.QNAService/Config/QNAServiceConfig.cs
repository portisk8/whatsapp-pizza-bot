using Feature.Core.Config;
using Microsoft.Extensions.Configuration;

namespace Feature.QNAService.Config
{
    public class QNAServiceConfig : ConfigBase
    {
        public string KnowledgebaseId { get; set; }
        public string APIKey { get; set; }
        public string HostName { get; set; }
        public bool EnablePreciseAnswer { get; set; }
        public bool DisplayPreciseAnswerOnly { get; set; }

        public static QNAServiceConfig Build(IConfiguration configuration)
        {
            var prefix = "QNAService";
            var config = new QNAServiceConfig();
            configuration.GetSection(prefix).Bind(config);
            return config;
        }
    }
}
