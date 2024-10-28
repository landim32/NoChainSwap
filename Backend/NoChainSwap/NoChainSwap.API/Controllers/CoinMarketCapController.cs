using NoChainSwap.Domain.Interfaces.Services;
using NoChainSwap.DTO.CoinMarketCap;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using NoChainSwap.Domain.Impl.Core;

namespace NoChainSwap.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CoinMarketCapController: Controller
    {
        private IUserService _userService;
        private ICoinMarketCapService _coinMarketCap;

        public CoinMarketCapController(IUserService userService, ICoinMarketCapService coinMarketCap)
        {
            _userService = userService;
            _coinMarketCap = coinMarketCap;
        }

        [HttpGet("getcurrentprice/{sender}/{receiver}")]
        public ActionResult<CoinSwapInfo> GetCurrentPrice(string sender, string receiver)
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
                var coinSender = Utils.StrToCoin(sender);
                var coinReceiver = Utils.StrToCoin(receiver);
                return _coinMarketCap.GetCurrentPrice(coinSender, coinReceiver);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
