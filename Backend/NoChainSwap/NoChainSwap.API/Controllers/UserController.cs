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
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
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
                Email = md.Email,
                IsAdmin = md.IsAdmin
            };
            return user;
        }

        [HttpGet("getbyid/{userId}")]
        public ActionResult<UserResult> GetById(long userId)
        {
            try
            {
                var user = _userService.GetUserByID(userId);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User Not Found" };
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

        [HttpGet("getbyaddress/{chainId}/{address}")]
        public ActionResult<UserResult> GetByAddress(int chainId, string address)
        {
            try
            {
                var user = _userService.GetUserByAddress((ChainEnum)chainId, address);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "Address Not Found" };
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

        [HttpGet("getbyemail/{email}")]
        public ActionResult<UserResult> GetByEmail(string email)
        {
            try
            {
                var user = _userService.GetUserByEmail(email);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User with email not found" };
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
                if (param == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User is empty" };
                }
                var user = _userService.Insert(new UserInfo
                {
                    Name = param.Name,
                    Email = param.Email
                });
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
        //[Authorize]
        public ActionResult<UserResult> Update(UserParam param)
        {
            try
            {

                //var userSession = _userService.GetUserInSession(HttpContext);
                if (param == null)
                {
                    //return StatusCode(401, "Not Authorized");
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User is empty" };
                }

                var user = _userService.Update(new UserInfo
                {
                    Id = param.Id,
                    Name = param.Name,
                    Email = param.Email,
                });
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

        [HttpPost("loginwithemail")]
        public ActionResult<UserResult> LoginWithEmail([FromBody]LoginParam param)
        {
            try
            {
                var user = _userService.LoginWithEmail(param.Email, param.Password);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "Email or password is wrong" };
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

        [HttpGet("haspassword/{userId}")]
        public ActionResult<StatusResult> HasPassword(long userId)
        {
            try
            {
                return new StatusResult
                {
                    Sucesso = _userService.HasPassword(userId),
                    Mensagem = "Password verify successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("changepassword")]
        public ActionResult<StatusResult> ChangePassword([FromBody]ChangePasswordParam param)
        {
            try
            {
                var user = _userService.GetUserByID(param.UserId);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "Email or password is wrong" };
                }
                _userService.ChangePassword(param.UserId, param.OldPassword, param.NewPassword);
                return new StatusResult
                {
                    Sucesso = true,
                    Mensagem = "Password changed successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("sendrecoverymail/{email}")]
        public async Task<ActionResult<StatusResult>> SendRecoveryMail(string email)
        {
            try
            {
                var user = _userService.GetUserByEmail(email);
                if (user == null)
                {
                    return new StatusResult
                    {
                        Sucesso = false,
                        Mensagem = "Email not exist"
                    };
                }
                await _userService.SendRecoveryEmail(email);
                return new StatusResult
                {
                    Sucesso = true,
                    Mensagem = "Recovery email sent successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("changepasswordusinghash")]
        public ActionResult<StatusResult> ChangePasswordUsingHash([FromBody] ChangePasswordUsingHashParam param)
        {
            try
            {
                _userService.ChangePasswordUsingHash(param.RecoveryHash, param.NewPassword);
                return new StatusResult
                {
                    Sucesso = true,
                    Mensagem = "Password changed successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
