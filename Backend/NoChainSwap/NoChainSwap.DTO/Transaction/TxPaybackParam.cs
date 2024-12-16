using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.Transaction
{
    public class TxPaybackParam
    {
        [JsonPropertyName("txId")]
        public long TxId { get; set; }
        [JsonPropertyName("receiverTxId")]
        public string ReceiverTxId { get; set; }
        [JsonPropertyName("receiverFee")]
        public int ReceiverFee { get; set; }
    }
}
