using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WhatsApp.Adapters.WhatsAppAdapter.Incomings
{
    public class WhatsAppIncomingRequest
    {

        [JsonPropertyName("object")]
        public string Object { get; set; }
        [JsonPropertyName("entry")]
        public List<Entry> Entries { get; set; }

    }
}
