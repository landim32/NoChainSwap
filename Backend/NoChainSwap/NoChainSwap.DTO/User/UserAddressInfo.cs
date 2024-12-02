using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.User
{
    public class UserAddressInfo
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("chainId")]
        public int ChainId { get; set; }
        [JsonPropertyName("createAt")]
        public DateTime CreateAt { get; set; }
        [JsonPropertyName("updateAt")]
        public DateTime UpdateAt { get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; }
    }
}
