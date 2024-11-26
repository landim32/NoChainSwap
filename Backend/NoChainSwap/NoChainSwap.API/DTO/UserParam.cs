using System;
using System.Text.Json.Serialization;

namespace NoChainSwap.API.DTO
{
    public class UserParam
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("chainId")]
        public int ChainId { get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; }

    }
}
