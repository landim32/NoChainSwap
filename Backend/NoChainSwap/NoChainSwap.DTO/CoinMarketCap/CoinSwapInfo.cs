using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.CoinMarketCap
{
    public class CoinSwapInfo
    {
        [JsonProperty("senderprice")]
        public decimal SenderPrice { get; set; }
        [JsonProperty("receiverprice")]
        public decimal ReceiverPrice { get; set; }
        [JsonProperty("senderproportion")]
        public decimal SenderProportion { get; set; }
        [JsonProperty("Receiverproportion")]
        public decimal ReceiverProportion { get; set; }
        [JsonProperty("original")]
        public CoinInfo Sender { get; set; }
        [JsonProperty("destiny")]
        public CoinInfo Receiver { get; set; }
    }
}
