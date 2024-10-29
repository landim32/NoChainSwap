using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.Stacks
{
    public class TxInfo
    {
        [JsonProperty("tx_id")]
        public string TxId {  get; set; }
        [JsonProperty("nonce")]
        public long Nonce { get; set; }
        [JsonProperty("fee_rate")]
        public string FeeRate { get; set; }
        [JsonProperty("sender_address")]
        public string SenderAddress { get; set; }
        [JsonProperty("sponsor_nonce")]
        public long SponsorNonce { get; set; }
        [JsonProperty("sponsored")]
        public bool Sponsored { get; set; }
        [JsonProperty("sponsor_address")]
        public string SponsorAddress {  get; set; }
        [JsonProperty("post_condition_mode")]
        public string PostConditionMode { get; set; }
        [JsonProperty("post_conditions")]
        public IList<TxPostConditionInfo> PostConditions { get; set; }
        [JsonProperty("anchor_mode")]
        public string AnchorMode { get; set; }
        [JsonProperty("block_hash")]
        public string BlockHash { get; set; }
        [JsonProperty("block_height")]
        public long BlockHeight { get; set; }
        [JsonProperty("block_time")]
        public long BlockTime { get; set; }
        [JsonProperty("block_time_iso")]
        public string BlockTimeIso { get; set; }
        [JsonProperty("burn_block_height")]
        public long BurnBlockHeight { get; set; }
        [JsonProperty("burn_block_time")]
        public long BurnBlockTime {  get; set; }
        [JsonProperty("burn_block_time_iso")]
        public string BurnBlockTimeIso { get; set; }
        [JsonProperty("parent_burn_block_time")]
        public long ParentBurnBlockTime { get; set; }
        [JsonProperty("parent_burn_block_time_iso")]
        public string ParentBurnBlockTimeIso { get; set; }
        [JsonProperty("canonical")]
        public bool Canonical {  get; set; }
        [JsonProperty("tx_index")]
        public long TxIndex { get; set; }
        [JsonProperty("tx_status")]
        public string TxStatus { get; set; }
        [JsonProperty("tx_result")]
        public TxResultInfo TxResult {  get; set; }
        [JsonProperty("event_count")]
        public long EventCount { get; set; }
        [JsonProperty("parent_block_hash")]
        public string ParentBlockHash { get; set; }
        [JsonProperty("is_unanchored")]
        public bool IsUnanchored {  get; set; }
        [JsonProperty("microblock_hash")]
        public string MicroblockHash { get; set; }
        [JsonProperty("microblock_sequence")]
        public long MicroblockSequence { get; set; }
        [JsonProperty("microblock_canonical")]
        public bool MicroblockCanonical { get; set; }
        [JsonProperty("execution_cost_read_count")]
        public long ExecutionCostReadCount { get; set; }
        [JsonProperty("execution_cost_read_length")]
        public long ExecutionCostReadLength {  get; set; }
        [JsonProperty("execution_cost_runtime")]
        public long ExecutionCostRuntime {  get; set; }
        [JsonProperty("execution_cost_write_count")]
        public long ExecutionCostWriteCount {  get; set; }
        [JsonProperty("execution_cost_write_length")]
        public long ExecutionCostWriteLength { get; set; }
        [JsonProperty("events")]
        public IList<TxEventInfo> Events {  get; set; }
        [JsonProperty("tx_type")]
        public string TxType { get; set; }
        [JsonProperty("token_transfer")]
        public TxTokenTransferInfo TokenTransfer { get; set; }
    }
}
