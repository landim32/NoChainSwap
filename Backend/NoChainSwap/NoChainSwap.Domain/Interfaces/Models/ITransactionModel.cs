using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Interfaces.Models
{
    public interface ITransactionModel
    {
        long TxId { get; set; }
        CoinEnum SenderCoin { get; set; }
        CoinEnum ReceiverCoin { get; set; }
        string SenderAddress { get; set; }
        string ReceiverAddress { get; set; }
        DateTime CreateAt { get; set; }
        DateTime UpdateAt { get; set; }
        TransactionStatusEnum Status { get; set; }
        string SenderTxid { get; set; }
        string ReceiverTxid { get; set; }
        int? SenderFee { get; set; }
        int? ReceiverFee { get; set; }
        long? SenderAmount { get; set; }
        long? ReceiverAmount { get; set; }

        string GetSenderCoinSymbol();
        string GetReceiverCoinSymbol();

        ITransactionModel Save();
        ITransactionModel Update();
        ITransactionModel GetBySenderAddr(string senderAddr, ITransactionDomainFactory factory);
        ITransactionModel GetById(long txId, ITransactionDomainFactory factory);
        ITransactionModel GetBySenderTxId(string txid, ITransactionDomainFactory factory);
        ITransactionModel GetByReceiverTxId(string txid, ITransactionDomainFactory factory);
        IEnumerable<ITransactionModel> ListBySenderAddr(string senderAddr, ITransactionDomainFactory factory);
        IEnumerable<ITransactionModel> ListByStatus(IList<int> status, ITransactionDomainFactory factory);
        IEnumerable<ITransactionModel> ListAll(ITransactionDomainFactory factory);
    }
}
