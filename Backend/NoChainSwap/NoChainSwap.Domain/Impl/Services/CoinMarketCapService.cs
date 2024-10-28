using NBitcoin;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Services;
using NoChainSwap.DTO.CoinMarketCap;
using NoChainSwap.DTO.Transaction;
using NoobsMuc.Coinmarketcap.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Impl.Services
{
    public class CoinMarketCapService : ICoinMarketCapService
    {
        //private const string API_KEY = "b54bcf4d-1bca-4e8e-9a24-22ff2c3d462c";
        private const string API_KEY = "7a2f3a1e-dac4-4a5a-8e38-8b535bedbe59";
        //private const decimal SPREAD = 0.05M;
        //private const string BTC_TO_STX_TEXT = "1 BTC = {0:N0} STX";
        //private const string STX_TO_BTC_TEXT = "1 STX = {0:N0} Satoshis";

        /*
        private readonly ICoinTxServiceFactory _coinFactory;

        public CoinMarketCapService(ICoinTxServiceFactory coinFactory)
        {
            _coinFactory = coinFactory;
        }
        */

        private CoinInfo CurrencyToCoin(Currency data)
        {
            return new CoinInfo
            {
                ConvertCurrency = data.ConvertCurrency,
                Id = data.Id,
                LastUpdated = data.LastUpdated,
                MarketCapConvert = data.MarketCapConvert,
                MarketCapUsd = data.MarketCapUsd,
                Name = data.Name,
                PercentChange1h = data.PercentChange1h,
                PercentChange24h = data.PercentChange24h,
                PercentChange7d = data.PercentChange7d,
                Price = data.Price,
                Rank = data.Rank,
                Symbol = data.Symbol,
                Volume24hUsd = data.Volume24hUsd
            };
        }

        public CoinSwapInfo GetCurrentPrice(CoinEnum senderCoin, CoinEnum receiverCoin)
        {
            //var senderService = _coinFactory.BuildCoinTxService(senderCoin);
            //var receiverService = _coinFactory.BuildCoinTxService(receiverCoin);

            var senderSymbol = Core.Utils.CoinToStr(senderCoin);
            var receiverSymbol = Core.Utils.CoinToStr(receiverCoin);

            var senderSlug = Core.Utils.CoinToSlug(senderCoin);
            var receiverSlug = Core.Utils.CoinToSlug(receiverCoin);

            var slugs = new string[2] { senderSlug, receiverSlug };
            var client = new CoinmarketcapClient(API_KEY);
            var data = client.GetCurrencyBySlugList(slugs, "USD");
            var senderPrice = data.Where(x => string.Compare(x.Symbol, senderSymbol, true) == 0).First().Price;
            var receiverPrice = data.Where(x => string.Compare(x.Symbol, receiverSymbol, true) == 0).First().Price;
            var senderProportion = senderPrice / receiverPrice;
            var receiverProportion = receiverPrice / senderPrice;
            return new CoinSwapInfo
            {
                SenderPrice = senderPrice,
                ReceiverPrice = receiverPrice,
                SenderProportion = senderProportion,
                ReceiverProportion = receiverProportion,
                Sender = CurrencyToCoin(data.First()),
                Receiver = CurrencyToCoin(data.Last())
            };
        }
    }
}
