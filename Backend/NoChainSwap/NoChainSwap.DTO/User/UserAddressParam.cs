using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.User
{
    public class UserAddressParam
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("chainId")]
        public int ChainId { get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; }
    }
}
