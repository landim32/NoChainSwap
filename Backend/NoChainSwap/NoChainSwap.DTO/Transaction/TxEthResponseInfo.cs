using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.Transaction
{
    public class TxEthResponseInfo
    {
        [JsonProperty("status")]
        public int Status {  get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("result")]
        public IList<TxEthInfo> Result { get; set; }
    }
}
