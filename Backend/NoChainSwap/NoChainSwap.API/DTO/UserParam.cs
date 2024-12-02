using System;
using System.Text.Json.Serialization;

namespace NoChainSwap.API.DTO
{
    public class UserParam
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }

    }
}
