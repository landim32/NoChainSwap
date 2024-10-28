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
        [JsonProperty("senderproportion")]
        public decimal SenderProportion { get; set; }
        [JsonProperty("senderpooladdr")]
        public string SenderPoolAddr { get; set; }
        [JsonProperty("senderpoolbalance")]
        public long SenderPoolBalance { get; set; }
        [JsonProperty("sender")]
        public CoinInfo Sender { get; set; }
        [JsonProperty("receiverprice")]
        public decimal ReceiverPrice { get; set; }
        [JsonProperty("Receiverproportion")]
        public decimal ReceiverProportion { get; set; }
        [JsonProperty("receiverpooladdr")]
        public string ReceiverPoolAddr { get; set; }
        [JsonProperty("receiverpoolbalance")]
        public long ReceiverPoolBalance { get; set; }
        [JsonProperty("receiver")]
        public CoinInfo Receiver { get; set; }
    }
}
