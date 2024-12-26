using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.Transaction
{
    public class TxEthInfo
    {
        [JsonProperty("blockNumber")]
        public long BlockNumber { get; set; }
        [JsonProperty("timeStamp")]
        public long TimeStamp { get; set; }
        [JsonProperty("hash")]
        public string Hash { get; set; }
        [JsonProperty("nonce")]
        public int nonce { get; set; }
        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }
        [JsonProperty("from")]
        public string From { get; set; }
        [JsonProperty("to")]
        public string To { get; set; }
        [JsonProperty("contractAddress")]
        public string ContractAddress { get; set; }
        [JsonProperty("value")]
        public BigInteger Value { get; set; }
        [JsonProperty("tokenName")]
        public string TokenName { get; set; }
        [JsonProperty("tokenSymbol")]
        public string TokenSymbol { get; set; }
        [JsonProperty("tokenDecimal")]
        public int TokenDecimal { get; set; }
        [JsonProperty("transactionIndex")]
        public int TransactionIndex { get; set; }
        [JsonProperty("gas")]
        public int Gas { get; set; }
        [JsonProperty("gasPrice")]
        public long GasPrice { get; set; }
        [JsonProperty("gasUsed")]
        public long GasUsed { get; set; }
        [JsonProperty("cumulativeGasUsed")]
        public long CumulativeGasUsed { get; set; }
        [JsonProperty("input")]
        public string Input { get; set; }
        [JsonProperty("Confirmations")]
        public long Confirmations { get; set; }
    }
}
