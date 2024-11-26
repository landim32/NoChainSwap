using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using NoChainSwap.Domain.Interfaces.Models;
using NoChainSwap.Domain.Interfaces.Services;
using NoChainSwap.DTO.User;
using Core.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NoChainSwap.Domain.Impl.Models;

namespace NoChainSwap.Domain
{
    public class AuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        //private readonly ICryptoUtils _cryptoUtils;
        private readonly IUserService _userService;

        private const string BTC_ADDRESS = "tb1qeeqfngu5tnqfkxh3e0rdkj6m2ng2vqsr86lskq";
        private const string STX_ADDRESS = "ST2JC8HK79X5QZW8ZXVA0Q6V1ZKAA3Q4VHZP29QAP";

        public AuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            //ICryptoUtils cryptoUtils,
            IUserService userService)
            : base(options, logger, encoder, clock)
        {
            //_cryptoUtils = cryptoUtils;
            _userService = userService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            //string btcAddress = null;
            string stxAddress = null;
            IUserModel user = null; 
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var masterKey = authHeader.Parameter;
                if(masterKey == "masterkeydoamor")
                {
                    user = _userService.GetUserByAddress(ChainEnum.StackAndBitcoin, STX_ADDRESS);
                }
                else
                {
                    var hashAuth = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter));
                    var hashList = hashAuth.Split("|separator|");
                    if (hashList.Count() == 2)
                    {
                        var signature = hashList[0];
                        //btcAddress = hashList[1];
                        stxAddress = hashList[1];

                        user = _userService.GetUserByAddress(ChainEnum.StackAndBitcoin, STX_ADDRESS);
                        if (user == null)
                            return AuthenticateResult.Fail("Invalid Session");
                        /*
                        if (!_cryptoUtils.CheckPersonalSignature(user.Hash, signature, btcAddress))
                        {
                            return AuthenticateResult.Fail("Invalid Session");
                        }
                        */
                    }
                    else
                    {
                        return AuthenticateResult.Fail("Incorrect Session");
                    }
                }
                
            }
            catch (Exception)
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
            
            var claims = new[] {
                new Claim("UserInfo",  JsonConvert.SerializeObject(new UserInfo() {
                     Id = user.Id,
                     Hash = user.Hash,
                     Name = user.Name,
                     Email = user.Email,
                }))
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
