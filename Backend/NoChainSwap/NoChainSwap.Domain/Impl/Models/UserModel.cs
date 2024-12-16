using System;
using System.Collections.Generic;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using Core.Domain;
using Core.Domain.Repository;
using System.Net;

namespace NoChainSwap.Domain.Impl.Models
{
    public class UserModel : IUserModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository<IUserModel, IUserDomainFactory> _repositoryUser;

        public UserModel(IUnitOfWork unitOfWork, IUserRepository<IUserModel, IUserDomainFactory> repositoryUser)
        {
            _unitOfWork = unitOfWork;
            _repositoryUser = repositoryUser;
        }

        public long Id { get; set; }
        public string Hash { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }

        private string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }

        public IUserModel GetById(long userId, IUserDomainFactory factory)
        {
            return _repositoryUser.GetById(userId, factory);
        }


        public IUserModel Save(IUserDomainFactory factory)
        {
            IUserModel ret = null;
            if (this.Id > 0)
            {
                ret = _repositoryUser.Update(this, factory);
            }
            else
            {
                ret = _repositoryUser.Insert(this, factory);
            }
            return ret;
        }

        public IUserModel Update(IUserDomainFactory factory)
        {
            return _repositoryUser.Update(this, factory);
        }

        public IEnumerable<IUserModel> ListUsers(IUserDomainFactory factory)
        {
            return _repositoryUser.ListUsers(factory);
        }

        public IUserModel GetByAddress(ChainEnum chain, string address, IUserDomainFactory factory)
        {
            return _repositoryUser.GetByAddress((int)chain, address, factory);
        }

        public IUserModel GetByEmail(string email, IUserDomainFactory factory)
        {
            return _repositoryUser.GetByEmail(email, factory);
        }

        public IUserModel GetByRecoveryHash(string recoveryHash, IUserDomainFactory factory)
        {
            return _repositoryUser.GetUserByRecoveryHash(recoveryHash, factory);
        }

        public IEnumerable<IUserModel> ListAllUsers(IUserDomainFactory factory)
        {
            return _repositoryUser.ListUsers(factory);
        }

        public IUserModel LoginWithEmail(string email, string password, IUserDomainFactory factory)
        {
            var user = _repositoryUser.GetByEmail(email, factory);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            string encryptPwd = CreateMD5(user.Hash + "|" + password);
            return _repositoryUser.LoginWithEmail(email, encryptPwd, factory);
        }

        public bool HasPassword(long userId, IUserDomainFactory factory)
        {
            return _repositoryUser.HasPassword(userId, factory);
        }

        public void ChangePassword(long userId, string password, IUserDomainFactory factory)
        {
            var user = _repositoryUser.GetById(userId, factory);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            string encryptPwd = CreateMD5(user.Hash + "|" + password);
            _repositoryUser.ChangePassword(userId, encryptPwd);
        }

        public void ChangePasswordUsingHash(string recoveryHash, string password, IUserDomainFactory factory)
        {
            var user = _repositoryUser.GetUserByRecoveryHash(recoveryHash, factory);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            string encryptPwd = CreateMD5(user.Hash + "|" + password);
            _repositoryUser.ChangePassword(user.Id, encryptPwd);
        }

        public string GenerateRecoveryHash(long userId, IUserDomainFactory factory)
        {
            var user = _repositoryUser.GetById(userId, factory);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            string recoveryHash = CreateMD5(user.Hash + "|" + Guid.NewGuid().ToString());
            _repositoryUser.UpdateRecoveryHash(userId, recoveryHash);
            return recoveryHash;
        }
    }
}
