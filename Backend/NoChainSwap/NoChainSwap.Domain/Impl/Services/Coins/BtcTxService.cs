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
    public class BtcTxService : BaseTransactionService, IBtcTxService
    {
        private readonly ICoinMarketCapService _coinMarketCapService;
        private readonly IMempoolService _mempoolService;
        private readonly IBitcoinService _btcService;
        private readonly IStacksService _stxService;
        private readonly ITransactionDomainFactory _txFactory;
        private readonly ITransactionLogDomainFactory _txLogFactory;

        public BtcTxService(
            ICoinMarketCapService coinMarketCapService,
            IMempoolService mempoolService,
            IBitcoinService btcService,
            IStacksService stxService,
            ITransactionDomainFactory txFactory,
            ITransactionLogDomainFactory txLogFactory
        )
        {
            _coinMarketCapService = coinMarketCapService;
            _mempoolService = mempoolService;
            _btcService = btcService;
            _stxService = stxService;
            _txFactory = txFactory;
            _txLogFactory = txLogFactory;
        }
        public async Task<bool> ProcessTransaction(ITransactionModel tx)
        {
            if (string.IsNullOrEmpty(tx.BtcTxid))
            {
                AddLog(tx.TxId, "Transaction tx_id is empty", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(false);
            }
            var mempoolTx = await _mempoolService.GetTransaction(tx.BtcTxid);
            if (mempoolTx == null)
            {
                //throw new Exception("Dont find transaction on mempool");
                AddLog(tx.TxId, "Dont find transaction on mempool", LogTypeEnum.Warning, _txLogFactory);
                return await Task.FromResult(false);
            }
            var poolBtcAddr = _btcService.GetPoolAddress();

            //var addrs = new List<string>();
            var addrsSender = mempoolTx.VIn.Select(x => x.Prevout.ScriptPubKeyAddress).ToList();
            var addrsReceiver = mempoolTx.VOut.Select(x => x.ScriptPubKeyAddress).ToList();

            if (!addrsSender.Contains(tx.BtcAddress))
            {
                AddLog(tx.TxId, "Sender address not in transaction", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(false);
            }

            if (!addrsReceiver.Contains(poolBtcAddr))
            {
                AddLog(tx.TxId, "Pool address not in transaction", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(false);
            }

            var poolBtcAmount = mempoolTx.VOut.Where(x => x.ScriptPubKeyAddress == poolBtcAddr).Select(x => x.Value).Sum();
            if (poolBtcAmount <= 0)
            {
                AddLog(tx.TxId, "Pool did not receive any value", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(false);
            }
            bool lastConfirm = false;
            TransactionStepInfo step = null;
            do
            {
                step = await TransactionNextStep(tx, mempoolTx, poolBtcAddr, poolBtcAmount);
                lastConfirm = step.Success;
            }
            while (step.DoNextStep);
            return await Task.FromResult(lastConfirm);
        }

        private async Task<TransactionStepInfo> TransactionNextStep(
            ITransactionModel tx,
            MemPoolTxInfo mempoolTx,
            string poolBtcAddr,
            long poolBtcAmount
        )
        {
            TransactionStepInfo ret = null;
            switch (tx.Status)
            {
                case TransactionStatusEnum.Initialized:
                    ret = await CalculateStep(tx, mempoolTx, poolBtcAmount);
                    break;
                case TransactionStatusEnum.Calculated:
                    ret = await SenderFirstConfirmStep(tx, mempoolTx);
                    break;
                case TransactionStatusEnum.BtcNotConfirmed:
                    ret = await SenderTryConfirmStep(tx, mempoolTx);
                    break;
                case TransactionStatusEnum.BtcConfirmed:
                    ret = await ReceiverSendTxStep(tx, mempoolTx, poolBtcAmount);
                    break;
                case TransactionStatusEnum.BtcConfirmedStxNotConfirmed:
                    ret = await ReceiverTryConfirmStep(tx);
                    break;
                case TransactionStatusEnum.BtcConfirmedStxConfirmed:
                    AddLog(tx.TxId, "Transaction already completed", LogTypeEnum.Error, _txLogFactory);
                    break;
                case TransactionStatusEnum.InvalidInformation:
                    AddLog(tx.TxId, "Cant reprocess a transaction with invalid information", LogTypeEnum.Error, _txLogFactory);
                    break;
                case TransactionStatusEnum.CriticalError:
                    AddLog(tx.TxId, "Cant reprocess a transaction with critical error", LogTypeEnum.Error, _txLogFactory);
                    break;
                default:
                    var statusStr = GetTransactionEnumToString(TransactionStatusEnum.StxNotConfirmed);
                    AddLog(tx.TxId, string.Format("'{0}' is not a valid status to BTC transaction", statusStr), LogTypeEnum.Error, _txLogFactory);
                    break;
            }
            return ret ?? new TransactionStepInfo
            {
                Success = false,
                DoNextStep = false
            }; ;
        }

        public async Task<TransactionStepInfo> CalculateStep(
            ITransactionModel tx,
            MemPoolTxInfo mempoolTx,
            long poolAmount
        )
        {
            var price = _coinMarketCapService.GetCurrentPrice("bitcoin", "stacks");
            var stxAmount = Convert.ToInt64(poolAmount / price.BtcProportion * 100000000M);

            tx.BtcAmount = poolAmount;
            tx.StxAmount = stxAmount;
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
        public async Task<TransactionStepInfo> SenderFirstConfirmStep(ITransactionModel tx, MemPoolTxInfo mempoolTx)
        {
            if (mempoolTx.Status.Confirmed)
            {
                tx.Status = TransactionStatusEnum.BtcConfirmed;
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
                tx.Status = TransactionStatusEnum.BtcNotConfirmed;
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
                recipientAddress = tx.StxAddress,
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

        public async Task<TransactionStepInfo> SenderTryConfirmStep(ITransactionModel tx, MemPoolTxInfo mempoolTx)
        {
            if (mempoolTx.Status.Confirmed)
            {
                tx.Status = TransactionStatusEnum.BtcConfirmed;
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

        public async Task<TransactionStepInfo> ReceiverSendTxStep(ITransactionModel tx, MemPoolTxInfo mempoolTx, long poolAmount)
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

            tx.BtcAmount = poolAmount;
            tx.StxAmount = stxAmount2;
            tx.BtcFee = mempoolTx.Fee;
            tx.Update();

            decimal btcValue2 = Math.Round(poolAmount / 100000000M, 5);
            decimal stxValue2 = Math.Round(stxAmount2 / 100000000M, 5);

            AddLog(tx.TxId, string.Format("Transaction has {0:N5} BTC, Fee {1:N0} and extimate {2:N5} STX.", btcValue2, tx.BtcFee, stxValue2), LogTypeEnum.Information, _txLogFactory);

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
                tx.StxAddress = txId;
                tx.Status = TransactionStatusEnum.BtcConfirmedStxNotConfirmed;
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
            if (string.IsNullOrEmpty(tx.StxTxid))
            {
                AddLog(tx.TxId, "STX Transaction ID (tx_id) is empty", LogTypeEnum.Warning, _txLogFactory);
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = false,
                    DoNextStep = false
                });
            }
            var stxTx = await _stxService.GetTransaction(tx.StxTxid);
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
                tx.StxFee = fee;
                tx.Status = TransactionStatusEnum.BtcConfirmedStxConfirmed;
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
