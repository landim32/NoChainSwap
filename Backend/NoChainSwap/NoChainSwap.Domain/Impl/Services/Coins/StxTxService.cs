using NBitcoin;
using Nethereum.HdWallet;
using Newtonsoft.Json;
using NoChainSwap.Domain.Impl.Models;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using NoChainSwap.Domain.Interfaces.Services;
using NoChainSwap.Domain.Interfaces.Services.Coins;
using NoChainSwap.DTO.Mempool;
using NoChainSwap.DTO.Stacks;
using NoChainSwap.DTO.Transaction;
using Org.BouncyCastle.Asn1.Tsp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static NBitcoin.Protocol.Behaviors.ChainBehavior;

namespace NoChainSwap.Domain.Impl.Services.Coins
{
    public class StxTxService: IStxTxService
    {
        protected readonly ICoinMarketCapService _coinMarketCapService;
        protected readonly ITransactionDomainFactory _txFactory;
        protected readonly ITransactionLogDomainFactory _txLogFactory;

        public static string WALLET_API { get; set; }
        public static string STACKS_API { get; set; }

        private const string TX_STATUS_SUCESS = "success";

        //private TxInfo _txInfo;

        public StxTxService(ICoinMarketCapService coinMarketCapService, ITransactionDomainFactory txFactory, ITransactionLogDomainFactory txLogFactory)
        {
            _coinMarketCapService = coinMarketCapService;
            _txFactory = txFactory;
            _txLogFactory = txLogFactory;
        }

        public bool IsPaybackAutomatic()
        {
            return true;
        }
        public string ConvertToString(decimal coin)
        {
            return (coin / 100000000M).ToString("N5") + " STX";
        }

        public string GetAddressUrl(string address)
        {
            return $"https://explorer.hiro.so/address/{address}?chain=testnet";
        }

        public CoinEnum GetCoin()
        {
            return CoinEnum.Stacks;
        }

        private async Task<TxInfo> GetTransaction(string txId)
        {
            using (var client = new HttpClient())
            {
                string url = $"{STACKS_API}/v1/tx/{txId}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<TxInfo>(responseBody);
            }
        }

        public async Task<TxResumeInfo> GetResumeTransaction(string txId)
        {
            var txInfo = await GetTransaction(txId);

            long amount = 0;
            if (!long.TryParse(txInfo.TokenTransfer.Amount, out amount))
            {
                throw new Exception($"{txInfo.TokenTransfer.Amount} is not a valid amount");
            }

            int fee = 0;
            if (!int.TryParse(txInfo.FeeRate, out fee))
            {
                throw new Exception($"Cant convert fee to number ({txInfo.FeeRate})");
            }
            return new TxResumeInfo
            {
                Amount = amount * 100,
                Fee = fee * 100,
                TxId = txId,
                SenderAddress = txInfo.SenderAddress,
                Success = string.Compare(txInfo.TxStatus, TX_STATUS_SUCESS, true) == 0
            };
        }

        /*
        private async Task<TxInfo> LoadTransaction(string txId)
        {
            if (_txInfo == null || string.Compare(_txInfo.TxId, txId, true) != 0)
            {
                _txInfo = await GetTransaction(txId);
            }
            return _txInfo;
        }

        public async Task<bool> IsTransactionSuccessful(string txid)
        {
            var txInfo = await LoadTransaction(txid);
            if (txInfo == null)
            {
                throw new Exception($"Dont find transaction on STX API ({txid})");
            }
            return string.Compare(txInfo.TxStatus, TX_STATUS_SUCESS, true) == 0;
        }

        public async Task<long> GetSenderAmount(string txid, string senderAddr)
        {
            var txInfo = await LoadTransaction(txid);
            if (txInfo == null)
            {
                throw new Exception($"Dont find transaction on STX API ({txid})");
            }
            long amount = 0;
            if (!long.TryParse(txInfo.TokenTransfer.Amount, out amount))
            {
                throw new Exception($"{txInfo.TokenTransfer.Amount} is not a valid amount");
            }
            return amount * 100;
        }

        public async Task<int> GetFee(string txid)
        {
            var txInfo = await LoadTransaction(txid);
            if (txInfo == null)
            {
                throw new Exception($"Dont find transaction on STX API ({txid})");
            }
            var feeStr = txInfo.FeeRate;
            int fee = 0;
            if (!int.TryParse(feeStr, out fee))
            {
                throw new Exception($"Cant convert fee to number ({feeStr})");
            }
            return fee * 100;
        }
        */

