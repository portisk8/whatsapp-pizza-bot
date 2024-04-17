using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WhatsApp.Adapters.WhatsAppAdapter.Incomings
{
    public class Change
    {
        [JsonPropertyName("field")]
        public string Field { get; set; }
        [JsonPropertyName("value")]
        public Value Value { get; set; }
    }
}
