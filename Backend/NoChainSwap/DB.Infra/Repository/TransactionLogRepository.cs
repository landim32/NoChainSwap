using NoChainSwap.Domain.Impl.Models;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using Core.Domain.Repository;
using DB.Infra.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infra.Repository
{
    public class TransactionLogRepository : ITransactionLogRepository<ITransactionLogModel, ITransactionLogDomainFactory>
    {
        private NoChainSwapContext _ccsContext;

        public TransactionLogRepository(NoChainSwapContext ccsContext)
        {
            _ccsContext = ccsContext;
        }

        private ITransactionLogModel DbToModel(ITransactionLogDomainFactory factory, TransactionLog u)
        {
            var md = factory.BuildTransactionLogModel();
            md.LogId = u.LogId;
            md.TxId = u.TxId;
            md.Date = u.Date;
            md.LogType = (LogTypeEnum)u.LogType;
            md.Message = u.Message;
            return md;
        }

        private void ModelToDb(ITransactionLogModel u, TransactionLog md)
        {
            md.LogId = u.LogId;
            md.TxId = u.TxId;
            md.Date = u.Date;
            md.LogType = (int)u.LogType;
            md.Message = u.Message;
        }

        public ITransactionLogModel Insert(ITransactionLogModel model)
        {
            try
            {
                var u = new TransactionLog();
                ModelToDb(model, u);

                _ccsContext.Add(u);
                _ccsContext.SaveChanges();
                model.TxId = u.TxId;
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<ITransactionLogModel> ListById(long txId, ITransactionLogDomainFactory factory)
        {
            var rows = _ccsContext.TransactionLogs.Where(x => x.TxId == txId).OrderByDescending(x => x.Date).ToList();
            return rows.Select(x => DbToModel(factory, x));
        }

        public IEnumerable<ITransactionLogModel> ListBySenderTx(string txId, ITransactionLogDomainFactory factory)
        {
            var rows = _ccsContext.TransactionLogs.Where(x => x.Tx.SenderTxid == txId).OrderByDescending(x => x.Date).ToList();
            return rows.Select(x => DbToModel(factory, x));
        }

        public IEnumerable<ITransactionLogModel> ListByReceiverTx(string txId, ITransactionLogDomainFactory factory)
        {
            var rows = _ccsContext.TransactionLogs.Where(x => x.Tx.ReceiverTxid == txId).OrderByDescending(x => x.Date).ToList();
            return rows.Select(x => DbToModel(factory, x));
        }
    }
}
