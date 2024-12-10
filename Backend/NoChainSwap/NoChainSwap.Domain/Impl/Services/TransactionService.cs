﻿using NBitcoin;
using NoChainSwap.Domain.Impl.Models;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using NoChainSwap.Domain.Interfaces.Services;
using NoChainSwap.Domain.Interfaces.Services.Coins;
using NoChainSwap.DTO.Mempool;
using NoChainSwap.DTO.Stacks;
using NoChainSwap.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Impl.Services
{
    public class TransactionService : ITransactionService
    {
        //protected readonly ICoinMarketCapService _coinMarketCapService;
        //protected readonly IMempoolService _mempoolService;
        //protected readonly IStacksService _stxService;
        protected readonly IUserService _userService;
        protected readonly ICoinTxServiceFactory _coinFactory;
        protected readonly ITransactionDomainFactory _txFactory;
        protected readonly ITransactionLogDomainFactory _txLogFactory;

        public TransactionService(
            //ICoinMarketCapService coinMarketCapService,
            //IMempoolService mempoolService,
            //IStacksService stxService,
            IUserService userService,
            ICoinTxServiceFactory coinFactory,
            ITransactionDomainFactory txFactory,
            ITransactionLogDomainFactory txLogFactory
        )
        {
            //_coinMarketCapService = coinMarketCapService;
            //_mempoolService = mempoolService;
            //_stxService = stxService;
            _userService = userService;
            _coinFactory = coinFactory;
            _txFactory = txFactory;
            _txLogFactory = txLogFactory;
        }

        public static string GetTransactionEnumToString(TransactionStatusEnum status)
        {
            string str = string.Empty;
            switch (status)
            {
                case TransactionStatusEnum.Initialized:
                    str = "Initialized";
                    break;
                case TransactionStatusEnum.Calculated:
                    str = "Calculated";
                    break;
                case TransactionStatusEnum.SenderNotConfirmed:
                    str = "Sender Not Confirmed";
                    break;
                case TransactionStatusEnum.SenderConfirmed:
                    str = "Sender Confirmed";
                    break;
                case TransactionStatusEnum.SenderConfirmedReiceiverNotConfirmed:
                    str = "Sender Confirmed, Receiver Not Confirmed";
                    break;
                case TransactionStatusEnum.Finished:
                    str = "Transaction finished";
                    break;
                case TransactionStatusEnum.InvalidInformation:
                    str = "Invalid Information";
                    break;
                case TransactionStatusEnum.CriticalError:
                    str = "Critical Error";
                    break;
            }
            return str;
        }

        public async Task<ITransactionModel> CreateTx(TransactionParamInfo param)
        {
            /*
            if (string.IsNullOrEmpty(param.SenderAddress))
            {
                throw new Exception($"Sender Address '{param.SenderAddress}' not informed");
            }
            if (string.IsNullOrEmpty(param.ReceiverAddress))
            {
                throw new Exception($"Receiver Address '{param.SenderAddress}' not informed");
            }
            */
            if (param.UserId > 0)
            {
                var user = _userService.GetUserByID(param.UserId);
                if (user == null) {
                    throw new Exception("User not found");
                }
            }
            else
            {
                throw new Exception("User not found");
            }
            if (!string.IsNullOrEmpty(param.SenderTxid))
            {
                var m1 = _txFactory.BuildTransactionModel().GetBySenderTxId(param.SenderTxid, _txFactory);
                if (m1 != null)
                {
                    throw new Exception($"Transaction '{param.SenderTxid}' is already registered");
                }
            }
            try
            {
                var model = _txFactory.BuildTransactionModel();
                model.UserId = param.UserId;
                //model.Type = param.BtcToStx ? TransactionEnum.BtcToStx : TransactionEnum.StxToBtc;
                model.SenderCoin = Core.Utils.StrToCoin(param.SenderCoin);
                model.ReceiverCoin = Core.Utils.StrToCoin(param.ReceiverCoin);
                //model.RecipientAddress = param.RecipientAddress;
                model.SenderAddress = param.SenderAddress;
                model.ReceiverAddress = param.ReceiverAddress;
                model.CreateAt = DateTime.Now;
                model.UpdateAt = DateTime.Now;
                model.Status = TransactionStatusEnum.Initialized;
                model.SenderAmount = param.SenderAmount;
                model.ReceiverAmount = param.ReceiverAmount;
                model.SenderTxid = param.SenderTxid;
                model.ReceiverTxid = null;
                model.SenderFee = null;
                model.ReceiverFee = null;

                if (model.SenderCoin == CoinEnum.BRL)
                {
                    double tax = Convert.ToDouble(model.SenderAmount) * 0.03;
                    model.SenderTax = Convert.ToInt64(Math.Truncate(tax));
                    model.ReceiverTax = 0;
                }
                else if (model.ReceiverCoin == CoinEnum.BRL)
                {
                    double tax = Convert.ToDouble(model.ReceiverAmount) * 0.03;
                    model.SenderTax = 0;
                    model.ReceiverTax = Convert.ToInt64(Math.Truncate(tax));
                }
                else
                {
                    double tax = Convert.ToDouble(model.SenderAmount) * 0.03;
                    model.SenderTax = Convert.ToInt64(Math.Truncate(tax));
                    model.ReceiverTax = 0;
                }

                model.Save();

                var senderService = _coinFactory.BuildCoinTxService(model.SenderCoin);
                var addr = await senderService.GetNewAddress(Convert.ToInt32(model.TxId));
                model.RecipientAddress = addr;
                model.Update();

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ITransactionModel GetTx(long txId)
        {
            try
            {
                return _txFactory.BuildTransactionModel().GetById(txId, _txFactory);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ITransactionModel Update(TransactionInfo tx)
        {
            try
            {
                var model = _txFactory.BuildTransactionModel().GetById(tx.TxId, _txFactory);
                if (model == null)
                {
                    throw new Exception("Transaction not found.");
                }
                model.SenderAddress = tx.SenderAddress;
                model.ReceiverAddress = tx.ReceiverAddress;
                model.UpdateAt = DateTime.Now;
                model.Status = tx.Status;
                model.SenderAmount = tx.SenderAmount;
                model.ReceiverAmount = tx.ReceiverAmount;
                model.SenderTxid = tx.SenderTxid;
                model.ReceiverTxid = tx.ReceiverTxid;
                model.SenderFee = tx.SenderFee;
                model.ReceiverFee = tx.ReceiverFee;
                return model.Update();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<ITransactionModel> ListByStatusActive()
        {
            var status = new List<int>() {
                (int) TransactionStatusEnum.Initialized,
                (int) TransactionStatusEnum.Calculated,
                (int) TransactionStatusEnum.WaitingSenderPayment,
                (int) TransactionStatusEnum.SenderNotConfirmed,
                (int) TransactionStatusEnum.SenderConfirmed,
                (int) TransactionStatusEnum.SenderConfirmedReiceiverNotConfirmed
            };
            return _txFactory.BuildTransactionModel().ListByStatus(status, _txFactory);
        }

        public IEnumerable<ITransactionModel> ListAll()
        {
            return _txFactory.BuildTransactionModel().ListAll(_txFactory);
        }

        public IEnumerable<ITransactionModel> ListByAddress(string senderAddr)
        {
            return _txFactory.BuildTransactionModel().ListByAddress(senderAddr, _txFactory);
        }

        public IEnumerable<ITransactionLogModel> ListLogById(long txid)
        {
            return _txLogFactory.BuildTransactionLogModel().ListById(txid, _txLogFactory);
        }

        public async Task<bool> ProcessAllTransaction()
        {
            foreach (var tx in ListByStatusActive())
            {
                await ProcessTransaction(tx);
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> ProcessTransaction(ITransactionModel tx)
        {
            if (tx == null)
            {
                throw new Exception("Transaction is null");
            }
            var senderService = _coinFactory.BuildCoinTxService(tx.SenderCoin);
            var receiverService = _coinFactory.BuildCoinTxService(tx.ReceiverCoin);
            if (senderService == null || receiverService == null)
            {
                throw new Exception("Transaction not suported");
            }
            try
            {
                return await senderService.ProcessTransaction(tx, receiverService);
            }
            catch (Exception err)
            {
                senderService.AddLog(tx.TxId, err.Message, LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.CriticalError;
                tx.Update();
            }
            return await Task.FromResult(false);
        }
    }
}
