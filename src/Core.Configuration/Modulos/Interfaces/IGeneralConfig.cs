namespace Pizza.Core.Configuration.Modulos.Interfaces
{
    public interface IGeneralConfig
    {
        string ConnectionString { get; set; }

        string InformationalVersion { get; set; }

        string Version { get; set; }
    }
}
