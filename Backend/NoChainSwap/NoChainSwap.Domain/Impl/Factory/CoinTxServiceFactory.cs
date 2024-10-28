using NoChainSwap.Domain.Impl.Services.Coins;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Services.Coins;
using NoChainSwap.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Impl.Factory
{
    public class CoinTxServiceFactory: ICoinTxServiceFactory
    {
        protected readonly IBtcTxService _btcTxService;
        protected readonly IStxTxService _stxTxService;

        public CoinTxServiceFactory(IBtcTxService btcTxService, IStxTxService stxTxService)
        {
            _btcTxService = btcTxService;
            _stxTxService = stxTxService;
        }

        public ICoinTxService BuildCoinTxService(CoinEnum coin)
        {
            ICoinTxService coinTxService = null;
            switch (coin)
            {
                case CoinEnum.Bitcoin:
                    coinTxService = _btcTxService;
                    break;
                case CoinEnum.Stacks:
                    coinTxService = _stxTxService;
                    break;
            }
            return coinTxService;
        }
    }
}
