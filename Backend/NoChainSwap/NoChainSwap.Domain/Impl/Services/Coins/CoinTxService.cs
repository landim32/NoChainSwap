using NBitcoin.Secp256k1;
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
using static NBitcoin.Scripting.OutputDescriptor.TapTree;

namespace NoChainSwap.Domain.Impl.Services.Coins
{
    public abstract class CoinTxService: ICoinTxService
    {
        protected ICoinMarketCapService _coinMarketCapService;
        protected ITransactionDomainFactory _txFactory;
        protected ITransactionLogDomainFactory _txLogFactory;

        public abstract Task<string> GetNewAddress(int index);
        public abstract Task<string> GetPoolAddress();
        public abstract Task<long> GetPoolBalance();
        public abstract Task<string> Transfer(string address, long amount);
        public abstract CoinEnum GetCoin();
        public abstract Task<bool> IsTransactionSuccessful(string txid);
        public abstract string GetAddressUrl(string address);
        public abstract string GetTransactionUrl(string txId);
        public abstract string ConvertToString(decimal coin);
        public abstract Task<int> GetFee(string txid);
        public abstract Task<long> GetSenderAmount(string txid, string senderAddr);
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
            /*
            if (string.IsNullOrEmpty(tx.SenderTxid) && string.IsNullOrEmpty(tx.ReceiverTxid))
            {
                AddLog(tx.TxId, "Transaction tx_id is empty", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(false);
            }
            */
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
                case TransactionStatusEnum.WaitingSenderPayment:
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
                case TransactionStatusEnum.Canceled:
                    AddLog(tx.TxId, "Cant process a canceled transaction", LogTypeEnum.Error, _txLogFactory);
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
            var senderAmount = Convert.ToDecimal(tx.SenderAmount);
            var receiverAmount = Convert.ToDecimal(tx.ReceiverAmount);
            int txFee = 0;
            if (!string.IsNullOrEmpty(tx.SenderTxid) && !string.IsNullOrEmpty(tx.SenderAddress))
            {
                var txSenderAmount = await GetSenderAmount(tx.SenderTxid, tx.SenderAddress);
                txFee = await GetFee(tx.SenderTxid);

                if (senderAmount != txSenderAmount)
                {
                    if (txFee > 0)
                    {
                        tx.SenderFee = txFee;
                    }
                    tx.Status = TransactionStatusEnum.InvalidInformation;
                    tx = tx.Update();

                    AddLog(tx.TxId, string.Format(
                        "Invalid sender amount, sender inform {0:N8}, tx sender {1:N8}.",
                        senderAmount,
                        txSenderAmount
                    ), LogTypeEnum.Error, _txLogFactory);
                    return await Task.FromResult(new TransactionStepInfo
                    {
                        Success = false,
                        DoNextStep = false
                    });
                }
            }
            var price = _coinMarketCapService.GetCurrentPrice(GetCoin(), receiverService.GetCoin(), CurrencyEnum.USD);
            var receiverAmountCalc = senderAmount / price.ReceiverProportion;

            if (receiverAmount > receiverAmountCalc)
            {
                var receiverSpread = receiverAmount - receiverAmountCalc;
                var spreadPercent = (receiverSpread / receiverAmount) * 100M;
                if (spreadPercent > 2M)
                {
                    tx.Status = TransactionStatusEnum.CriticalError;
                    tx = tx.Update();
                    AddLog(tx.TxId, string.Format(
                        "Transaction generated a spread {0:N2}%, receiver amount {1} > {2} ({3}).",
                        spreadPercent,
                        receiverService.ConvertToString(Convert.ToDecimal(receiverAmount)),
                        receiverService.ConvertToString(Convert.ToDecimal(receiverAmountCalc)),
                        receiverService.ConvertToString(Convert.ToDecimal(receiverSpread))
                    ), LogTypeEnum.Error, _txLogFactory);
                    return await Task.FromResult(new TransactionStepInfo
                    {
                        Success = false,
                        DoNextStep = false
                    });
                }
                else
                {
                    AddLog(tx.TxId, string.Format(
                        "Transaction generated a spread {0:N2}%, receiver amount {1} > {2} ({3}).",
                        spreadPercent,
                        receiverService.ConvertToString(Convert.ToDecimal(receiverAmount)),
                        receiverService.ConvertToString(Convert.ToDecimal(receiverAmountCalc)),
                        receiverService.ConvertToString(Convert.ToDecimal(receiverSpread))
                    ), LogTypeEnum.Warning, _txLogFactory);
                }
            }

