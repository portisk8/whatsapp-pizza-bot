using Newtonsoft.Json;

namespace Feature.CluService.Clu
{
    public class CluEntity
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("confidenceScore")]
        public float ConfidenceScore { get; set; }

        [JsonProperty("extraInformation")]
        public List<ExtraInformation> ExtraInformation { get; set; }
    }
    public class ExtraInformation
    {
        [JsonProperty("extraInformationKind")]
        public string ExtraInformationKind { get; set; }

        [JsonProperty("key")]
        public string key { get; set; }
    }
}