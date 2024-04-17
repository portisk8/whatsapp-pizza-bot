using System.Text.Json.Serialization;

namespace WhatsApp.Adapters.WhatsAppAdapter.Incomings
{
    public class MessageLocation
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}