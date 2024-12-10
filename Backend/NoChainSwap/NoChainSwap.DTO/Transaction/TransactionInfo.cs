using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.Transaction
{
    public class TransactionInfo
    {
        public long TxId { get; set; }
        public CoinEnum SenderCoin { get; set; }
        public CoinEnum ReceiverCoin { get; set; }
        public string RecipientAddress { get; set; }
        public string SenderAddress { get; set; }
        public string ReceiverAddress { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public TransactionStatusEnum Status { get; set; }
        public string SenderTxid { get; set; }
        public string ReceiverTxid { get; set; }
        public int? SenderFee { get; set; }
        public int? ReceiverFee { get; set; }
        public long? SenderAmount { get; set; }
        public long? ReceiverAmount { get; set; }
    }
}
