using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using NoChainSwap.API.DTO;
using NoChainSwap.Domain.Interfaces.Services;
using NoChainSwap.DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoChainSwap.Domain.Impl.Models;
using NoChainSwap.Domain.Interfaces.Models;
using NoChainSwap.DTO.Domain;
using NoChainSwap.Domain.Impl.Services;
using System.Runtime.CompilerServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NoChainSwap.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAddressController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserAddressController(IUserService userService)
        {
            _userService = userService;
        }

        private UserAddressInfo ModelToInfo(IUserAddressModel md)
        {
            var userAddr = new UserAddressInfo { 
                Id = md.Id,
                ChainId = (int)md.Chain,
                CreateAt = md.CreateAt,
                UpdateAt = md.UpdateAt,
                Address = md.Address
            };
            return userAddr;
        }

        [HttpGet("listaddressbyuser/{userId}")]
        public ActionResult<UserAddressListResult> ListAddressByUser(long userId)
        {
            try
            {
                var userAddrs = _userService.ListAddressByUser(userId);
                if (userAddrs == null)
                {
                    return new UserAddressListResult() { UserAddresses = null, Sucesso = false, Mensagem = "User Addresses Not Found" };
                }
                return new UserAddressListResult()
                {
                    UserAddresses = userAddrs.Select(x => ModelToInfo(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getaddressbychain/{userId}/{chainId}")]
        public ActionResult<UserAddressResult> ListAddressByUser(long userId, int chainId)
        {
            try
            {
                var userAddr = _userService.GetAddressByChain(userId, (ChainEnum)chainId);
                if (userAddr == null)
                {
                    return new UserAddressResult() { UserAddress = null, Sucesso = false, Mensagem = "User Addresses Not Found" };
                }
                return new UserAddressResult()
                {
                    UserAddress = ModelToInfo(userAddr)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("addorchangeaddress")]
        public ActionResult<StatusResult> AddOrChangeAddress([FromBody] UserAddressParam param)
        {
            try
            {
                _userService.AddOrChangeAddress(param.UserId, (ChainEnum)param.ChainId, param.Address);
                return new StatusResult
                {
                    Sucesso = true,
                    Mensagem = "Address add successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("removeaddress/{userId}/{chainId}")]
        public ActionResult<StatusResult> RemoveAddress(long userId, int chainId)
        {
            try
            {
                _userService.RemoveAddress(userId, (ChainEnum)chainId);
                return new StatusResult
                {
                    Sucesso = true,
                    Mensagem = "Address remove successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
