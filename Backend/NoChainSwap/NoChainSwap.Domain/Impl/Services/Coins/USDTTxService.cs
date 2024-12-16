using NBitcoin;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.HdWallet;
using Nethereum.Web3;
using Newtonsoft.Json;
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
    public class USDTTxService : CoinTxService, IUSDTTxService
    {
        private const string MNEMONIC =
            "aunt federal magic they culture car primary maple snack misery dumb force " + 
            "three erosion vendor chair just twice blade front unhappy miss inject under";
        private readonly BigInteger CHAIN_ID = new BigInteger(97); // testenet
        private const string NODE_URL = "https://data-seed-prebsc-1-s1.binance.org:8545";

        public USDTTxService(ICoinMarketCapService coinMarketCapService, ITransactionDomainFactory txFactory, ITransactionLogDomainFactory txLogFactory)
        {
            _coinMarketCapService = coinMarketCapService;
            _txFactory = txFactory;
            _txLogFactory = txLogFactory;
        }

        public override string ConvertToString(decimal coin)
        {
            return (coin / 100000000M).ToString("N5") + " USDT";
        }

        public override string GetAddressUrl(string address)
        {
            return $"https://testnet.bscscan.com/address/{address}";
        }

        public override CoinEnum GetCoin()
        {
            return CoinEnum.USDT;
        }

        public override async Task<int> GetFee(string txid)
        {
            var web3 = new Web3(NODE_URL);
            var tx = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txid);
            return (tx != null) ? (int)tx.CumulativeGasUsed.Value : 0; 
        }

        public override Task<string> GetPoolAddress()
        {
            return Task<string>.FromResult("");
        }

        public override Task<string> GetNewAddress(int index)
        {
            var wallet = new Wallet(MNEMONIC, "");
            var account = wallet.GetAccount(index, CHAIN_ID);
            return Task.FromResult<string>(account.Address);
        }

        public override Task<long> GetPoolBalance()
        {
            return Task<long>.FromResult<long>(0);
        }

        public override Task<long> GetSenderAmount(string txid, string senderAddr)
        {
            return Task<long>.FromResult<long>(0);
        }

        public override string GetSwapDescription(decimal proportion)
        {
            return "";
        }

        public override string GetTransactionUrl(string txId)
        {
            return $"https://testnet.bscscan.com/tx/{txId}";
        }

        public override async Task<bool> IsTransactionSuccessful(string txid)
        {
            var web3 = new Web3(NODE_URL);
            var tx = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txid);
            return (tx != null && tx.Status.Value == 1);
        }

        public override Task<string> Transfer(string address, long amount)
        {
            return Task<string>.FromResult<string>("");
        }

        public override Task<bool> VerifyTransaction(ITransactionModel tx)
        {
            return Task<bool>.FromResult<bool>(true);
        }
    }
}
