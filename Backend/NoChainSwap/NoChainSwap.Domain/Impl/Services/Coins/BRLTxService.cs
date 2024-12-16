using NBitcoin;
using Nethereum.HdWallet;
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
using System.Text;  
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Impl.Services.Coins
{

    public class BRLTxService : CoinTxService, IBRLTxService
    {
        private const string CHAVE_PIX =
            "00020126360014BR.GOV.BCB.PIX0114+55619987525885204000053039865802BR5923" +
            "RODRIGO LANDIM CARNEIRO6008BRASILIA622605224FxfGt3HNLyWnYNzHFy6wZ6304F090";

        public BRLTxService(ICoinMarketCapService coinMarketCapService, ITransactionDomainFactory txFactory, ITransactionLogDomainFactory txLogFactory)
        {
            _coinMarketCapService = coinMarketCapService;
            _txFactory = txFactory;
            _txLogFactory = txLogFactory;
        }

        public override string ConvertToString(decimal coin)
        {
            return (coin / 100000000M).ToString("N2");
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

        public override Task<string> GetNewAddress(int index)
        {
            return Task<string>.FromResult(CHAVE_PIX);
        }

        public override Task<string> GetPoolAddress()
        {
            return Task<string>.FromResult("");
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
            return Task<bool>.FromResult<bool>(true);
        }
    }
}
