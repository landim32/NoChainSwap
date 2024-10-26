    using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using Core.Domain.Repository;
using Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Impl.Models
{
    public class TransactionLogModel: ITransactionLogModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionLogRepository<ITransactionLogModel, ITransactionLogDomainFactory> _repositoryTxLog;

        public TransactionLogModel(IUnitOfWork unitOfWork, ITransactionLogRepository<ITransactionLogModel, ITransactionLogDomainFactory> repositoryTxLog)
        {
            _unitOfWork = unitOfWork;
            _repositoryTxLog = repositoryTxLog;
        }

        public long LogId { get; set; }
        public long TxId { get; set; }
        public DateTime Date { get; set; }
        public LogTypeEnum LogType { get; set; }
        public string Message { get; set; }

        public ITransactionLogModel Insert()
        {
            return _repositoryTxLog.Insert(this);
        }
        public IEnumerable<ITransactionLogModel> ListById(long logId, ITransactionLogDomainFactory factory)
        {
            return _repositoryTxLog.ListById(logId, factory);
        }
        public IEnumerable<ITransactionLogModel> GetBySenderTx(string txId, ITransactionLogDomainFactory factory)
        {
            return _repositoryTxLog.ListBySenderTx(txId, factory);
        }
        public IEnumerable<ITransactionLogModel> GetByReceiverTx(string txId, ITransactionLogDomainFactory factory)
        {
            return _repositoryTxLog.ListByReceiverTx(txId, factory);
        }
    }
}
