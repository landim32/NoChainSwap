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
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }

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

        public IEnumerable<IUserModel> ListAllUsers(IUserDomainFactory factory)
        {
            return _repositoryUser.ListUsers(factory);
        }
    }
}
