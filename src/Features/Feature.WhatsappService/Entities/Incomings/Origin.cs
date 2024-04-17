using System.Text.Json.Serialization;

namespace WhatsApp.Adapters.WhatsAppAdapter.Incomings
{
    public class Origin
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}