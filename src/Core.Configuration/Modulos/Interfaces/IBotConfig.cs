namespace Pizza.Core.Configuration.Modulos.Interfaces
{
    public interface IBotConfig
    {
        int ConversationTimeoutInMinutes { get; set; }
        string BotName { get; set; }
        string CompanyName { get; set; }
        string WelcomeMessage { get; set; }
        string QnaDefaultNoAnswer { get; set; }
        string ContentRootPath { get; set; }
        string ErrorMessage { get; set; }
        string ApiUrl { get; set; }
    }
}
