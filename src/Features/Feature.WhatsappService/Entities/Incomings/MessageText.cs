using System.Text.Json.Serialization;

namespace WhatsApp.Adapters.WhatsAppAdapter.Incomings
{
    public class MessageText
    {
        [JsonPropertyName("body")]
        public string Text { get; set; }
    }
}