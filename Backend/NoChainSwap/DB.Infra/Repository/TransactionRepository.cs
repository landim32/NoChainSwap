using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using NoChainSwap.DTO.Transaction;
using Core.Domain.Repository;
using DB.Infra.Context;
using NoobsMuc.Coinmarketcap.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoChainSwap.Domain.Impl.Core;
using System.Net;

namespace DB.Infra.Repository
{
    public class TransactionRepository : ITransactionRepository<ITransactionModel, ITransactionDomainFactory>
    {
        private NoChainSwapContext _ccsContext;

        public TransactionRepository(NoChainSwapContext ccsContext)
        {
            _ccsContext = ccsContext;
        }

        private ITransactionModel DbToModel(ITransactionDomainFactory factory, Transaction u)
        {
            var md = factory.BuildTransactionModel();
            md.TxId = u.TxId;
            md.SenderCoin = Utils.StrToCoin(u.SenderCoin);
            md.ReceiverCoin = Utils.StrToCoin(u.ReceiverCoin);
            md.SenderAddress = u.SenderAddress;
            md.ReceiverAddress = u.ReceiverAddress;
            md.CreateAt = u.CreateAt;
            md.UpdateAt = u.UpdateAt;
            md.Status = (TransactionStatusEnum)u.Status;
            md.SenderTxid = u.SenderTxid;
            md.ReceiverTxid = u.ReceiverTxid;
            md.SenderFee = u.SenderFee;
            md.ReceiverFee = u.ReceiverFee;
            md.SenderAmount = u.SenderAmount;
            md.ReceiverAmount = u.ReceiverAmount;
            return md;
        }

        private void ModelToDb(ITransactionModel u, Transaction md)
        {
            md.TxId = u.TxId;
            md.SenderCoin = Utils.CoinToStr(u.SenderCoin);
            md.ReceiverCoin = Utils.CoinToStr(u.ReceiverCoin);
            md.SenderAddress = u.SenderAddress;
            md.ReceiverAddress = u.ReceiverAddress;
            md.CreateAt = u.CreateAt;
            md.UpdateAt = u.UpdateAt;
            md.Status = (int)u.Status;
            md.SenderTxid = u.SenderTxid;
            md.ReceiverTxid = u.ReceiverTxid;
            md.SenderFee = u.SenderFee;
            md.ReceiverFee = u.ReceiverFee;
            md.SenderAmount = u.SenderAmount;
            md.ReceiverAmount = u.ReceiverAmount;
        }

        public ITransactionModel GetBySenderAddr(string senderAddr, ITransactionDomainFactory factory)
        {
            try
            {
                var row = _ccsContext.Transactions.Where(x => x.SenderAddress == senderAddr).FirstOrDefault();
                if (row != null)
                {
                    return DbToModel(factory, row);
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ITransactionModel GetById(long txId, ITransactionDomainFactory factory)
        {
            try
            {
                var row = _ccsContext.Transactions.Where(x => x.TxId == txId).FirstOrDefault();
                if (row != null)
                {
                    return DbToModel(factory, row);
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<ITransactionModel> ListByUser(long userId, ITransactionDomainFactory factory)
        {
            var rows = _ccsContext.Transactions.Where(x => x.UserId == userId).ToList();
            return rows.Select(x => DbToModel(factory, x));
        }

        public IEnumerable<ITransactionModel> ListByAddress(string address, ITransactionDomainFactory factory)
        {
            var rows = _ccsContext.Transactions.Where(x => x.SenderAddress == address || x.ReceiverAddress == address).ToList();
            return rows.Select(x => DbToModel(factory, x));
        }

        public IEnumerable<ITransactionModel> ListByStatus(IList<int> status, ITransactionDomainFactory factory)
        {
            var rows = _ccsContext.Transactions.Where(x => status.Contains(x.Status)).OrderBy(x => x.CreateAt).ToList();
            return rows.Select(x => DbToModel(factory, x));
        }

        public IEnumerable<ITransactionModel> ListAll(ITransactionDomainFactory factory)
        {
            var rows = _ccsContext.Transactions.OrderByDescending(x => x.UpdateAt).Take(100).ToList();
            return rows.Select(x => DbToModel(factory, x));
        }

        public ITransactionModel SaveTx(ITransactionModel model)
        {
            try
            {
                var u = new Transaction();
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

        public ITransactionModel UpdateTx(ITransactionModel model)
        {
            try
            {
                var row = _ccsContext.Transactions.Where(x => x.TxId == model.TxId).FirstOrDefault();
                ModelToDb(model, row);
                _ccsContext.Transactions.Update(row);
                _ccsContext.SaveChanges();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ITransactionModel GetBySenderTxId(string txid, ITransactionDomainFactory factory)
        {
            try
            {
                var row = _ccsContext.Transactions.Where(x => x.SenderTxid == txid).FirstOrDefault();
                if (row != null)
                {
                    return DbToModel(factory, row);
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ITransactionModel GetByReceiverTxId(string txid, ITransactionDomainFactory factory)
        {
            try
            {
                var row = _ccsContext.Transactions.Where(x => x.ReceiverTxid == txid).FirstOrDefault();
                if (row != null)
                {
                    return DbToModel(factory, row);
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
