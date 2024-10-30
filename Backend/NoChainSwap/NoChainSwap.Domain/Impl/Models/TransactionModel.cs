﻿using NoChainSwap.Domain.Interfaces.Factory;
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
        public CoinEnum SenderCoin { get; set; }
        public CoinEnum ReceiverCoin { get; set; }
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



        public string GetSenderCoinSymbol()
        {
            return Utils.CoinToStr(SenderCoin);
        }
        public string GetReceiverCoinSymbol()
        {
            return Utils.CoinToStr(ReceiverCoin);
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

        public ITransactionModel GetById(long txId, ITransactionDomainFactory factory)
        {
            return _repositoryTx.GetById(txId, factory);
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

        public ITransactionModel Save()
        {
            return _repositoryTx.SaveTx(this);
        }

        public ITransactionModel Update()
        {
            return _repositoryTx.UpdateTx(this);
        }
    }
}
