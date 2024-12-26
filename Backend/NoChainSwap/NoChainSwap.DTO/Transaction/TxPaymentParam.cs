using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.Transaction
{
    public class TxPaymentParam
    {
        [JsonPropertyName("txId")]
        public long TxId { get; set; }
        [JsonPropertyName("senderTxId")]
        public string SenderTxId { get; set; }
    }
}