        private async Task<long> GetBalance(string stxAddress)
        {
            using (var client = new HttpClient())
            {
                string url = $"{STACKS_API}/v1/address/{stxAddress}/stx";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var balance = JsonConvert.DeserializeObject<StxBalanceInfo>(responseBody);
                long balanceLng = 0;
                if (!long.TryParse(balance.Balance, out balanceLng))
                {
                    throw new Exception(String.Format("Invalid balance ({0}).", balance.Balance));
                }
                return balanceLng * 100;
            }
        }

        public async Task<string> GetNewAddress(int index)
        {
            using (var client = new HttpClient())
            {
                string url = $"{WALLET_API}/new-address/{index}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<string>(responseBody);
            }
        }

        public async Task<string> GetPoolAddress()
        {
            using (var client = new HttpClient())
            {
                string url = $"{WALLET_API}/get-address";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<string>(responseBody);
            }
        }

        public async Task<long> GetPoolBalance()
        {
            return await GetBalance(await GetPoolAddress());
        }

        public string GetTransactionUrl(string txId)
        {
            return $"https://explorer.hiro.so/txid/{txId}?chain=testnet";
        }

        public async Task<string> Transfer(string address, long amount)
        {
            using (var client = new HttpClient())
            {
                string url = $"{WALLET_API}/transfer";
                TransferParamInfo param = new TransferParamInfo
                {
                    recipientAddress = address,
                    amount = amount / 100
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(url, jsonContent);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var txHandle = JsonConvert.DeserializeObject<TxHandleInfo>(responseBody);
                if (!string.IsNullOrEmpty(txHandle.Error))
                {
                    throw new Exception(string.Format("{0} ({1})", txHandle.Error, txHandle.Reason));
                }
                if (!string.IsNullOrEmpty(txHandle.TxId))
                {
                    return await Task.FromResult(txHandle.TxId);
                }
                return await Task.FromResult(string.Empty);
            }
        }

        public async Task<bool> VerifyTransaction(ITransactionModel tx)
        {
            if (string.IsNullOrEmpty(tx.SenderTxid) && string.IsNullOrEmpty(tx.ReceiverTxid))
            {
                AddLog(tx.TxId, "Transaction tx_id is empty", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(false);
            }
            var stxTx = await GetTransaction(tx.SenderTxid);
            if (stxTx == null)
            {
                AddLog(tx.TxId, "Dont find transaction on mempool", LogTypeEnum.Warning, _txLogFactory);
                return await Task.FromResult(false);
            }
            var poolAddr = await GetPoolAddress();
            if (string.Compare(poolAddr, stxTx.TokenTransfer.RecipientAddress, true) != 0)
            {
                AddLog(tx.TxId, "Pool address not in transaction", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(false);
            }
            if (string.Compare(tx.SenderAddress, stxTx.SenderAddress, true) != 0)
            {
                AddLog(tx.TxId, "Sender address not in transaction", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(false);
            }
            long amount = 0;
            if (!long.TryParse(stxTx.TokenTransfer.Amount, out amount))
            {
                AddLog(tx.TxId, $"{stxTx.TokenTransfer.Amount} is not a valid amount", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(false);
            }
            if (amount <= 0)
            {
                AddLog(tx.TxId, "Pool did not receive any value", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }
        public Task<IList<TxDetectedInfo>> DetectNewTransactions(IList<string> addresses)
        {
            return Task.FromResult<IList<TxDetectedInfo>>(new List<TxDetectedInfo>());
        }

        public void AddLog(long txId, string msg, LogTypeEnum t, ITransactionLogDomainFactory txLogFactory)
        {
            var md = txLogFactory.BuildTransactionLogModel();
            md.TxId = txId;
            md.Date = DateTime.Now;
            md.LogType = t;
            md.Message = msg;
            md.Insert();
        }

    }
}
