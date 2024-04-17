using Microsoft.Extensions.Configuration;
using Pizza.Core.Configuration.Interfaces;
using Pizza.Core.Configuration.Modulos;
using Pizza.Core.Configuration.Modulos.Interfaces;

namespace Pizza.Core.Configuration
{
    public class CurrentConfiguration : ICurrentConfiguration
    {
        private static CurrentConfiguration _uniqueInstance;
        public IGeneralConfig General { get; set; }
        public IBotConfig BotSettings { get; set; }

        public CurrentConfiguration()
        {
        }

        public static ICurrentConfiguration Build(IConfiguration configuration)
        {

            _uniqueInstance = new CurrentConfiguration
            {
                General = GeneralConfig.Build(configuration),
                BotSettings = BotConfig.Build(configuration)
            };
            return _uniqueInstance;
        }
        public static CurrentConfiguration GetInstance()
        {
            return _uniqueInstance;
        }
    }
}