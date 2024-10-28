using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.Transaction
{
    public class TransactionParamInfo
    {
        [JsonPropertyName("sendercoin")]
        public string SenderCoin { get; set; }
        [JsonPropertyName("receivercoin")]
        public string ReceiverCoin { get; set; }
        [JsonPropertyName("senderaddress")]
        public string SenderAddress { get; set; }
        [JsonPropertyName("receiveraddress")]
        public string ReceiverAddress { get; set; }
        [JsonPropertyName("sendertxid")]
        public string SenderTxid { get; set; }
    }
}
