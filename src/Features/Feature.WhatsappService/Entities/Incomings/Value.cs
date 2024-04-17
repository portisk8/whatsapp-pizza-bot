using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WhatsApp.Adapters.WhatsAppAdapter.Incomings
{
    public class Value
    {
        [JsonPropertyName("messaging_product")]
        public string MessagingProduct { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }

        [JsonPropertyName("contacts")]
        public List<Contact> Contacts { get; set; }

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }

        [JsonPropertyName("statuses")]
        public List<Status> Statuses { get; set; }
    }
}