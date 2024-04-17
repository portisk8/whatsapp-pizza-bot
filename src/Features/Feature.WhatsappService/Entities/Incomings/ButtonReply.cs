using System.Text.Json.Serialization;

namespace Whatsapp.Adapter.WhatsAppAdapter.Incomings
{
    public class ButtonReply
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}