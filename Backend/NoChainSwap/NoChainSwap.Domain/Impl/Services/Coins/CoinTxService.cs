using NoChainSwap.Domain.Impl.Core;
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

namespace NoChainSwap.Domain.Impl.Services.Coins
{
    public abstract class CoinTxService: ICoinTxService
    {
        protected ICoinMarketCapService _coinMarketCapService;
        protected IMempoolService _mempoolService;
        protected ITransactionDomainFactory _txFactory;
        protected ITransactionLogDomainFactory _txLogFactory;

        public abstract Task<string> GetPoolAddress();
        public abstract Task<long> GetPoolBalance();
        public abstract Task<string> Transfer(string address, long amount);
        public abstract string GetSlug();
        public abstract CoinEnum GetCoin();
        public abstract Task<bool> IsTransactionSuccessful(string txid);
        public abstract string GetAddressUrl(string address);
        public abstract string GetTransactionUrl(string txId);
        public abstract string ConvertToString(long coin);
        public abstract Task<int> GetFee(string txid);
        public abstract Task<long> GetSenderAmount(string txid, string senderAddr);
        public abstract decimal GetSenderProportion(ICoinTxService receiverService);
        public abstract string GetSwapDescription(decimal proportion);
        public abstract Task<bool> VerifyTransaction(ITransactionModel tx);

        public void AddLog(long txId, string msg, LogTypeEnum t, ITransactionLogDomainFactory txLogFactory)
        {
            var md = txLogFactory.BuildTransactionLogModel();
            md.TxId = txId;
            md.Date = DateTime.Now;
            md.LogType = t;
            md.Message = msg;
            md.Insert();
        }

