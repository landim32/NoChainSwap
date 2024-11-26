using System;
using System.Collections.Generic;
using NoChainSwap.Domain.Interfaces.Models;
using NoChainSwap.DTO.User;
using Microsoft.AspNetCore.Http;
using NoChainSwap.Domain.Impl.Models;

namespace NoChainSwap.Domain.Interfaces.Services
{
    public interface IUserService
    {
        IUserModel CreateNewUser(UserInfo user);
        IUserModel UpdateUser(UserInfo user);
        IUserModel GetUserByAddress(ChainEnum chain, string address);
        IEnumerable<IUserModel> GetAllUserAddress();
        IUserModel GetUSerByID(long userId);
        IUserModel GetUserHash(ChainEnum chain, string address);
        UserInfo GetUserInSession(HttpContext httpContext);

    }
}
