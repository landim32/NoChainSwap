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
        IUserModel Insert(UserInfo user);
        IUserModel Update(UserInfo user);
        IUserModel GetUserByEmail(string email);
        IUserModel GetUserByAddress(ChainEnum chain, string address);
        IEnumerable<IUserModel> GetAllUserAddress();
        IUserModel GetUserByID(long userId);
        IUserModel GetUserHash(ChainEnum chain, string address);
        UserInfo GetUserInSession(HttpContext httpContext);

        IEnumerable<IUserAddressModel> ListAddressByUser(long userId);
        IUserAddressModel GetAddressByChain(long userId, ChainEnum chain);
        void AddOrChangeAddress(long userId, ChainEnum chain, string address);
        void RemoveAddress(long userId, ChainEnum chain);
    }
}
