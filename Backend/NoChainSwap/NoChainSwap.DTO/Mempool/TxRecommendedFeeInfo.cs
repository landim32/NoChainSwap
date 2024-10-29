using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.Mempool
{
    public class TxRecommendedFeeInfo
    {
        [JsonProperty("fastestFee")]
        [JsonPropertyName("fastestFee")]
        public int FastestFee {  get; set; }
        [JsonProperty("halfHourFee")]
        [JsonPropertyName("halfHourFee")]
        public int HalfHourFee { get; set; }
        [JsonProperty("hourFee")]
        [JsonPropertyName("hourFee")]
        public int HourFee { get; set; }
        [JsonProperty("economyFee")]
        [JsonPropertyName("economyFee")]
        public int EconomyFee { get; set; }
        [JsonProperty("minimumFee")]
        [JsonPropertyName("minimumFee")]
        public int MinimumFee { get; set; }
    }
}
