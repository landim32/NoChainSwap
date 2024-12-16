using System;
using System.Collections;
using System.Collections.Generic;
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
        [JsonPropertyName("isAdmin")]
        public bool IsAdmin { get; set; }
        [JsonPropertyName("createAt")]
        public DateTime CreateAt { get; set; }
        [JsonPropertyName("updateAt")]
        public DateTime UpdateAt { get; set; }
    }
}