        public async Task<bool> ProcessTransaction(ITransactionModel tx, ICoinTxService receiverService)
        {
            if (string.IsNullOrEmpty(tx.SenderTxid) && string.IsNullOrEmpty(tx.ReceiverTxid))
            {
                AddLog(tx.TxId, "Transaction tx_id is empty", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(false);
            }
            if (! await VerifyTransaction(tx))
            {
                return await Task.FromResult(false);
            }
            bool lastConfirm = false;
            TransactionStepInfo step = null;
            do
            {
                step = await TransactionNextStep(tx, receiverService);
                lastConfirm = step.Success;
            }
            while (step.DoNextStep);
            return await Task.FromResult(lastConfirm);
        }

        private async Task<TransactionStepInfo> TransactionNextStep(
            ITransactionModel tx,
            ICoinTxService receiverService
        )
        {
            TransactionStepInfo ret = null;
            switch (tx.Status)
            {
                case TransactionStatusEnum.Initialized:
                    ret = await CalculateStep(tx, receiverService);
                    break;
                case TransactionStatusEnum.Calculated:
                    ret = await SenderFirstConfirmStep(tx, receiverService);
                    break;
                case TransactionStatusEnum.SenderNotConfirmed:
                    ret = await SenderTryConfirmStep(tx, receiverService);
                    break;
                case TransactionStatusEnum.SenderConfirmed:
                    ret = await ReceiverSendTxStep(tx, receiverService);
                    break;
                case TransactionStatusEnum.SenderConfirmedReiceiverNotConfirmed:
                    ret = await ReceiverTryConfirmStep(tx, receiverService);
                    break;
                case TransactionStatusEnum.Finished:
                    AddLog(tx.TxId, "Transaction already completed", LogTypeEnum.Error, _txLogFactory);
                    break;
                case TransactionStatusEnum.InvalidInformation:
                    AddLog(tx.TxId, "Cant reprocess a transaction with invalid information", LogTypeEnum.Error, _txLogFactory);
                    break;
                case TransactionStatusEnum.CriticalError:
                    AddLog(tx.TxId, "Cant reprocess a transaction with critical error", LogTypeEnum.Error, _txLogFactory);
                    break;
                default:
                    var statusStr = TransactionService.GetTransactionEnumToString(tx.Status);
                    AddLog(tx.TxId, string.Format("'{0}' is not a valid status to transaction", statusStr), LogTypeEnum.Error, _txLogFactory);
                    break;
            }
            return ret ?? new TransactionStepInfo
            {
                Success = false,
                DoNextStep = false
            }; ;
        }

        private async Task<TransactionStepInfo> CalculateStep(
            ITransactionModel tx,
            ICoinTxService receiverService
        )
        {
            var senderAmount = await GetSenderAmount(tx.SenderTxid, tx.SenderAddress);
            var senderProportion = GetSenderProportion(receiverService);
            var price = _coinMarketCapService.GetCurrentPrice(GetCoin(), receiverService.GetCoin());
            var receiverAmount = Convert.ToInt64(senderAmount / senderProportion * 100000000M);

            var senderSymbol = Utils.CoinToStr(GetCoin());
            var receiverSymbol = Utils.CoinToStr(receiverService.GetCoin());

            tx.SenderAmount = senderAmount;
            tx.ReceiverAmount = receiverAmount;
            tx.SenderFee = await GetFee(tx.SenderTxid);
            tx.Status = TransactionStatusEnum.Calculated;
            tx.Update();

            decimal senderValue = Math.Round(senderAmount / 100000000M, 5);
            decimal receiverValue = Math.Round(receiverAmount / 100000000M, 5);

            AddLog(tx.TxId, string.Format(
                "Transaction has {0:N5} {1}, Fee {2:N0} {3} and extimate {4:N5} {5}.", 
                senderValue, senderSymbol, 
                tx.SenderFee, senderSymbol, 
                receiverValue, receiverSymbol
            ), LogTypeEnum.Information, _txLogFactory);

            return await Task.FromResult(new TransactionStepInfo
            {
                Success = true,
                DoNextStep = true
            });
        }
        private async Task<TransactionStepInfo> SenderFirstConfirmStep(ITransactionModel tx, ICoinTxService receiverService)
        {
            if (await IsTransactionSuccessful(tx.SenderTxid))
            {
                tx.Status = TransactionStatusEnum.SenderConfirmed;
                tx.Update();
                AddLog(tx.TxId, "Sender transaction confirmed.", LogTypeEnum.Information, _txLogFactory);
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = true,
                    DoNextStep = true
                });
            }
            else
            {
                tx.Status = TransactionStatusEnum.SenderNotConfirmed;
                tx.Update();
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = false,
                    DoNextStep = false
                });
            }
        }

        private async Task<TransactionStepInfo> SenderTryConfirmStep(ITransactionModel tx, ICoinTxService receiverService)
        {
            if (await IsTransactionSuccessful(tx.SenderTxid))
            {
                tx.Status = TransactionStatusEnum.SenderConfirmed;
                tx.Update();
                AddLog(tx.TxId, "Sender Transaction confirmed.", LogTypeEnum.Information, _txLogFactory);
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = true,
                    DoNextStep = true
                });
            }
            else
            {
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = true,
                    DoNextStep = false
                });
            }
        }

        private async Task<TransactionStepInfo> ReceiverSendTxStep(ITransactionModel tx, ICoinTxService receiverService)
        {
            if (await IsTransactionSuccessful(tx.SenderTxid))
            {
                AddLog(tx.TxId, "Transaction local is confirmed, but mempool not confirm", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = false,
                    DoNextStep = false
                });
            }
            var senderAmount = await GetSenderAmount(tx.SenderTxid, tx.SenderAddress);
            var price = _coinMarketCapService.GetCurrentPrice(GetCoin(), receiverService.GetCoin());
            var receiverAmount = Convert.ToInt64((senderAmount / price.SenderProportion) * 100000000M);

            var senderSymbol = Utils.CoinToStr(GetCoin());
            var receiverSymbol = Utils.CoinToStr(receiverService.GetCoin());

            tx.SenderAmount = senderAmount;
            tx.ReceiverAmount = receiverAmount;
            tx.SenderFee = await GetFee(tx.SenderTxid);
            tx.Update();

            decimal senderValue = Math.Round(senderAmount / 100000000M, 5);
            decimal receiverValue = Math.Round(receiverAmount / 100000000M, 5);

            AddLog(tx.TxId, string.Format(
                "Transaction has {0:N5} {1}, Fee {2:N0} {3} and extimate {4:N5} {5}.",
                senderValue, senderSymbol,
                tx.SenderFee, senderSymbol,
                receiverValue, receiverSymbol
            ), LogTypeEnum.Information, _txLogFactory);

            //var poolAddr = await receiverService.GetPoolAddress();
            var poolBalance = await receiverService.GetPoolBalance();
            if (poolBalance < receiverAmount)
            {
                AddLog(tx.TxId, "Pool without enough balance", LogTypeEnum.Warning, _txLogFactory);
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = false,
                    DoNextStep = false
                });
            }
            try
            {
                var txId = await receiverService.Transfer(tx.ReceiverAddress, receiverAmount);
                if (string.IsNullOrEmpty(txId))
                {
                    AddLog(tx.TxId, "Tansaction ID (tx_id) is empty", LogTypeEnum.Warning, _txLogFactory);
                    return await Task.FromResult(new TransactionStepInfo
                    {
                        Success = false,
                        DoNextStep = false
                    });
                }
                tx.ReceiverAddress = txId;
                tx.Status = TransactionStatusEnum.SenderConfirmedReiceiverNotConfirmed;
                tx.Update();
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = true,
                    DoNextStep = false
                });
            }
            catch (Exception err)
            {
                AddLog(tx.TxId, err.Message, LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.CriticalError;
                tx.Update();
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = false,
                    DoNextStep = false
                });
            }
        }

        public async Task<TransactionStepInfo> ReceiverTryConfirmStep(ITransactionModel tx, ICoinTxService receiverService)
        {
            if (string.IsNullOrEmpty(tx.ReceiverTxid))
            {
                AddLog(tx.TxId, "Receiver transaction ID (tx_id) is empty", LogTypeEnum.Warning, _txLogFactory);
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = false,
                    DoNextStep = false
                });
            }
            /*
            var stxTx = await _stxService.GetTransaction(tx.ReceiverTxid);
            if (stxTx == null)
            {
                AddLog(tx.TxId, "STX Transaction is empty", LogTypeEnum.Warning, _txLogFactory);
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = false,
                    DoNextStep = false
                });
            }
            */
            if (await receiverService.IsTransactionSuccessful(tx.ReceiverTxid))
            {
                /*
                int fee = 0;
                if (!int.TryParse(stxTx.FeeRate, out fee))
                {
                    AddLog(tx.TxId, string.Format("Cant convert fee to int ({0})", stxTx.FeeRate), LogTypeEnum.Warning, _txLogFactory);
                    return await Task.FromResult(new TransactionStepInfo
                    {
                        Success = false,
                        DoNextStep = false
                    });
                }
                */
                tx.ReceiverFee = await receiverService.GetFee(tx.ReceiverTxid);
                tx.Status = TransactionStatusEnum.Finished;
                tx.Update();
                AddLog(tx.TxId, "Receiver transaction confirmed.", LogTypeEnum.Information, _txLogFactory);
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = true,
                    DoNextStep = false
                });
            }
            return await Task.FromResult(new TransactionStepInfo
            {
                Success = false,
                DoNextStep = false
            });
        }
    }
}
