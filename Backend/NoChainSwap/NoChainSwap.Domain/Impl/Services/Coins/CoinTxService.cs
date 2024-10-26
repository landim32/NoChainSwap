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
        protected IStacksService _stxService;
        protected ITransactionDomainFactory _txFactory;
        protected ITransactionLogDomainFactory _txLogFactory;

        public abstract string GetPoolAddress();
        public abstract string GetSlug();
        public abstract string GetAddressUrl(string address);
        public abstract string GetTransactionUrl(string txId);
        public abstract string ConvertToString(long coin);
        public abstract long GetSenderAmount();
        public abstract decimal GetSenderProportion(ICoinTxService receiverService);
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
            //MemPoolTxInfo mempoolTx,
            //string poolBtcAddr,
            //long poolBtcAmount
        )
        {
            TransactionStepInfo ret = null;
            switch (tx.Status)
            {
                case TransactionStatusEnum.Initialized:
                    ret = await CalculateStep(tx, receiverService);
                    break;
                case TransactionStatusEnum.Calculated:
                    //ret = await SenderFirstConfirmStep(tx, mempoolTx);
                    break;
                case TransactionStatusEnum.SenderNotConfirmed:
                    //ret = await SenderTryConfirmStep(tx, mempoolTx);
                    break;
                case TransactionStatusEnum.SenderConfirmed:
                    //ret = await ReceiverSendTxStep(tx, mempoolTx, poolBtcAmount);
                    break;
                case TransactionStatusEnum.SenderConfirmedReiceiverNotConfirmed:
                    ret = await ReceiverTryConfirmStep(tx);
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
        //MemPoolTxInfo mempoolTx,
        //long poolAmount
        )
        {
            var senderAmount = GetSenderAmount();
            var senderProportion = GetSenderProportion(receiverService);
            var price = _coinMarketCapService.GetCurrentPrice(GetSlug(), receiverService.GetSlug());
            var receiverAmount = Convert.ToInt64(senderAmount / senderProportion * 100000000M);

            tx.SenderAmount = senderAmount;
            tx.ReceiverAmount = receiverAmount;
            tx.BtcFee = mempoolTx.Fee;
            tx.Status = TransactionStatusEnum.Calculated;
            tx.Update();

            decimal btcValue = Math.Round(poolAmount / 100000000M, 5);
            decimal stxValue = Math.Round(stxAmount / 100000000M, 5);

            AddLog(tx.TxId, string.Format("Transaction has {0:N5} BTC, Fee {1:N0} and extimate {2:N5} STX.", btcValue, tx.BtcFee, stxValue), LogTypeEnum.Information, _txLogFactory);

            return await Task.FromResult(new TransactionStepInfo
            {
                Success = true,
                DoNextStep = true
            });
        }
        private async Task<TransactionStepInfo> SenderFirstConfirmStep(ITransactionModel tx, MemPoolTxInfo mempoolTx)
        {
            if (mempoolTx.Status.Confirmed)
            {
                tx.Status = TransactionStatusEnum.SenderConfirmed;
                tx.Update();
                AddLog(tx.TxId, "BTC Transaction confirmed.", LogTypeEnum.Information, _txLogFactory);
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

        private async Task<string> StartStxTransfer(ITransactionModel tx, long stxAmount)
        {
            var txHandle = await _stxService.Transfer(new TransferParamInfo
            {
                recipientAddress = tx.ReceiverAddress,
                amount = stxAmount
            });
            if (!string.IsNullOrEmpty(txHandle.Error))
            {
                throw new Exception(string.Format("{0} ({1})", txHandle.Error, txHandle.Reason));
            }
            if (!string.IsNullOrEmpty(txHandle.TxId))
            {
                return await Task.FromResult(txHandle.TxId);
            }
            return await Task.FromResult("");
        }

        private async Task<TransactionStepInfo> SenderTryConfirmStep(ITransactionModel tx, MemPoolTxInfo mempoolTx)
        {
            if (mempoolTx.Status.Confirmed)
            {
                tx.Status = TransactionStatusEnum.SenderConfirmed;
                tx.Update();
                AddLog(tx.TxId, "BTC Transaction confirmed.", LogTypeEnum.Information, _txLogFactory);
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

        private async Task<TransactionStepInfo> ReceiverSendTxStep(ITransactionModel tx, MemPoolTxInfo mempoolTx, long poolAmount)
        {
            if (!mempoolTx.Status.Confirmed)
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
            var price2 = _coinMarketCapService.GetCurrentPrice("bitcoin", "stacks");
            var stxAmount2 = Convert.ToInt64((poolAmount / price2.BtcProportion) * 100000000M);

            tx.SenderAmount = poolAmount;
            tx.ReceiverAmount = stxAmount2;
            tx.SenderFee = mempoolTx.Fee;
            tx.Update();

            decimal btcValue2 = Math.Round(poolAmount / 100000000M, 5);
            decimal stxValue2 = Math.Round(stxAmount2 / 100000000M, 5);

            AddLog(tx.TxId, string.Format("Transaction has {0:N5} BTC, Fee {1:N0} and extimate {2:N5} STX.", btcValue2, tx.SenderFee, stxValue2), LogTypeEnum.Information, _txLogFactory);

            var poolAddr = await _stxService.GetPoolAddress();
            var poolBalance = await _stxService.GetBalance(poolAddr);
            if (poolBalance < stxAmount2)
            {
                AddLog(tx.TxId, "Pool without enough STX", LogTypeEnum.Warning, _txLogFactory);
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = false,
                    DoNextStep = false
                });
            }
            try
            {
                var txId = await StartStxTransfer(tx, stxAmount2);
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

        public async Task<TransactionStepInfo> ReceiverTryConfirmStep(ITransactionModel tx)
        {
            if (string.IsNullOrEmpty(tx.ReceiverTxid))
            {
                AddLog(tx.TxId, "STX Transaction ID (tx_id) is empty", LogTypeEnum.Warning, _txLogFactory);
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = false,
                    DoNextStep = false
                });
            }
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
            if (string.Compare(stxTx.TxStatus, "success", true) == 0)
            {
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
                tx.ReceiverFee = fee;
                tx.Status = TransactionStatusEnum.Finished;
                tx.Update();
                AddLog(tx.TxId, "STX Transaction confirmed.", LogTypeEnum.Information, _txLogFactory);
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
