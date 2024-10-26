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
    public class StxTxService//: CoinTxService, IStxTxService
    {
        private const string TX_STATUS_SUCESS = "success";
        /*
        public StxTxService(
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
        */

    }
}
