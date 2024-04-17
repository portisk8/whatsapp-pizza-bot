using System.Text.Json.Serialization;

namespace WhatsApp.Adapters.WhatsAppAdapter.Incomings
{
    public class ContactProfile
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}