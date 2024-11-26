using System;
using System.Text.Json.Serialization;

namespace NoChainSwap.DTO.User
{
    public class UserInfo
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("hash")]
        public string Hash { get; set; }
        [JsonPropertyName("chain_id")]
        public int ChainId {  get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; }
    }
}
