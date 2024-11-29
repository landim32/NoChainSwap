using System;
using NoChainSwap.Domain.Impl.Models;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using Core.Domain;
using Core.Domain.Repository;

namespace NoChainSwap.Domain.Impl.Factory
{
    public class UserAddressDomainFactory : IUserAddressDomainFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory> _repositoryUserAddr;

        public UserAddressDomainFactory(IUnitOfWork unitOfWork, IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory> repositoryUserAddr)
        {
            _unitOfWork = unitOfWork;
            _repositoryUserAddr = repositoryUserAddr;
        }

        public IUserAddressModel BuildUserAddressModel()
        {
            return new UserAddressModel(_unitOfWork, _repositoryUserAddr);
        }
    }
}
