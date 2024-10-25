using NBitcoin;
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
    public class TransactionService : BaseTransactionService, ITransactionService
    {
        private readonly ICoinMarketCapService _coinMarketCapService;
        private readonly IMempoolService _mempoolService;
        private readonly IBitcoinService _btcService;
        private readonly IStacksService _stxService;
        private readonly IBtcTxService _btcTxService;
        private readonly IStxTxService _stxTxService;
        private readonly ITransactionDomainFactory _txFactory;
        private readonly ITransactionLogDomainFactory _txLogFactory;

        public TransactionService(
            ICoinMarketCapService coinMarketCapService,
            IMempoolService mempoolService,
            IBitcoinService btcService,
            IStacksService stxService,
            IBtcTxService btcTxService,
            IStxTxService stxTxService,
            ITransactionDomainFactory txFactory,
            ITransactionLogDomainFactory txLogFactory
        )
        {
            _coinMarketCapService = coinMarketCapService;
            _mempoolService = mempoolService;
            _btcService = btcService;
            _stxService = stxService;
            _btcTxService = btcTxService;
            _stxTxService = stxTxService;
            _txFactory = txFactory;
            _txLogFactory = txLogFactory;
        }

        public ITransactionModel CreateTx(TransactionParamInfo param)
        {
            if (!string.IsNullOrEmpty(param.BtcAddress))
            {
                param.BtcAddress = param.BtcAddress.ToLower();
            }
            if (!string.IsNullOrEmpty(param.StxAddress))
            {
                param.StxAddress = param.StxAddress.ToUpper();
            }
            if (!string.IsNullOrEmpty(param.BtcTxid))
            {
                param.BtcTxid = param.BtcTxid.ToLower();
                var m1 = _txFactory.BuildTransactionModel().GetByBtcTxId(param.BtcTxid, _txFactory);
                if (m1 != null)
                {
                    throw new Exception($"Transaction '{param.BtcTxid}' is already registered");
                }
            }
            if (!string.IsNullOrEmpty(param.StxTxid))
            {
                if (param.StxTxid.StartsWith("0x") || param.StxTxid.StartsWith("0X"))
                {
                    param.StxTxid = param.StxTxid.Substring(2);
                }
                param.StxTxid = param.StxTxid.ToLower();
                var m2 = _txFactory.BuildTransactionModel().GetByStxTxId(param.StxTxid, _txFactory);
                if (m2 != null)
                {
                    throw new Exception($"Transaction '{param.StxTxid}' is already registered");
                }
            }
            try
            {
                var model = _txFactory.BuildTransactionModel();
                model.Type = param.BtcToStx ? TransactionEnum.BtcToStx : TransactionEnum.StxToBtc;
                model.BtcAddress = param.BtcAddress;
                model.StxAddress = param.StxAddress;
                model.CreateAt = DateTime.Now;
                model.UpdateAt = DateTime.Now;
                model.Status = TransactionStatusEnum.Initialized;
                model.BtcAmount = 0;
                model.StxAmount = 0;
                model.BtcTxid = param.BtcTxid;
                model.StxTxid = param.StxTxid;
                model.BtcFee = null;
                model.StxFee = null;

                model.Save();

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
                model.UpdateAt = DateTime.Now;
                model.Status = tx.Status;
                model.BtcAmount = tx.BtcAmount;
                model.StxAmount = tx.StxAmount;
                model.BtcTxid = tx.BtcTxid;
                model.StxTxid = tx.StxTxid;
                model.BtcFee = tx.BtcFee;
                model.StxFee = tx.StxFee;
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
                (int) TransactionStatusEnum.BtcNotConfirmed,
                (int) TransactionStatusEnum.StxNotConfirmed,
                (int) TransactionStatusEnum.BtcConfirmed,
                (int) TransactionStatusEnum.StxConfirmed,
                (int) TransactionStatusEnum.BtcConfirmedStxNotConfirmed,
                (int) TransactionStatusEnum.StxConfirmedBtcNotConfirmed
            };
            return _txFactory.BuildTransactionModel().ListByStatus(status, _txFactory);
        }

        public IEnumerable<ITransactionModel> ListAll()
        {
            return _txFactory.BuildTransactionModel().ListAll(_txFactory);
        }

        public IEnumerable<ITransactionLogModel> ListLogById(long txid)
        {
            return _txLogFactory.BuildTransactionLogModel().ListById(txid, _txLogFactory);
        }

        public async Task<bool> ProcessAllTransaction()
        {
            var txList = _txFactory.BuildTransactionModel().ListByStatus(new List<int>
            {
                (int)TransactionStatusEnum.Initialized,
                (int)TransactionStatusEnum.Calculated,
                (int)TransactionStatusEnum.BtcNotConfirmed,
                (int)TransactionStatusEnum.StxNotConfirmed,
                (int)TransactionStatusEnum.BtcConfirmed,
                (int)TransactionStatusEnum.StxConfirmed,
                (int)TransactionStatusEnum.BtcConfirmedStxNotConfirmed,
                (int)TransactionStatusEnum.StxConfirmedBtcNotConfirmed,
                (int)TransactionStatusEnum.BtcConfirmedStxConfirmed
            }, _txFactory);
            foreach (var tx in txList)
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
            try
            {
                if (tx.Type == TransactionEnum.BtcToStx)
                {
                    return await _btcTxService.ProcessTransaction(tx);
                }
                else
                {
                    return await _stxTxService.ProcessTransaction(tx);
                }
            }
            catch (Exception err)
            {
                AddLog(tx.TxId, err.Message, LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.CriticalError;
                tx.Update();
            }
            return await Task.FromResult(false);
        }
    }
}
