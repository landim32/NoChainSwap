using Nethereum.HdWallet;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using NoChainSwap.Domain.Interfaces.Services;
using NoChainSwap.Domain.Interfaces.Services.Coins;
using NoChainSwap.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public USDTTxService(ICoinMarketCapService coinMarketCapService, ITransactionDomainFactory txFactory, ITransactionLogDomainFactory txLogFactory)
        {
            _coinMarketCapService = coinMarketCapService;
            _txFactory = txFactory;
            _txLogFactory = txLogFactory;
        }

        public override string ConvertToString(decimal coin)
        {
            return (coin / 100000000M).ToString("N2") + " USDT";
        }

        public override string GetAddressUrl(string address)
        {
            return "";
        }

        public override CoinEnum GetCoin()
        {
            return CoinEnum.BRL;
        }

        public override Task<int> GetFee(string txid)
        {
            return Task.FromResult(0);
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
            return "";
        }

        public override Task<bool> IsTransactionSuccessful(string txid)
        {
            return Task<bool>.FromResult<bool>(false);
        }

        public override Task<string> Transfer(string address, long amount)
        {
            return Task<string>.FromResult<string>("");
        }

        public override Task<bool> VerifyTransaction(ITransactionModel tx)
        {
            return Task<bool>.FromResult<bool>(false);
        }
    }
}
