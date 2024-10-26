using NoChainSwap.Domain.Impl.Models;
using NoChainSwap.Domain.Interfaces.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Interfaces.Models
{
    public interface ITransactionLogModel
    {
        long LogId { get; set; }
        long TxId { get; set; }
        DateTime Date {  get; set; }
        LogTypeEnum LogType { get; set; }
        string Message { get; set; }

        ITransactionLogModel Insert();
        IEnumerable<ITransactionLogModel> ListById(long logId, ITransactionLogDomainFactory factory);
        IEnumerable<ITransactionLogModel> GetBySenderTx(string txId, ITransactionLogDomainFactory factory);
        IEnumerable<ITransactionLogModel> GetByReceiverTx(string txId, ITransactionLogDomainFactory factory);
    }
}
