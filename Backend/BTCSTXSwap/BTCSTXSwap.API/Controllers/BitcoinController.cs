﻿using BTCSTXSwap.Domain.Interfaces.Services;
using BTCSTXSwap.DTO.CoinMarketCap;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BTCSTXSwap.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class BitcoinController: Controller
    {
        private IUserService _userService;
        private IBitcoinService _bitcoinService;

        public BitcoinController(IUserService userService, IBitcoinService bitcoinService)
        {
            _userService = userService;
            _bitcoinService = bitcoinService;
        }

        [HttpGet("getaddress")]
        public ActionResult<string> GetAddress()
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
                return _bitcoinService.GetPoolAddress();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getbalance")]
        public async Task<ActionResult<long>> GetBalance()
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
                return await _bitcoinService.GetBalance(_bitcoinService.GetPoolAddress());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
