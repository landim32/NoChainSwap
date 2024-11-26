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

        public UserService(IUserDomainFactory userFactory)
        {
            _userFactory = userFactory;
        }

        public IUserModel CreateNewUser(UserInfo user)
        {
            try
            {
                var model = _userFactory.BuildUserModel();
                model.Name = user.Name;
                model.Email = user.Email;
                model.CreateAt = DateTime.Now;
                model.UpdateAt = DateTime.Now;
                model.Hash = GetUniqueToken();

                model.Save(_userFactory);

                return model;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public IUserModel UpdateUser(UserInfo user)
        {
            try
            {
                IUserModel model = null;
                if (user.Id > 0)
                {
                    model = _userFactory.BuildUserModel().GetById(user.Id, _userFactory);
                }
                else if (!string.IsNullOrEmpty(user.Email))
                {
                    model = _userFactory.BuildUserModel().GetByEmail(user.Email, _userFactory);
                }
                else if (user.ChainId > 0 && !string.IsNullOrEmpty(user.Address))
                {
                    model = _userFactory.BuildUserModel().GetByAddress((ChainEnum) user.ChainId, user.Address, _userFactory);
                }
                model.Name = user.Name;
                model.Email = user.Email;
                model.UpdateAt = DateTime.Now;
                model.Update(_userFactory);
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IUserModel GetUserByAddress(ChainEnum chain, string address)
        {
            try
            {
                return _userFactory.BuildUserModel().GetByAddress(chain, address, _userFactory);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IUserModel GetUSerByID(long userId)
        {
            try
            {
                return _userFactory.BuildUserModel().GetById(userId, _userFactory);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IUserModel GetUserHash(ChainEnum chain, string address)
        {
            try
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
            catch (Exception err)
            {
                throw err;
            }
        }

        public UserInfo GetUserInSession(HttpContext httpContext)
        {
            try
            {
                if (httpContext.User.Claims.Count() > 0)
                {
                    return JsonConvert.DeserializeObject<UserInfo>(httpContext.User.Claims.First().Value);
                }
                return null;
            }
            catch(Exception err)
            {
                return null;
            }
            
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
    }
}
