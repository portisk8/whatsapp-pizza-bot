using System.Text.Json.Serialization;

namespace WhatsApp.Adapters.WhatsAppAdapter.Incomings
{
    public class Conversation
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("origin")]
        public Origin Origin { get; set; }
    }
}