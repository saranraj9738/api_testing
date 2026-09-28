using System.Text.Json.Serialization;

namespace Response
{
    public class Deviceresponse
    {
        [JsonPropertyName("id")]
        public object Id { get; set; } = string.Empty; 


        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public object Price { get; set; } = 0; 
    }
}
