using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WhatsApp.Adapters.WhatsAppAdapter.Incomings
{
    public class Entry
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("changes")]
        public List<Change> Changes { get; set; }
    }
}
