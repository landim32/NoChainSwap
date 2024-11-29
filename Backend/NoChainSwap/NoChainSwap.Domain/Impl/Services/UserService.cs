using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using NoChainSwap.Domain.Interfaces.Services;
using NoChainSwap.DTO.User;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NoChainSwap.Domain.Impl.Models;

namespace NoChainSwap.Domain.Impl.Services
{
    public class UserService : IUserService
    {
        private readonly IUserDomainFactory _userFactory;
        private readonly IUserAddressDomainFactory _userAddressFactory;

        public UserService(IUserDomainFactory userFactory, IUserAddressDomainFactory userAddressFactory)
        {
            _userFactory = userFactory;
            _userAddressFactory = userAddressFactory;
        }

        public IUserModel Insert(UserInfo user)
        {
            var model = _userFactory.BuildUserModel();
            if (!string.IsNullOrEmpty(user.Email))
            {
                var userWithEmail = model.GetByEmail(user.Email, _userFactory);
                if (userWithEmail != null)
                {
                    throw new Exception("User with email already registered");
                }
            }

            model.Name = user.Name;
            model.Email = user.Email;
            model.CreateAt = DateTime.Now;
            model.UpdateAt = DateTime.Now;
            model.Hash = GetUniqueToken();

            model.Save(_userFactory);

            return model;
        }

        public IUserModel Update(UserInfo user)
        {
            IUserModel model = null;
            if (!(user.Id > 0))
            {
                throw new Exception("User not found");
            }
            model = _userFactory.BuildUserModel().GetById(user.Id, _userFactory);
            if (model == null)
            {
                throw new Exception("User not exists");
            }
            model.Name = user.Name;
            model.Email = user.Email;
            model.UpdateAt = DateTime.Now;
            model.Update(_userFactory);
            return model;
        }

        public IUserModel GetUserByEmail(string email)
        {
            return _userFactory.BuildUserModel().GetByEmail(email, _userFactory);
        }

        public IUserModel GetUserByAddress(ChainEnum chain, string address)
        {
            return _userFactory.BuildUserModel().GetByAddress(chain, address, _userFactory);
        }

        public IUserModel GetUserByID(long userId)
        {
            return _userFactory.BuildUserModel().GetById(userId, _userFactory);
        }

        public IUserModel GetUserHash(ChainEnum chain, string address)
        {
            var user = _userFactory.BuildUserModel().GetByAddress(chain, address, _userFactory);
            if (user != null)
            {
                user.Hash = GetUniqueToken();
                return user.Update(_userFactory);
            }
            else
            {
                return user;
            }
        }

        public UserInfo GetUserInSession(HttpContext httpContext)
        {
            if (httpContext.User.Claims.Count() > 0)
            {
                return JsonConvert.DeserializeObject<UserInfo>(httpContext.User.Claims.First().Value);
            }
            return null;
        }
        private string GetUniqueToken()
        {
            using (var crypto = new RNGCryptoServiceProvider())
            {
                int length = 100;
                string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_";
                byte[] data = new byte[length];

                // If chars.Length isn't a power of 2 then there is a bias if we simply use the modulus operator. The first characters of chars will be more probable than the last ones.
                // buffer used if we encounter an unusable random byte. We will regenerate it in this buffer
                byte[] buffer = null;

                // Maximum random number that can be used without introducing a bias
                int maxRandom = byte.MaxValue - ((byte.MaxValue + 1) % chars.Length);

                crypto.GetBytes(data);

                char[] result = new char[length];

                for (int i = 0; i < length; i++)
                {
                    byte value = data[i];

                    while (value > maxRandom)
                    {
                        if (buffer == null)
                        {
                            buffer = new byte[1];
                        }

                        crypto.GetBytes(buffer);
                        value = buffer[0];
                    }

                    result[i] = chars[value % chars.Length];
                }

                return new string(result);
            }
        }

        public IEnumerable<IUserModel> GetAllUserAddress()
        {
            return _userFactory.BuildUserModel().ListAllUsers(_userFactory);
        }

        public IEnumerable<IUserAddressModel> ListAddressByUser(long userId)
        {
            return _userAddressFactory.BuildUserAddressModel().ListByUser(userId, _userAddressFactory);
        }
        public IUserAddressModel GetAddressByChain(long userId, ChainEnum chain)
        {
            return _userAddressFactory.BuildUserAddressModel().GetByChain(userId, chain, _userAddressFactory);
        }
        public void AddOrChangeAddress(long userId, ChainEnum chain, string address)
        {
            var addr = _userAddressFactory.BuildUserAddressModel().GetByChain(userId, chain, _userAddressFactory);
            if (addr != null)
            {
                addr.Address = address;
                addr.Update();
            }
            else
            {
                addr = _userAddressFactory.BuildUserAddressModel();
                addr.UserId = userId;
                addr.Chain = chain;
                addr.Address = address;
                addr.Insert();
            }
        }
        public void RemoveAddress(long userId, ChainEnum chain) {
            var addr = _userAddressFactory.BuildUserAddressModel().GetByChain(userId, chain, _userAddressFactory);
            if (addr != null)
            {
                _userAddressFactory.BuildUserAddressModel().Remove(addr.Id);
            }
        }
    }
}
