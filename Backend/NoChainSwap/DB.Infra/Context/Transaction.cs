using System;
using System.Collections.Generic;

namespace DB.Infra.Context;

public partial class Transaction
{
    public long TxId { get; set; }

    public string SenderAddress { get; set; }

    public string ReceiverAddress { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public int Status { get; set; }

    public string SenderTxid { get; set; }

    public string ReceiverTxid { get; set; }

    public int? SenderFee { get; set; }

    public int? ReceiverFee { get; set; }

    public long SenderAmount { get; set; }

    public long ReceiverAmount { get; set; }

    public string SenderCoin { get; set; }

    public string ReceiverCoin { get; set; }

    public long UserId { get; set; }

    public int? ChainId { get; set; }

    public string RecipientAddress { get; set; }

    public long? SenderTax { get; set; }

    public long? ReceiverTax { get; set; }

    public string Hash { get; set; }

    public virtual ICollection<TransactionLog> TransactionLogs { get; set; } = new List<TransactionLog>();

    public virtual User User { get; set; }
}
