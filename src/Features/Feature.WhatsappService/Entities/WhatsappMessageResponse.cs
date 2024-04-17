using System.Collections.Generic;
using System.Text.Json.Serialization;
using WhatsApp.Adapters.WhatsAppAdapter.Incomings;

namespace WhatsApp.Adapters.WhatsAppAdapter.Entities
{
    public class WhatsappMessageResponse
    {
        [JsonPropertyName("messaging_product")]
        public string MessagingProduct { get; set; }

        [JsonPropertyName("contacts")]
        public List<Contact> Contacts { get; set; }

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }
    }
}
