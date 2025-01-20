using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.Contracts;
using Nethereum.HdWallet;
using Nethereum.Hex.HexTypes;
using Nethereum.Model;
using Nethereum.RLP;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json;
using NoChainSwap.Domain.Impl.Models;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using NoChainSwap.Domain.Interfaces.Services;
using NoChainSwap.Domain.Interfaces.Services.Coins;
using NoChainSwap.DTO.Stacks;
using NoChainSwap.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Impl.Services.Coins
{
    [Event("Transfer")]
    public class TransferEventDTO : IEventDTO
    {
        [Parameter("address", "_from", 1, true)]
        public string From { get; set; }

        [Parameter("address", "_to", 2, true)]
        public string To { get; set; }

        [Parameter("uint256", "_value", 3, false)]
        public BigInteger Value { get; set; }
    }

    public class USDTTxService : IUSDTTxService
    {
        private const string MNEMONIC =
            "aunt federal magic they culture car primary maple snack misery dumb force " + 
            "three erosion vendor chair just twice blade front unhappy miss inject under";
        private readonly BigInteger CHAIN_ID = new BigInteger(97); // testenet
        private const string NODE_URL = "https://data-seed-prebsc-1-s1.binance.org:8545";
        private const string API_URL = "https://api-testnet.bscscan.com/api";
        private const string API_KEY = "VA5HUYT7BSTKMHN6UWFZ9Y1HYM1GYQ73PI";
        private const string CONTRACT_ADDRESS = "0x7ef95a0FEE0Dd31b22626fA2e10Ee6A223F8a684";
        private BigInteger _CurrentBlockNumber = 0;

        protected readonly ICoinMarketCapService _coinMarketCapService;
        protected readonly ITransactionDomainFactory _txFactory;
        protected readonly ITransactionLogDomainFactory _txLogFactory;

        public USDTTxService(ICoinMarketCapService coinMarketCapService, ITransactionDomainFactory txFactory, ITransactionLogDomainFactory txLogFactory)
        {
            _coinMarketCapService = coinMarketCapService;
            _txFactory = txFactory;
            _txLogFactory = txLogFactory;
        }

        public bool IsPaybackAutomatic()
        {
            return false;
        }

        public string ConvertToString(decimal coin)
        {
            return (coin / 100000000M).ToString("N5") + " USDT";
        }

        public string GetAddressUrl(string address)
        {
            return $"https://testnet.bscscan.com/address/{address}";
        }

        public CoinEnum GetCoin()
        {
            return CoinEnum.USDT;
        }

        public async Task<TxResumeInfo> GetResumeTransaction(string txId)
        {
            /*
            using (var client = new HttpClient())
            {
                string url = $"{API_URL}?module=transaction&action=tokentx" +
                    $"&contractaddress={CONTRACT_ADDRESS}" +
                    $"&txhash={txId}" +
                    $"&apikey={API_KEY}&sort=asc";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var txResponse = JsonConvert.DeserializeObject<TxEthResponseInfo>(responseBody);
                if (txResponse.Status == 1 && txResponse.Result.Count() > 0)
                {
                    var tx = txResponse.Result.FirstOrDefault();
                    return new TxResumeInfo
                    {
                        Amount = Convert.ToInt64(tx.Value),
                        Fee = tx.CumulativeGasUsed,
                        TxId = tx.Hash,
                        SenderAddress = tx.From,
                        Success = tx.Confirmations > 0
                    };
                }
                else
                {
                    throw new Exception(txResponse.Message);
                }
            }
             */

            var web3 = new Web3(NODE_URL);
            var tx = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txId);
            if (tx != null)
            {
                var log = tx.DecodeAllEvents<TransferEventDTO>().First();

                return new TxResumeInfo
                {
                    Amount = log.Event.Value.ToHexBigInteger().ToLong(),
                    Fee = tx.CumulativeGasUsed.ToLong(),
                    TxId = tx.TransactionHash,
                    SenderAddress = tx.From,
                    Success = tx.Succeeded()
                };

                //logs.First().Event.Value

                //var transferEvent = tx.Logs.First().DecodeEvent<TransferEventDTO>();
            }
            return null;
            //var txs = new List<TxDetectedInfo>();
        }

        /*
        public async Task<bool> IsTransactionSuccessful(string txid)
        {
            var web3 = new Web3(NODE_URL);
            var tx = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txid);
            return (tx != null && tx.Status.Value == 1);
        }

        public Task<long> GetSenderAmount(string txid, string senderAddr)
        {
            return Task<long>.FromResult<long>(0);
        }

        public async Task<int> GetFee(string txid)
        {
            var web3 = new Web3(NODE_URL);
            var tx = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txid);
            return (tx != null) ? (int)tx.CumulativeGasUsed.Value : 0; 
        }
        */

        public Task<string> GetPoolAddress()
        {
            return Task<string>.FromResult("");
        }

        public Task<string> GetNewAddress(int index)
        {
            var wallet = new Wallet(MNEMONIC, "");
            var account = wallet.GetAccount(index, CHAIN_ID);
            return Task.FromResult<string>(account.Address);
        }

        public Task<long> GetPoolBalance()
        {
            return Task<long>.FromResult<long>(0);
        }

        public string GetTransactionUrl(string txId)
        {
            return $"https://testnet.bscscan.com/tx/{txId}";
        }


        public Task<string> Transfer(string address, long amount)
        {
            return Task<string>.FromResult<string>("");
        }

        public Task<bool> VerifyTransaction(ITransactionModel tx)
        {
            return Task<bool>.FromResult<bool>(true);
        }

        public async Task<IList<TxDetectedInfo>> DetectNewTransactions(IList<string> addresses) {
            var txs = new List<TxDetectedInfo>();
            using (var client = new HttpClient())
            {
                string url = $"{API_URL}?module=account&action=tokentx" +
                    $"&contractaddress={CONTRACT_ADDRESS}" +
                    "&address=" + string.Join(',', addresses.ToArray()) +
                    "&sort=asc" +
                    $"&apikey={API_KEY}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                try
                {
                    var txResponse = JsonConvert.DeserializeObject<TxEthResponseInfo>(responseBody);
                    if (txResponse.Status == 1 && txResponse.Result.Count() > 0)
                    {
                        foreach (var tx in txResponse.Result)
                        {
                            txs.Add(new TxDetectedInfo
                            {
                                RecipientAddress = tx.To,
                                SenderAddress = tx.From,
                                SenderTxId = tx.Hash
                            });
                        }
                    }
                }
                catch {
                    Console.WriteLine("URL: " + url);
                    Console.WriteLine("Response:" + responseBody);
                    throw;
                }
            }
            return txs;
        }

        public void AddLog(long txId, string msg, LogTypeEnum t, ITransactionLogDomainFactory txLogFactory)
        {
            var currentDate = DateTime.Now;
            Console.WriteLine(string.Format("{0} - [{1}] {2}", currentDate.ToString("yyyy-MM-dd h:mm:ss"), t.ToString(), msg));

            var md = txLogFactory.BuildTransactionLogModel();
            md.TxId = txId;
            md.Date = currentDate;
            md.LogType = t;
            md.Message = msg;
            md.Insert();
        }
    }
}
