using System.Text.Json.Serialization;

namespace WhatsApp.Adapters.WhatsAppAdapter.Incomings
{
    public class MessageContext
    {
        [JsonPropertyName("from")]
        public string From { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}