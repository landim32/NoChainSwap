using NoChainSwap.Domain.Interfaces.Services.Coins;
using NoChainSwap.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Interfaces.Factory
{
    public interface ICoinTxServiceFactory
    {
        ICoinTxService BuildCoinTxService(CoinEnum coin);
    }
}
