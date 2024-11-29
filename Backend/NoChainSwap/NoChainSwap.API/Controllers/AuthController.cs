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

        private UserInfo ModelToInfo(IUserModel md)
        {
            var user = new UserInfo { 
                Id = md.Id,
                Hash = md.Hash,
                CreateAt = md.CreateAt,
                UpdateAt = md.UpdateAt,
                Name = md.Name,
                Email = md.Email
            };
            user.Addresses = _userService
                .ListAddressByUser(user.Id)
                .Select(x => new UserAddressInfo
                {
                    Id = x.Id,
                    ChainId = (int) x.Chain,
                    CreateAt = x.CreateAt,
                    UpdateAt = x.UpdateAt,
                    Address = x.Address
                })
                .ToList();
            return user;
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
                    User = ModelToInfo(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getByEmail/{email}")]
        public ActionResult<UserResult> GetByEmail(string email)
        {
            try
            {
                var user = _userService.GetUserByEmail(email);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = true, Mensagem = "User with email not found" };
                }
                return new UserResult()
                {
                    User = ModelToInfo(user)
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
                    User = ModelToInfo(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("insert")]
        public ActionResult<UserResult> Insert(UserParam param)
        {
            try
            {
                //if(String.IsNullOrEmpty(param.Address))
                //    return StatusCode(400, "Address is empty");

                var user = _userService.Insert(new UserInfo
                {
                    Name = param.Name,
                    Email = param.Email
                });
                _userService.AddOrChangeAddress(user.Id, (ChainEnum)param.ChainId, param.Address);
                return new UserResult()
                {
                    User = ModelToInfo(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("update")]
        [Authorize]
        public ActionResult<UserResult> Update(UserParam param)
        {
            try
            {

                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }

                var user = _userService.Update(new UserInfo
                {
                    Id = userSession.Id,
                    Name = userSession.Name,
                    Email = userSession.Email,
                });
                _userService.AddOrChangeAddress(user.Id, (ChainEnum)param.ChainId, param.Address);
                return new UserResult()
                {
                    User = ModelToInfo(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
