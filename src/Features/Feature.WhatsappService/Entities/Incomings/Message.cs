using System.Text.Json.Serialization;
using Whatsapp.Adapter.WhatsAppAdapter.Incomings;

namespace WhatsApp.Adapters.WhatsAppAdapter.Incomings
{
    public class Message
    {
        [JsonPropertyName("from")]
        public string From { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("context")]
        public MessageContext Context { get; set; }

        [JsonPropertyName("text")]
        public MessageText Text { get; set; }

        [JsonPropertyName("location")]
        public MessageLocation Location { get; set; }

        [JsonPropertyName("image")]
        public MessageImage Image { get; set; }

        [JsonPropertyName("interactive")]
        public MessageInteractive Interactive { get; set; }

        //[JsonPropertyName("location")]
        //public MessageLocation Location { get; set; }
    }
}