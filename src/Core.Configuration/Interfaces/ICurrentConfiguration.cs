using Pizza.Core.Configuration.Modulos.Interfaces;

namespace Pizza.Core.Configuration.Interfaces
{
    public interface ICurrentConfiguration
    {
        IGeneralConfig General {get;set;}
        IBotConfig BotSettings { get; set; }
    }
}
