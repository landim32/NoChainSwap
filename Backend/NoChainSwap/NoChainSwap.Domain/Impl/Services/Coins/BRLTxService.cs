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

    public class BRLTxService : IBRLTxService
    {
        private const string CHAVE_PIX =
            "00020126360014BR.GOV.BCB.PIX0114+55619987525885204000053039865802BR5923" +
            "RODRIGO LANDIM CARNEIRO6008BRASILIA622605224FxfGt3HNLyWnYNzHFy6wZ6304F090";

        protected readonly ICoinMarketCapService _coinMarketCapService;
        protected readonly ITransactionDomainFactory _txFactory;
        protected readonly ITransactionLogDomainFactory _txLogFactory;

        public BRLTxService(ICoinMarketCapService coinMarketCapService, ITransactionDomainFactory txFactory, ITransactionLogDomainFactory txLogFactory)
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
            return "R$ " + (coin / 100000000M).ToString("N2");
        }

        public string GetAddressUrl(string address)
        {
            return "";
        }

        public CoinEnum GetCoin()
        {
            return CoinEnum.BRL;
        }

        public Task<TxResumeInfo> GetResumeTransaction(string txId)
        {
            return Task.FromResult(new TxResumeInfo());
        }

        public Task<string> GetNewAddress(int index)
        {
            return Task<string>.FromResult(CHAVE_PIX);
        }

        public Task<string> GetPoolAddress()
        {
            return Task<string>.FromResult("");
        }

        public Task<long> GetPoolBalance()
        {
            return Task<long>.FromResult<long>(0);
        }

        /*
        public string GetSwapDescription(decimal proportion)
        {
            return "";
        }
        */

        public string GetTransactionUrl(string txId)
        {
            return "";
        }

        public Task<string> Transfer(string address, long amount)
        {
            return Task<string>.FromResult<string>("");
        }

        public Task<bool> VerifyTransaction(ITransactionModel tx)
        {
            return Task<bool>.FromResult<bool>(true);
        }

        public Task<IList<TxDetectedInfo>> DetectNewTransactions(string address)
        {
            return Task.FromResult<IList<TxDetectedInfo>>(new List<TxDetectedInfo>());
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
