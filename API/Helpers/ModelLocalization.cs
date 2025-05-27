using System.Text.Json.Serialization;

namespace API.Helpers
{
    public class ModelLocalizaton
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("en")]
        public string En { get; set; }
    }
}
