using NoChainSwap.DTO.CoinMarketCap;
using NoChainSwap.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Interfaces.Services
{
    public interface ICoinMarketCapService
    {
        CoinSwapInfo GetCurrentPrice(CoinEnum senderCoin, CoinEnum receiverCoin);
    }
}
