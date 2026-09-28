using System.Text.Json.Serialization;

namespace listrequest
{
    public class DeviceResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public DeviceResponseData? Data { get; set; }
    }

    public class DeviceResponseData
    {
        [JsonPropertyName("price")]
        public object? Price { get; set; }
    }
}
