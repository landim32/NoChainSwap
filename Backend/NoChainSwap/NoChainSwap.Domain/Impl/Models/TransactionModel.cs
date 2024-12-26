using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using NoChainSwap.DTO.Transaction;
using Core.Domain;
using Core.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoChainSwap.Domain.Impl.Core;

namespace NoChainSwap.Domain.Impl.Models
{
    public class TransactionModel : ITransactionModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionRepository<ITransactionModel, ITransactionDomainFactory> _repositoryTx;

        public TransactionModel(IUnitOfWork unitOfWork, ITransactionRepository<ITransactionModel, ITransactionDomainFactory> repositoryTx)
        {
            _unitOfWork = unitOfWork;
            _repositoryTx = repositoryTx;
        }

        public long TxId { get; set; }
        public long UserId { get; set; }
        public string Hash { get; set; }
        public ChainEnum Chain { get; set; }
        public CoinEnum SenderCoin { get; set; }
        public CoinEnum ReceiverCoin { get; set; }
        public string RecipientAddress { get; set; }
        public string SenderAddress { get; set; }
        public string ReceiverAddress { get; set; }
        public long? SenderTax { get; set; }
        public long? ReceiverTax { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public TransactionStatusEnum Status { get; set; }
        public string SenderTxid { get; set; }
        public string ReceiverTxid { get; set; }
        public long? SenderFee { get; set; }
        public long? ReceiverFee { get; set; }
        public long SenderAmount { get; set; }
        public long ReceiverAmount { get; set; }



        public string GetSenderCoinSymbol()
        {
            return Utils.CoinToStr(SenderCoin);
        }
        public string GetReceiverCoinSymbol()
        {
            return Utils.CoinToStr(ReceiverCoin);
        }

        public IUserModel GetUser(IUserDomainFactory factory)
        {
            if (UserId > 0)
            {
                return factory.BuildUserModel().GetById(this.UserId, factory);
            }
            return null;
        }

        public ITransactionModel GetBySenderTxId(string txid, ITransactionDomainFactory factory)
        {
            return _repositoryTx.GetBySenderTxId(txid, factory);
        }
        public ITransactionModel GetByReceiverTxId(string txid, ITransactionDomainFactory factory)
        {
            return _repositoryTx.GetByReceiverTxId(txid, factory);
        }
        public ITransactionModel GetBySenderAddr(string senderAddr, ITransactionDomainFactory factory)
        {
            return _repositoryTx.GetBySenderAddr(senderAddr, factory);
        }

        public ITransactionModel GetByRecipientAddr(string recipientAddr, ITransactionDomainFactory factory)
        {
            return _repositoryTx.GetByRecipientAddr(recipientAddr, factory);
        }

        public ITransactionModel GetById(long txId, ITransactionDomainFactory factory)
        {
            return _repositoryTx.GetById(txId, factory);
        }

        public ITransactionModel GetByHash(string hash, ITransactionDomainFactory factory)
        {
            return _repositoryTx.GetByHash(hash, factory);
        }

        public IEnumerable<ITransactionModel> ListByAddress(string address, ITransactionDomainFactory factory)
        {
            return _repositoryTx.ListByAddress(address, factory);
        }

        public IEnumerable<ITransactionModel> ListByStatus(IList<int> status, ITransactionDomainFactory factory)
        {
            return _repositoryTx.ListByStatus(status, factory);
        }

        public IEnumerable<ITransactionModel> ListAll(ITransactionDomainFactory factory)
        {
            return _repositoryTx.ListAll(factory);
        }

        public IEnumerable<ITransactionModel> ListByUser(long userId, ITransactionDomainFactory factory)
        {
            return _repositoryTx.ListByUser(userId, factory);
        }

        public ITransactionModel Save()
        {
            return _repositoryTx.SaveTx(this);
        }

        public ITransactionModel Update()
        {
            return _repositoryTx.UpdateTx(this);
        }

        public IEnumerable<ITransactionModel> ListToDetect(ITransactionDomainFactory factory)
        {
            return _repositoryTx.ListToDetect(factory);
        }
    }
}
