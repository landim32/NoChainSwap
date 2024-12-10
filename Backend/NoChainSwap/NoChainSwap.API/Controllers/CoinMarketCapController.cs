using NoChainSwap.Domain.Interfaces.Services;
using NoChainSwap.DTO.CoinMarketCap;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using NoChainSwap.Domain.Impl.Core;
using NoChainSwap.Domain.Impl.Services;
using NoChainSwap.DTO;
using System.Threading.Tasks;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.DTO.Transaction;

namespace NoChainSwap.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CoinMarketCapController: Controller
    {
        private const bool SHOW_POOL_BALANCE = false;

        protected readonly IUserService _userService;
        protected readonly ICoinMarketCapService _coinMarketCap;
        protected readonly ICoinTxServiceFactory _coinFactory;

        public CoinMarketCapController(
            IUserService userService, 
            ICoinMarketCapService coinMarketCap,
            ICoinTxServiceFactory coinFactory
        )
        {
            _userService = userService;
            _coinMarketCap = coinMarketCap;
            _coinFactory = coinFactory;
        }

        [HttpGet("getcurrentprice/{sender}/{receiver}")]
        public async Task<ActionResult<CoinSwapInfo>> GetCurrentPrice(string sender, string receiver)
        {
            try
            {
                /*
                var user = _userService.GetUserInSession(HttpContext);
                if (user == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                */
                var coinSender = Utils.StrToCoin(sender.ToLower());
                var coinReceiver = Utils.StrToCoin(receiver.ToLower());
                //var currencyValue = Utils.StrToCurrency(currency.ToUpper());

                var senderService = _coinFactory.BuildCoinTxService(coinSender);
                var receiverService = _coinFactory.BuildCoinTxService(coinReceiver);

                var price = _coinMarketCap.GetCurrentPrice(coinSender, coinReceiver, CurrencyEnum.USD);
                if (price != null && SHOW_POOL_BALANCE)
                {
                    price.SenderPoolAddr = await senderService.GetPoolAddress();
                    price.ReceiverPoolAddr = await receiverService.GetPoolAddress();
                    price.SenderPoolBalance = await senderService.GetPoolBalance();
                    price.ReceiverPoolBalance = await receiverService.GetPoolBalance();
                }
                return price;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
