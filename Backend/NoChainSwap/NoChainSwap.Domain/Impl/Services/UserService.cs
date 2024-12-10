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
using NoChainSwap.DTO.MailerSend;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Impl.Services
{
    public class UserService : IUserService
    {
        private readonly IUserDomainFactory _userFactory;
        private readonly IUserAddressDomainFactory _userAddressFactory;
        private readonly IMailerSendService _mailerSendService;

        public UserService(IUserDomainFactory userFactory, IUserAddressDomainFactory userAddressFactory, IMailerSendService mailerSendService)
        {
            _userFactory = userFactory;
            _userAddressFactory = userAddressFactory;
            _mailerSendService = mailerSendService;
        }

        public IUserModel LoginWithEmail(string email, string password)
        {
            return _userFactory.BuildUserModel().LoginWithEmail(email, password, _userFactory);
        }

        public bool HasPassword(long userId)
        {
            return _userFactory.BuildUserModel().HasPassword(userId, _userFactory);
        }

        public void ChangePasswordUsingHash(string recoveryHash, string newPassword)
        {
            if (string.IsNullOrEmpty(recoveryHash))
            {
                throw new Exception("Recovery hash cant be empty");
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                throw new Exception("Password cant be empty");
            }
            var md = _userFactory.BuildUserModel();
            var user = md.GetByRecoveryHash(recoveryHash, _userFactory);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            md.ChangePassword(user.Id, newPassword, _userFactory);
        }

        public void ChangePassword(long userId, string oldPassword, string newPassword)
        {
            bool hasPassword = HasPassword(userId);
            if (hasPassword && string.IsNullOrEmpty(oldPassword))
            {
                throw new Exception("Old password cant be empty");
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                throw new Exception("New password cant be empty");
            }
            var md = _userFactory.BuildUserModel();
            var user = md.GetById(userId, _userFactory);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new Exception("To change password you need a email");
            }
            if (hasPassword)
            {
                var mdUser = md.LoginWithEmail(user.Email, oldPassword, _userFactory);
                if (mdUser == null)
                {
                    throw new Exception("Email or password is wrong");
                }
            }
            md.ChangePassword(user.Id, newPassword, _userFactory);
        }

        public async Task<bool> SendRecoveryEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new Exception("Email cant be empty");
            }
            var md = _userFactory.BuildUserModel();
            var user = md.GetByEmail(email, _userFactory);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var recoveryHash = md.GenerateRecoveryHash(user.Id, _userFactory);
            var recoveryUrl = $"https://nochainswap.org/recoverypassword/{recoveryHash}";

            var textMessage = 
                $"Hi {user.Name},\r\n\r\n" +
                "We received a request to reset your password. If you made this request, " + 
                "please click the link below to reset your password:\r\n\r\n" +
                recoveryUrl + "\r\n\r\n" +
                "If you didn’t request a password reset, please ignore this email or contact " + 
                "our support team if you have any concerns.\r\n\r\n" +
                "Best regards,\r\n" +
                "NoChainSwap Team";
            var htmlMessage =
                $"Hi <b>{user.Name}</b>,<br />\r\n<br />\r\n" +
                "We received a request to reset your password. If you made this request, " + 
                "please click the link below to reset your password:<br />\r\n<br />\r\n" +
                $"<a href=\"{recoveryUrl}\">{recoveryUrl}</a><br />\r\n<br />\r\n" +
                "If you didn’t request a password reset, please ignore this email or contact " + 
                "our support team if you have any concerns.<br />\r\n<br />\r\n" +
                "Best regards,<br />\r\n" +
                "<b>NoChainSwap Team</b>";

            var mail = new MailerInfo
            {
                From = new MailerRecipientInfo
                {
                    Email = "contact@nochainswap.org",
                    Name = "NoChainSwap Mailmaster"
                },
                To = new List<MailerRecipientInfo> {
                    new MailerRecipientInfo {
                        Email = user.Email,
                        Name = user.Name ?? user.Email
                    }
                },
                Subject = "[NoChainSwap] Password Recovery Email",
                Text = textMessage,
                Html = htmlMessage
            };
            await _mailerSendService.Sendmail(mail);
            return await Task.FromResult(true);
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
