using NBitcoin;
using NoChainSwap.Domain.Impl.Models;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using NoChainSwap.Domain.Interfaces.Services;
using NoChainSwap.Domain.Interfaces.Services.Coins;
using NoChainSwap.DTO.MailerSend;
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
        protected readonly ICoinMarketCapService _coinMarketCapService;
        protected readonly IMailerSendService _mailerSendService;
        protected readonly IUserDomainFactory _userFactory;
        protected readonly ITransactionDomainFactory _txFactory;
        protected readonly ITransactionLogDomainFactory _txLogFactory;

        private const string DOMAIN = "https://nochainswap.org";

        public TransactionService(
            //ICoinMarketCapService coinMarketCapService,
            //IMempoolService mempoolService,
            //IStacksService stxService,
            IUserService userService,
            ICoinTxServiceFactory coinFactory,
            ICoinMarketCapService coinMarketCapService,
            IMailerSendService mailerSendService,
            IUserDomainFactory userFactory,
            ITransactionDomainFactory txFactory,
            ITransactionLogDomainFactory txLogFactory
        )
        {
            //_coinMarketCapService = coinMarketCapService;
            //_mempoolService = mempoolService;
            //_stxService = stxService;
            _userService = userService;
            _coinFactory = coinFactory;
            _coinMarketCapService = coinMarketCapService;
            _mailerSendService = mailerSendService;
            _userFactory = userFactory;
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
            if (!(param.SenderAmount > 0))
            {
                throw new Exception("Sender amount is empty");
            }
            if (!(param.ReceiverAmount > 0))
            {
                throw new Exception("Receiver amount is empty");
            }
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
                model.Hash = Guid.NewGuid().ToString();
                model.SenderCoin = Core.Utils.StrToCoin(param.SenderCoin);
                model.ReceiverCoin = Core.Utils.StrToCoin(param.ReceiverCoin);
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
                    double tax = Convert.ToDouble(model.ReceiverAmount) * 0.03;
                    model.SenderTax = null;
                    model.ReceiverTax = Convert.ToInt64(Math.Truncate(tax));
                    //model.ReceiverAmount -= model.ReceiverTax;
                }
                else if (model.ReceiverCoin == CoinEnum.BRL)
                {
                    double tax = Convert.ToDouble(model.SenderAmount) * 0.03;
                    model.SenderTax = Convert.ToInt64(Math.Truncate(tax));
                    model.ReceiverTax = null;
                    //model.SenderAmount -= model.SenderTax;
                }
                else
                {
                    double tax = Convert.ToDouble(model.SenderAmount) * 0.03;
                    model.SenderTax = Convert.ToInt64(Math.Truncate(tax));
                    model.ReceiverTax = null;
                    //model.SenderAmount -= model.SenderTax;
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

        public void ChangeStatus(long txId, TransactionStatusEnum status, string message)
        {
            var tx = GetById(txId);
            var senderService = _coinFactory.BuildCoinTxService(tx.SenderCoin);
            if (senderService == null)
            {
                throw new Exception("Transaction not suported");
            }
            tx.Status = status;
            tx.Update();

            senderService.AddLog(tx.TxId, message, LogTypeEnum.Warning, _txLogFactory);
        }

        public async void Payback(long txId, string receiverTxId, int receiverFee)
        {
            if (!(txId > 0))
            {
                throw new Exception("Transaction ID is empty");
            }
            if (string.IsNullOrEmpty(receiverTxId))
            {
                throw new Exception("Receiver Transaction ID is empty");
            }

            var tx = GetById(txId);

            var senderService = _coinFactory.BuildCoinTxService(tx.SenderCoin);
            var receiverService = _coinFactory.BuildCoinTxService(tx.ReceiverCoin);

            if (tx.Status == TransactionStatusEnum.WaitingSenderPayment)
            {
                tx.Status = TransactionStatusEnum.SenderConfirmed;
                tx.Update();

                AddLog(tx.TxId, "Sender transaction confirmed.", LogTypeEnum.Information, _txLogFactory);

                tx = GetById(txId);
                await SendEmail(tx, senderService, receiverService);
            }

            if (tx.Status != TransactionStatusEnum.SenderConfirmed)
            {
                throw new Exception("Transaction invalid for payback");
            }

            tx.ReceiverFee = receiverFee;
            tx.ReceiverTxid = receiverTxId;
            tx.Status = TransactionStatusEnum.SenderConfirmedReiceiverNotConfirmed;
            tx.Update();

            receiverService.AddLog(tx.TxId, "Transaction payback send with success", LogTypeEnum.Information, _txLogFactory);

            await SendEmail(tx, senderService, receiverService);
        }

        public void ConfirmSendPayment(long txId, string senderTxId)
        {
            if (!(txId > 0))
            {
                throw new Exception("Transaction ID is empty");
            }
            if (string.IsNullOrEmpty(senderTxId))
            {
                throw new Exception("Sender Transaction ID is empty");
            }

            var tx = GetById(txId);
            if (tx.Status != TransactionStatusEnum.WaitingSenderPayment)
            {
                throw new Exception("Transaction invalid for payment");
            }

            tx.SenderTxid = senderTxId;
            tx.Status = TransactionStatusEnum.SenderNotConfirmed;
            tx.Update();

            var senderService = _coinFactory.BuildCoinTxService(tx.SenderCoin);
            senderService.AddLog(tx.TxId, "Transaction payment send with success", LogTypeEnum.Information, _txLogFactory);
        }

        public async void ConfirmPayment(long txId)
        {
            if (!(txId > 0))
            {
                throw new Exception("Transaction ID is empty");
            }

            var tx = GetById(txId);
            if (tx.Status != TransactionStatusEnum.WaitingSenderPayment &&
                tx.Status != TransactionStatusEnum.SenderConfirmed &&
                tx.Status != TransactionStatusEnum.SenderConfirmedReiceiverNotConfirmed &&
                tx.Status != TransactionStatusEnum.SenderConfirmedReiceiverPaymentWaiting)
            {
                throw new Exception("Transaction invalid for payment");
            }

            var senderService = _coinFactory.BuildCoinTxService(tx.SenderCoin);
            var receiverService = _coinFactory.BuildCoinTxService(tx.ReceiverCoin);

            if (tx.Status == TransactionStatusEnum.WaitingSenderPayment)
            {
                tx.Status = TransactionStatusEnum.SenderConfirmed;
                tx.Update();

                AddLog(tx.TxId, "Sender transaction confirmed.", LogTypeEnum.Information, _txLogFactory);

                await SendEmail(tx, senderService, receiverService);

                tx.Status = TransactionStatusEnum.SenderConfirmedReiceiverPaymentWaiting;
                tx.Update();

                senderService.AddLog(tx.TxId, "Transaction payment send with success", LogTypeEnum.Information, _txLogFactory);
            }
            else
            {
                tx.Status = TransactionStatusEnum.Finished;
                tx.Update();

                receiverService.AddLog(tx.TxId, "Transaction completed successfully", LogTypeEnum.Information, _txLogFactory);
            }

            await SendEmail(tx, senderService, receiverService);
        }

        public ITransactionModel GetById(long txId)
        {
            return _txFactory.BuildTransactionModel().GetById(txId, _txFactory);
        }

        public ITransactionModel GetByHash(string hash)
        {
            return _txFactory.BuildTransactionModel().GetByHash(hash, _txFactory);
        }

        public ITransactionModel Update(TransactionInfo tx)
        {
            var model = _txFactory.BuildTransactionModel().GetById(tx.TxId, _txFactory);
            if (model == null)
            {
                throw new Exception("Transaction not found.");
            }
            model.SenderAddress = tx.SenderAddress;
            model.ReceiverAddress = tx.ReceiverAddress;
            model.RecipientAddress = tx.RecipientAddress;
            model.UpdateAt = DateTime.Now;
            model.Status = tx.Status;
            model.SenderAmount = tx.SenderAmount;
            model.ReceiverAmount = tx.ReceiverAmount;
            model.SenderTxid = tx.SenderTxid;
            model.ReceiverTxid = tx.ReceiverTxid;
            model.SenderFee = tx.SenderFee;
            model.ReceiverFee = tx.ReceiverFee;
            model.SenderTax = tx.SenderTax;
            return model.Update();
        }

        public IEnumerable<ITransactionModel> ListByStatusActive()
        {
            var status = new List<int>() {
                (int) TransactionStatusEnum.Initialized,
                (int) TransactionStatusEnum.Calculated,
                (int) TransactionStatusEnum.WaitingSenderPayment,
                (int) TransactionStatusEnum.DetectedSenderPayment,
                (int) TransactionStatusEnum.SenderNotConfirmed,
                (int) TransactionStatusEnum.SenderConfirmed,
                (int) TransactionStatusEnum.SenderConfirmedReiceiverPaymentWaiting,
                (int) TransactionStatusEnum.SenderConfirmedReiceiverNotConfirmed,
            };
            return _txFactory.BuildTransactionModel().ListByStatus(status, _txFactory);
        }

        public IEnumerable<ITransactionModel> ListAll()
        {
            return _txFactory.BuildTransactionModel().ListAll(_txFactory);
        }

        public IEnumerable<ITransactionModel> ListByUser(long userId)
        {
            return _txFactory.BuildTransactionModel().ListByUser(userId, _txFactory);
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
                return await StartProcessTransaction(tx, senderService, receiverService);
            }
            catch (Exception err)
            {
                senderService.AddLog(tx.TxId, err.Message, LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.CriticalError;
                tx.Update();
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> DetectAllTransaction() {
            var allTxs = _txFactory.BuildTransactionModel().ListToDetect(_txFactory);
            if (allTxs.Count() == 0)
            {
                return await Task.FromResult(true);
            }
            var txs = allTxs.GroupBy(x => x.SenderCoin).Select(x => new
            {
                SenderCoin = x.Key,
                Addresses = x.ToList().Select(y => y.RecipientAddress).ToList()
            }).ToList();
            if (txs.Count() == 0)
            {
                return await Task.FromResult(true);
            }
            foreach (var txGroup in txs)
            {
                var senderService = _coinFactory.BuildCoinTxService(txGroup.SenderCoin);
                var txsBlockchain = await senderService.DetectNewTransactions(txGroup.Addresses);
                if (txsBlockchain.Count() > 0)
                {
                    foreach (var txReturn in txsBlockchain)
                    {
                        var tx = _txFactory.BuildTransactionModel().GetByRecipientAddr(txReturn.RecipientAddress, _txFactory);
                        tx.Status = TransactionStatusEnum.DetectedSenderPayment;
                        tx.SenderAddress = txReturn.SenderAddress;
                        tx.SenderTxid = txReturn.SenderTxId;
                        tx.Update();

                        senderService.AddLog(tx.TxId, "Transaction detected on blockchain", LogTypeEnum.Information, _txLogFactory);
                    }
                }
            }
            return await Task.FromResult(true);
        }

        private void AddLog(long txId, string msg, LogTypeEnum t, ITransactionLogDomainFactory txLogFactory)
        {
            var md = txLogFactory.BuildTransactionLogModel();
            md.TxId = txId;
            md.Date = DateTime.Now;
            md.LogType = t;
            md.Message = msg;
            md.Insert();
        }

        private async Task<bool> StartProcessTransaction(
            ITransactionModel tx, 
            ICoinTxService senderService, 
            ICoinTxService receiverService
        )
        {
            if (!await senderService.VerifyTransaction(tx))
            {
                return await Task.FromResult(false);
            }
            bool lastConfirm = false;
            TransactionStepInfo step = null;
            do
            {
                step = await TransactionNextStep(tx, senderService, receiverService);
                lastConfirm = step.Success;
            }
            while (step.DoNextStep);
            return await Task.FromResult(lastConfirm);
        }

        private async Task<TransactionStepInfo> TransactionNextStep(
            ITransactionModel tx,
            ICoinTxService senderService,
            ICoinTxService receiverService
        )
        {
            TransactionStepInfo ret = null;
            switch (tx.Status)
            {
                case TransactionStatusEnum.Initialized:
                    ret = await ReCalculateStep(tx, senderService, receiverService, TransactionStatusEnum.Calculated);
                    break;
                case TransactionStatusEnum.Calculated:
                    ret = await SenderFirstConfirmStep(tx, senderService, receiverService);
                    break;
                case TransactionStatusEnum.WaitingSenderPayment:
                    ret = await SenderFirstConfirmStep(tx, senderService, receiverService);
                    break;
                case TransactionStatusEnum.DetectedSenderPayment:
                    ret = await ReCalculateStep(tx, senderService, receiverService, TransactionStatusEnum.SenderNotConfirmed);
                    break;
                case TransactionStatusEnum.SenderNotConfirmed:
                    ret = await SenderTryConfirmStep(tx, senderService, receiverService);
                    break;
                case TransactionStatusEnum.SenderConfirmed:
                    ret = await ReCalculateStep(tx, senderService, receiverService, TransactionStatusEnum.SenderConfirmedReiceiverPaymentWaiting);
                    break;
                case TransactionStatusEnum.SenderConfirmedReiceiverPaymentWaiting:
                    ret = await ReceiverSendTxStep(tx, senderService, receiverService);
                    break;
                case TransactionStatusEnum.SenderConfirmedReiceiverNotConfirmed:
                    ret = await ReceiverTryConfirmStep(tx, senderService, receiverService);
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

        private async Task<TransactionStepInfo> ReCalculateStep(
            ITransactionModel tx,
            ICoinTxService senderService,
            ICoinTxService receiverService,
            TransactionStatusEnum successStatus
        )
        {
            var senderAmount = Convert.ToDecimal(tx.SenderAmount);
            var receiverAmount = Convert.ToDecimal(tx.ReceiverAmount);
            long txFee = 0;
            if (!string.IsNullOrEmpty(tx.SenderTxid) && !string.IsNullOrEmpty(tx.SenderAddress))
            {
                var txResume = await senderService.GetResumeTransaction(tx.SenderTxid);
                txFee = txResume.Fee;
                //var txSenderAmount = txResume.SenderAmount;
                //txFee = await senderService.GetFee(tx.SenderTxid);

                if (senderAmount != txResume.Amount)
                {
                    if (txResume.Fee > 0)
                    {
                        tx.SenderFee = txResume.Fee;
                    }
                    tx.Status = TransactionStatusEnum.InvalidInformation;
                    tx = tx.Update();

                    AddLog(tx.TxId, string.Format(
                        "Invalid sender amount, sender inform {0:N8}, tx sender {1:N8}.",
                        senderAmount,
                        txResume.Amount
                    ), LogTypeEnum.Error, _txLogFactory);
                    return await Task.FromResult(new TransactionStepInfo
                    {
                        Success = false,
                        DoNextStep = false
                    });
                }
            }
            var price = _coinMarketCapService.GetCurrentPrice(senderService.GetCoin(), receiverService.GetCoin(), CurrencyEnum.USD);
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
            tx.Status = successStatus;
            tx = tx.Update();

            if (tx.SenderFee > 0)
            {
                AddLog(tx.TxId, string.Format(
                    "Transaction sender amount {0}, Fee {1} and receiver amount {2}.",
                    senderService.ConvertToString(Convert.ToDecimal(tx.SenderAmount)),
                    senderService.ConvertToString(Convert.ToDecimal(tx.SenderFee)),
                    receiverService.ConvertToString(Convert.ToDecimal(tx.ReceiverAmount))
                ), LogTypeEnum.Information, _txLogFactory);
            }
            else
            {
                AddLog(tx.TxId, string.Format(
                    "Transaction sender amount {0} and receiver amount {1}.",
                    senderService.ConvertToString(Convert.ToDecimal(tx.SenderAmount)),
                    receiverService.ConvertToString(Convert.ToDecimal(tx.ReceiverAmount))
                ), LogTypeEnum.Information, _txLogFactory);
            }

            return await Task.FromResult(new TransactionStepInfo
            {
                Success = true,
                DoNextStep = true
            });
        }

        private async Task<TransactionStepInfo> SenderFirstConfirmStep(
            ITransactionModel tx,
            ICoinTxService senderService,
            ICoinTxService receiverService
        )
        {
            if (string.IsNullOrEmpty(tx.SenderTxid))
            {
                if (tx.Status != TransactionStatusEnum.WaitingSenderPayment)
                {
                    tx.Status = TransactionStatusEnum.WaitingSenderPayment;
                    tx.Update();
                    await SendEmail(tx, senderService, receiverService);
                }
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = true,
                    DoNextStep = false
                });
            }
            else
            {
                var txResume = await senderService.GetResumeTransaction(tx.SenderTxid);
                if (txResume.Success)
                {
                    tx.Status = TransactionStatusEnum.SenderConfirmed;
                    tx.Update();
                    AddLog(tx.TxId, "Sender transaction confirmed.", LogTypeEnum.Information, _txLogFactory);

                    await SendEmail(tx, senderService, receiverService);

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
        }

        private async Task<TransactionStepInfo> SenderTryConfirmStep(
            ITransactionModel tx,
            ICoinTxService senderService,
            ICoinTxService receiverService
        )
        {
            var txResume = await senderService.GetResumeTransaction(tx.SenderTxid);
            if (txResume.Success)
            {
                tx.Status = TransactionStatusEnum.SenderConfirmed;
                tx.Update();
                AddLog(tx.TxId, "Sender Transaction confirmed.", LogTypeEnum.Information, _txLogFactory);

                await SendEmail(tx, senderService, receiverService);

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

        private async Task<TransactionStepInfo> ReceiverSendTxStep(
            ITransactionModel tx,
            ICoinTxService senderService,
            ICoinTxService receiverService
        )
        {
            if (!string.IsNullOrEmpty(tx.SenderTxid))
            {
                var txResume = await senderService.GetResumeTransaction(tx.SenderTxid);
                if (!txResume.Success)
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
            }

            if (!receiverService.IsPaybackAutomatic())
            {
                return await Task.FromResult(new TransactionStepInfo
                {
                    Success = true,
                    DoNextStep = false
                });
            }
            var poolBalance = await receiverService.GetPoolBalance();
            if (poolBalance < tx.ReceiverAmount)
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
                var txId = await receiverService.Transfer(tx.ReceiverAddress, tx.ReceiverAmount);
                if (string.IsNullOrEmpty(txId))
                {
                    AddLog(tx.TxId, "Transaction ID (tx_id) is empty", LogTypeEnum.Warning, _txLogFactory);
                    return await Task.FromResult(new TransactionStepInfo
                    {
                        Success = false,
                        DoNextStep = false
                    });
                }

                tx.ReceiverTxid = txId;
                tx.Status = TransactionStatusEnum.SenderConfirmedReiceiverNotConfirmed;
                tx.Update();
                AddLog(tx.TxId, "Payment sent successfully, waiting confirmation", LogTypeEnum.Information, _txLogFactory);

                await SendEmail(tx, senderService, receiverService);

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

        public async Task<TransactionStepInfo> ReceiverTryConfirmStep(
            ITransactionModel tx,
            ICoinTxService senderService,
            ICoinTxService receiverService
        )
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
            var txResume = await senderService.GetResumeTransaction(tx.ReceiverTxid);
            if (txResume.Success)
            {
                tx.ReceiverFee = txResume.Fee;
                tx.Status = TransactionStatusEnum.Finished;
                tx.Update();

                AddLog(tx.TxId, "Receiver transaction confirmed.", LogTypeEnum.Information, _txLogFactory);

                await SendEmail(tx, senderService, receiverService);

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

        #region Mail send

        private async Task<bool> SendEmail(
            ITransactionModel tx, 
            ICoinTxService senderService,
            ICoinTxService receiverService
        )
        {
            var user = tx.GetUser(_userFactory);
            if (string.IsNullOrEmpty(user.Email))
            {
                return false;
            }
            var md = _userFactory.BuildUserModel();

            var mailSubject = string.Empty;
            var senderCoin = Core.Utils.CoinToText(tx.SenderCoin);
            var receiverCoin = Core.Utils.CoinToText(tx.ReceiverCoin);
            var senderAmount = senderService.ConvertToString(tx.SenderAmount);
            var receiverAmount = receiverService.ConvertToString(tx.ReceiverAmount);

            var txUrl = $"{DOMAIN}/tx/{tx.Hash}";
            var txSenderName = "NoChainSwap";

            if (tx.SenderCoin == CoinEnum.BRL || tx.ReceiverCoin == CoinEnum.BRL)
            {
                txUrl = $"{DOMAIN}/big-whale/tx/{tx.Hash}";
                txSenderName = "Big Bank Security";
            }

            var textMessage = $"Hi {user.Name},\r\n\r\n";
            var textHtml = $"<p>Hi <b>{user.Name}</b>,</p>\r\n";

            if (tx.Status == TransactionStatusEnum.WaitingSenderPayment)
            {
                mailSubject = $"New transaction {senderCoin} x {receiverCoin}";
                textMessage +=
                    $"Thank you for initiating a conversion request from {senderCoin} to {receiverCoin}. " +
                    "Here are the details of your transaction:\r\n\r\n";
                textHtml +=
                    $"<p>Thank you for initiating a conversion request from <b>{senderCoin}</b> to <b>{receiverCoin}</b>. " +
                    "Here are the details of your transaction:</p>\r\n";
                if (tx.SenderCoin == CoinEnum.BRL)
                {
                    textMessage +=
                        "PIX Key:\r\n\r\n" + tx.RecipientAddress + "\r\n" +
                        $"Exact Value: {senderAmount}.\r\n\r\n";
                    textHtml +=
                        "<p>PIX Key:<br />\r\n<b>" + tx.RecipientAddress + "</b></p>\r\n" +
                        $"<ul><li>Exact Value: <b>{senderAmount}</b></li></ul>\r\n\r\n";
                }
                else
                {
                    textMessage +=
                        $"Deposite Address: {tx.RecipientAddress}\r\n\r\n" +
                        $"Exact Value: {senderAmount}.\r\n\r\n";
                    textHtml +=
                        "<ul>\r\n" + 
                        $"<li>Deposite Address: <b>{tx.RecipientAddress}</b></li>\r\n" +
                        $"<li>Exact Value: <b>{senderAmount}</b></li>\r\n" +
                        "</ul>\r\n";
                }
                textMessage +=
                    "Please ensure that you transfer the exact amount listed above.\r\n" +
                    $"Once the payment is confirmed, we will proceed to transfer {receiverAmount} " +
                    $"to your informed address {tx.ReceiverAddress}.\r\n";
                textHtml +=
                    "<p>Please ensure that you transfer the exact amount listed above.</p>\r\n" +
                    $"<p>Once the payment is confirmed, we will proceed to transfer <b>{receiverAmount}</b> " +
                    $"to your informed address <b>{tx.ReceiverAddress}</b>.</p>\r\n";
            }
            else if (tx.Status == TransactionStatusEnum.SenderConfirmed)
            {
                mailSubject = $"Payment Received in {senderCoin} ({senderAmount})";
                textMessage +=
                    $"We have successfully received your payment of {senderAmount} via {senderCoin}. " +
                    $"Your conversion request from {senderCoin} to {receiverCoin} is now being processed.\r\n\r\n" +
                    $"Payment Received: {senderAmount}\r\n";
                textHtml +=
                    $"<p>We have successfully received your payment of <b>{senderAmount}</b> via <b>{senderCoin}</b>. " +
                    $"Your conversion request from <b>{senderCoin}</b> to <b>{receiverCoin}</b> is now being processed.</p>\r\n" +
                    "<ul>\r\n" +
                    $"<li>Payment Received: <b>{senderAmount}</b></li>\r\n";
                if (!string.IsNullOrEmpty(tx.SenderAddress))
                {
                    textMessage +=
                        $"Sender Address: {tx.SenderAddress}\r\n" +
                        "Address URL: " + senderService.GetAddressUrl(tx.SenderAddress) + "\r\n";
                    textHtml +=
                        $"<li>Sender Address: <b><a href=\"" +
                        senderService.GetAddressUrl(tx.SenderAddress) +
                        $"\">{tx.SenderAddress}</a></b></li>\r\n";
                }
                if (!string.IsNullOrEmpty(tx.SenderTxid))
                {
                    textMessage +=
                        $"Transaction Hash: {tx.SenderTxid}\r\n" +
                        "Transaction URL: " + senderService.GetTransactionUrl(tx.SenderTxid) + "\r\n";
                    textHtml +=
                        $"<li>Transaction Hash: <b><a href=\"" +
                        senderService.GetTransactionUrl(tx.SenderTxid) +
                        $"\">{tx.SenderTxid}</a></b></li>\r\n";
                }
                textHtml += "</ul>\r\n";

                if (tx.ReceiverCoin == CoinEnum.BRL)
                {
                    textMessage += "\r\n" +
                        $"Your {senderAmount} will be transferred to the PIX key " +
                        $"{tx.ReceiverAddress} you provided shortly.\r\n";
                    textHtml += "\r\n" +
                        $"<p>Your <b>{senderAmount}</b> will be transferred to the PIX key " +
                        $"<b>{tx.ReceiverAddress}</b> you provided shortly.</p>\r\n";
                }
                else
                {
                    textMessage += "\r\n" +
                        $"Your {senderAmount} will be transferred to the wallet address " +
                        $"{tx.ReceiverAddress} you provided shortly.\r\n";
                    textHtml += "\r\n" +
                        $"<p>Your <b>{senderAmount}</b> will be transferred to the wallet address " +
                        $"<b>{tx.ReceiverAddress}</b> you provided shortly.</p>\r\n";
                }

            }
            else if (
                tx.Status == TransactionStatusEnum.SenderConfirmedReiceiverNotConfirmed ||
                tx.Status == TransactionStatusEnum.SenderConfirmedReiceiverPaymentWaiting
            )
            {
                mailSubject = $"Payment Sent in {receiverCoin} ({receiverAmount})";
                if (tx.ReceiverCoin == CoinEnum.BRL)
                {
                    textMessage +=
                        $"We have sent the {receiverCoin} payment for your {senderCoin} to {receiverCoin} conversion request.\r\n\r\n";
                    textHtml +=
                        $"<p>We have sent the <b>{receiverCoin}</b> payment for " + 
                        $"your <b>{senderCoin}</b> to <b>{receiverCoin}</b> conversion request.</p>\r\n";
                }
                else
                {
                    textMessage +=
                        $"We have sent the {receiverCoin} payment for your {senderCoin} to {receiverCoin} conversion request. " +
                        "The transaction is currently awaiting confirmation on the blockchain.\r\n\r\n";
                    textHtml +=
                        $"<p>We have sent the <b>{receiverCoin}</b> payment for your <b>{senderCoin}</b> " + 
                        $"to {receiverCoin} conversion request.</p>\r\n" +
                        "<p>The transaction is currently awaiting confirmation on the blockchain.</p>\r\n";
                }

                textMessage += "Here are the details of your transaction:\r\n";
                textHtml += "<p>Here are the details of your transaction:</p>\r\n<ul>\r\n";

                if (!string.IsNullOrEmpty(tx.ReceiverTxid))
                {
                    textMessage +=
                        $"- Transaction Hash: {tx.ReceiverTxid}\r\n" +
                        "- Transaction URL: " + receiverService.GetTransactionUrl(tx.ReceiverTxid) + "\r\n";
                    textHtml +=
                        $"<li>Transaction Hash: <a href=\"" +
                        receiverService.GetTransactionUrl(tx.ReceiverTxid) +
                        $"\">{tx.ReceiverTxid}</a></li>\r\n";
                }
                textMessage += $"- Amount Sent: {receiverAmount}\r\n";
                textHtml += $"<li>Amount Sent: <b>{receiverAmount}</b></li>\r\n";
                if (tx.ReceiverCoin == CoinEnum.BRL)
                {
                    textMessage += $"- PIX Key: {tx.ReceiverAddress}\r\n";
                    textHtml += $"<li>PIX Key: <b>{tx.ReceiverAddress}</b></li>\r\n";
                }
                else
                {
                    textMessage += $"- Recipient Address Wallet: {tx.ReceiverAddress}\r\n";
                    textHtml += $"<li>Recipient Address Wallet: <b>{tx.ReceiverAddress}</b></li>\r\n";
                }

                textHtml += "</ul>\r\n";

                if (tx.ReceiverCoin == CoinEnum.BRL)
                {
                    textMessage +=
                        "The transaction is expected to be confirmed shortly. Once it is fully " +
                        "processed, you will receive the funds in your PIX account.\r\n\r\n";
                    textHtml +=
                        "<p>The transaction is expected to be confirmed shortly. Once it is fully " +
                        "processed, you will receive the funds in your PIX account.</p>\r\n";
                }
                else
                {
                    textMessage += 
                        "The transaction is expected to be confirmed shortly. Once it is fully " + 
                        "processed, you will receive the funds in your wallet.\r\n\r\n";
                    textHtml +=
                        "<p>The transaction is expected to be confirmed shortly. Once it is fully " +
                        "processed, you will receive the funds in your wallet.</p>\r\n";
                }
            }
            else if (tx.Status == TransactionStatusEnum.Finished)
            {
                mailSubject = "Payment Confirmed: Transaction Completed Successfully";

                textMessage += 
                    $"We are pleased to inform you that your {senderCoin} to {receiverCoin} conversion " + 
                    $"request has been successfully completed. The {receiverCoin} payment has been confirmed " + 
                    "and is now available in your " + 
                    ((tx.ReceiverCoin == CoinEnum.BRL) ? "PIX account" : "wallet") + 
                    ".\r\n\r\n";
                textHtml +=
                    $"<p>We are pleased to inform you that your <b>{senderCoin}</b> to <b>{receiverCoin}</b> conversion " +
                    $"request has been successfully completed. The <b>{receiverCoin}</b> payment has been confirmed " +
                    "and is now available in your " +
                    ((tx.ReceiverCoin == CoinEnum.BRL) ? "PIX account" : "wallet") +
                    ".</p>\r\n";

                if (!string.IsNullOrEmpty(tx.ReceiverTxid))
                {
                    textMessage +=
                        $"- Transaction Hash: {tx.ReceiverTxid}\r\n" +
                        "- Transaction URL: " + receiverService.GetTransactionUrl(tx.ReceiverTxid) + "\r\n";
                    textHtml +=
                        $"<li>Transaction Hash: <a href=\"" +
                        receiverService.GetTransactionUrl(tx.ReceiverTxid) +
                        $"\">{tx.ReceiverTxid}</a></li>\r\n";
                }

                textMessage += $"- Amount Sent: {receiverAmount}\r\n";
                textHtml += $"<li>Amount Sent: <b>{receiverAmount}</b></li>\r\n";

                if (tx.ReceiverCoin == CoinEnum.BRL)
                {
                    textMessage += $"- PIX Key: {tx.ReceiverAddress}\r\n";
                    textHtml += $"<li>PIX Key: <b>{tx.ReceiverAddress}</b></li>\r\n";
                }
                else
                {
                    textMessage += $"- Recipient Address Wallet: {tx.ReceiverAddress}\r\n";
                    textHtml += $"<li>Recipient Address Wallet: <b>{tx.ReceiverAddress}</b></li>\r\n";
                }

                textHtml += "</ul>\r\n";

                textMessage += 
                    "Thank you for choosing our service! If you have any further questions or need " + 
                    "assistance, please don’t hesitate to contact our support team.\r\n\r\n";
                textHtml +=
                    "<p>Thank you for choosing our service! If you have any further questions or need " +
                    "assistance, please don’t hesitate to contact our support team.</p>\r\n";
            }

            textMessage +=
                "You can follow all the steps of the transaction by clicking on the link below:\r\n\r\n" +
                txUrl + "\r\n\r\n" +
                "Best regards,\r\n" +
                "NoChainSwap Team";
            textHtml +=
                "<p>You can follow all the steps of the transaction by clicking on the link below:<br />\r\n<br />\r\n" +
                $"<a href=\"{txUrl}\">{txUrl}</a></p>\r\n" +
                $"<p>Best regards,<br />\r\n<b>{txSenderName} Team</b></p>";

            var mail = new MailerInfo
            {
                From = new MailerRecipientInfo
                {
                    Email = "contact@nochainswap.org",
                    Name = $"{txSenderName} Mailmaster"
                },
                To = new List<MailerRecipientInfo> {
                    new MailerRecipientInfo {
                        Email = user.Email,
                        Name = user.Name ?? user.Email
                    }
                },
                Subject = $"[{txSenderName}] {mailSubject}",
                Text = textMessage,
                Html = textHtml
            };
            await _mailerSendService.Sendmail(mail);
            return await Task.FromResult(true);
        }
        #endregion
    }
}