            if (txFee > 0)
            {
                tx.SenderFee = txFee;
            }
            tx.Status = TransactionStatusEnum.Calculated;
            tx = tx.Update();

            if (tx.SenderFee > 0)
            {
                AddLog(tx.TxId, string.Format(
                    "Transaction sender amount {0}, Fee {1} and receiver amount {2}.",
                    ConvertToString(Convert.ToDecimal(tx.SenderAmount)),
                    ConvertToString(Convert.ToDecimal(tx.SenderFee)),
                    receiverService.ConvertToString(Convert.ToDecimal(tx.ReceiverAmount))
                ), LogTypeEnum.Information, _txLogFactory);
            }
            else
            {
                AddLog(tx.TxId, string.Format(
                    "Transaction sender amount {0} and receiver amount {1}.",
                    ConvertToString(Convert.ToDecimal(tx.SenderAmount)),
                    receiverService.ConvertToString(Convert.ToDecimal(tx.ReceiverAmount))
                ), LogTypeEnum.Information, _txLogFactory);
            }

            return await Task.FromResult(new TransactionStepInfo
            {
                Success = true,
                DoNextStep = true
            });
        }
        private async Task<TransactionStepInfo> SenderFirstConfirmStep(ITransactionModel tx, ICoinTxService receiverService)
        {
            if (string.IsNullOrEmpty(tx.SenderTxid))
            {
                tx.Status = TransactionStatusEnum.WaitingSenderPayment;
                tx.Update();
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = true,
                    DoNextStep = false
                });
            }
            else if (await IsTransactionSuccessful(tx.SenderTxid))
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
                    Success = true,
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
            if (!string.IsNullOrEmpty(tx.SenderTxid) && !await IsTransactionSuccessful(tx.SenderTxid))
            {
                AddLog(tx.TxId, "Transaction local is confirmed, but not confirm on chain", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = false,
                    DoNextStep = false
                });
            }
            //var senderAmount = await GetSenderAmount(tx.SenderTxid, tx.SenderAddress);
            var senderAmount = tx.SenderAmount;
            if (!string.IsNullOrEmpty(tx.SenderTxid) && !string.IsNullOrEmpty(tx.SenderAddress))
            {
                senderAmount = await GetSenderAmount(tx.SenderTxid, tx.SenderAddress);
            }
            var price = _coinMarketCapService.GetCurrentPrice(GetCoin(), receiverService.GetCoin(), CurrencyEnum.USD);
            var receiverAmount = Convert.ToInt64(senderAmount / price.ReceiverProportion);

            var senderSymbol = Utils.CoinToStr(GetCoin());
            var receiverSymbol = Utils.CoinToStr(receiverService.GetCoin());

            var fee = await GetFee(tx.SenderTxid);

            tx.SenderAmount = senderAmount;
            tx.ReceiverAmount = receiverAmount;
            tx.SenderFee = fee;
            tx.Update();

            decimal feeValue = Math.Round(fee / 100000000M, 5);
            decimal senderValue = Math.Round(senderAmount / 100000000M, 5);
            decimal receiverValue = Math.Round(receiverAmount / 100000000M, 5);

            AddLog(tx.TxId, string.Format(
                "Transaction has {0:N5} {1}, Fee {2:N5} {3} and {4:N5} {5}.",
                senderValue, senderSymbol.ToUpper(),
                feeValue, senderSymbol.ToUpper(),
                receiverValue, receiverSymbol.ToUpper()
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
                tx.ReceiverTxid = txId;
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
                tx.Status = TransactionStatusEnum.SenderConfirmed;
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
