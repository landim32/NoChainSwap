﻿using NoChainSwap.Domain.Interfaces.Services;
using NoChainSwap.DTO.CoinMarketCap;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

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

        [HttpGet("getcurrentprice")]
        public ActionResult<CoinSwapInfo> GetCurrentPrice()
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
                return _coinMarketCap.GetCurrentPrice("bitcoin", "stacks");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
