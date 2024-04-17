using Feature.Core.Config;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature.WhatsappService.Config
{
    public class WhatsappConfig: ConfigBase
    {

        public string PhoneNumberId { get; set; }
        public string AccountId { get; set; }
        public string WhatsAppBusinessId { get; set; }
        public string AccessToken { get; set; }
        public string WhatsappURL { get; set; }
        public string WebHookToken { get; set; }
        public string AppName { get; set; }
        public string Version { get; set; }


        public static WhatsappConfig Build(IConfiguration configuration)
        {
            var prefix = "WhatsAppBusinessConfig";
            var config = new WhatsappConfig();
            configuration.GetSection(prefix).Bind(config);
            return config;
        }
    }
}
