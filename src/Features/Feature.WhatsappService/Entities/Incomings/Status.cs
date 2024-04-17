using System.Text.Json.Serialization;

namespace WhatsApp.Adapters.WhatsAppAdapter.Incomings
{
    public class Status
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("status")]
        public string StatusText { get; set; }
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
        [JsonPropertyName("recipient_id")]
        public string RecipientId { get; set; }
        [JsonPropertyName("conversation")]
        public Conversation Conversation { get; set; }
        [JsonPropertyName("pricing")]
        public Pricing Pricing { get; set; }

    }
}