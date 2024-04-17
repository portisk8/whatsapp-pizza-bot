using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pizza.Bot.Entities
{
    public class QNAResponse
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("images")]
        public List<QNAImages> Images { get; set; }
    }

    public class QNAImages
    {
        [JsonPropertyName("body")]
        public string Body { get; set; }
        [JsonPropertyName("media")]
        public string Media { get; set; }
    }
}
