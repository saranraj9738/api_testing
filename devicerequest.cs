using System.Text.Json.Serialization;

namespace devicerequest
{
    public class DeviceRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public DeviceData Data { get; set; } = new DeviceData();
    }
    public class DeviceData
    {
        [JsonPropertyName("price")]
        public object Price { get; set; } = 0; 

    }
}
