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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NoChainSwap.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{chainId}/{address}")]
        public ActionResult<UserResult> Get(int chainId, string address)
        {
            try
            {
                var user = _userService.GetUserByAddress((ChainEnum)chainId, address);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = true, Mensagem = "Address Not Found" };
                }
                return new UserResult()
                {
                    User = new UserInfo()
                    {
                        Id = user.Id,
                        Hash = user.Hash,
                        ChainId = chainId,
                        Address = address
                    }
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("checkUserRegister/{chainId}/{address}")]
        public ActionResult<UserResult> CheckUserRegister(int chainId, string address)
        {
            try
            {
                //Console.WriteLine("Chegou aqui");
                var user = _userService.GetUserByAddress((ChainEnum)chainId, address);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = true, Mensagem = "BTC Address Not Found" };
                }
                return new UserResult()
                {
                    User = new UserInfo()
                    {
                        Id = user.Id,
                        Hash = user.Hash,
                        ChainId = chainId,
                        Address = address
                    }
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<UserResult> Post(UserParam param)
        {
            try
            {
                if(String.IsNullOrEmpty(param.Address))
                    return StatusCode(400, "Address is empty");

                var user = _userService.CreateNewUser(new UserInfo
                {
                    Address = param.Address
                });
                return new UserResult()
                {
                    User = new UserInfo()
                    {
                        Id = user.Id,
                        Hash = user.Hash,
                        ChainId = param.ChainId,
                        Address = param.Address
                    }
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        public ActionResult<UserResult> UpdateUser(UserParam param)
        {
            try
            {

                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }

                var user = _userService.UpdateUser(new UserInfo
                {
                    Name = userSession.Name,
                    Email = userSession.Email,
                });
                return new UserResult()
                {
                    User = new UserInfo()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email
                    }
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
