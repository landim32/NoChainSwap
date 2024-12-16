using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.Transaction
{
    public class TxRevertStatusParam
    {
        [JsonProperty("txId")]
        public long TxId { get; set; }
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
