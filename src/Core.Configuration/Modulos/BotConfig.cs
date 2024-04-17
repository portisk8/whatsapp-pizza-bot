using Microsoft.Extensions.Configuration;
using Pizza.Core.Configuration.Modulos.Interfaces;

namespace Pizza.Core.Configuration.Modulos
{
    public class BotConfig : IBotConfig
    {
        public int ConversationTimeoutInMinutes { get; set; } = 5;
        public string BotName { get; set; }
        public string CompanyName { get; set; }
        public string WelcomeMessage { get; set; }
        public string QnaDefaultNoAnswer { get; set; }
        public string ContentRootPath { get; set; }
        public string ErrorMessage { get; set; }
        public string ApiUrl { get; set; }


        public static IBotConfig Build(IConfiguration configuration)
        {
            return new BotConfig()
            {
                ConversationTimeoutInMinutes = int.Parse(configuration["Bot:ConversationTimeoutInMinutes"]),
                BotName = configuration["Bot:BotName"],
                CompanyName = configuration["Bot:CompanyName"],
                WelcomeMessage = configuration["Bot:WelcomeMessage"],
                QnaDefaultNoAnswer = configuration["Bot:QnaDefaultNoAnswer"],
                ContentRootPath = configuration["Bot:ContentRootPath"],
                ErrorMessage = configuration["Bot:ErrorMessage"],
                ApiUrl = configuration["Bot:ApiUrl"]
                

            };
        }
    }
}
