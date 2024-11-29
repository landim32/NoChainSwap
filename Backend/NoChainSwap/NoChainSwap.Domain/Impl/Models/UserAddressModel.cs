using Core.Domain.Repository;
using Core.Domain;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Impl.Models
{
    public class UserAddressModel : IUserAddressModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory> _repositoryUserAddr;

        public UserAddressModel(IUnitOfWork unitOfWork, IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory> repositoryUserAddr)
        {
            _unitOfWork = unitOfWork;
            _repositoryUserAddr = repositoryUserAddr;
        }

        public long Id { get; set; }
        public long UserId { get; set; }
        public ChainEnum Chain { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string Address { get; set; }

        public IUserAddressModel GetByChain(long userId, ChainEnum chain, IUserAddressDomainFactory factory)
        {
            return _repositoryUserAddr.GetByChain(userId, (int)chain, factory);
        }

        public IUserAddressModel GetById(long id, IUserAddressDomainFactory factory)
        {
            return _repositoryUserAddr.GetById(id, factory);
        }

        public IEnumerable<IUserAddressModel> ListByUser(long userId, IUserAddressDomainFactory factory)
        {
            return _repositoryUserAddr.ListByUser(userId, factory);
        }

        public IUserAddressModel Insert()
        {
            var user = _repositoryUserAddr.Insert(this);
            return user;
        }

        public void Remove(long addressId)
        {
            _repositoryUserAddr.Delete(addressId);
        }

        public IUserAddressModel Update()
        {
            var user = _repositoryUserAddr.Update(this);
            return user;
        }
    }
}