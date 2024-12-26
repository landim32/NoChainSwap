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
using DB.Infra.Context;
using NoChainSwap.Domain.Interfaces.Factory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NoChainSwap.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IUserDomainFactory _userFactory;

        public UserController(IUserService userService, IUserDomainFactory userFactory)
        {
            _userService = userService;
            _userFactory = userFactory;
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

        [HttpGet("gettokenunauthorized/{chainId}/{address}")]
        public ActionResult<UserTokenResult> GetTokenUnauthorized(int chainId, string address)
        {
            try
            {
                var user = _userService.GetUserByAddress((ChainEnum) chainId, address);
                if (user == null)
                {
                    return new UserTokenResult() { Sucesso = false, Mensagem = "User Not Found" };
                }
                if (user.IsAdmin)
                {
                    return new UserTokenResult() { Sucesso = false, Mensagem = "Cant get token for admin" };
                }

                return new UserTokenResult()
                {
                    Token = user.GenerateNewToken(_userFactory)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("gettokenauthorized")]
        public ActionResult<UserTokenResult> GetTokenAuthorized([FromBody] LoginParam login)
        {
            try
            {
                var user = _userService.LoginWithEmail(login.Email, login.Password);
                if (user == null)
                {
                    return new UserTokenResult() {Sucesso = false, Mensagem = "Email or password is wrong" };
                }
                return new UserTokenResult()
                {
                    Token = user.GenerateNewToken(_userFactory)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getme")]
        [Authorize]
        public ActionResult<UserResult> GetMe()
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.Id);
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
        public ActionResult<UserResult> Insert([FromBody] UserParam param)
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

        [Authorize]
        [HttpPost("update")]
        public ActionResult<UserResult> Update(UserParam param)
        {
            try
            {
                if (param == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User is empty" };
                }
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                if (userSession.Id != param.Id)
                {
                    throw new Exception("Only can update your user");
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

        [Authorize]
        [HttpGet("haspassword")]
        public ActionResult<StatusResult> HasPassword()
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.Id);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User Not Found" };
                }
                return new StatusResult
                {
                    Sucesso = _userService.HasPassword(user.Id),
                    Mensagem = "Password verify successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("changepassword")]
        public ActionResult<StatusResult> ChangePassword([FromBody]ChangePasswordParam param)
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.Id);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "Email or password is wrong" };
                }
                _userService.ChangePassword(user.Id, param.OldPassword, param.NewPassword);
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
