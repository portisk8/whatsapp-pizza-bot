using System.Text.Json.Serialization;

namespace WhatsApp.Adapters.WhatsAppAdapter.Incomings
{
    public class Metadata
    {
        [JsonPropertyName("display_phone_number")]
        public string DisplayPhoneNumber { get; set; }
        [JsonPropertyName("phone_number_id")]
        public string PhoneNumberId { get; set; }
    }
}