using NoChainSwap.Domain.Impl.Models;
using NoChainSwap.Domain.Interfaces.Factory;
using System;
using System.Collections.Generic;

namespace NoChainSwap.Domain.Interfaces.Models
{
    public interface IUserModel
    {
        long Id { get; set; }
        string Hash { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        bool IsAdmin { get; set; }
        DateTime CreateAt { get; set; }
        DateTime UpdateAt { get; set; }

        IUserModel Save(IUserDomainFactory factory);
        IUserModel Update(IUserDomainFactory factory);
        IUserModel GetByAddress(ChainEnum chain, string address, IUserDomainFactory factory);
        IUserModel GetByEmail(string email, IUserDomainFactory factory);
        IUserModel GetById(long userId, IUserDomainFactory factory);
        IUserModel GetByRecoveryHash(string recoveryHash, IUserDomainFactory factory);
        IEnumerable<IUserModel> ListAllUsers(IUserDomainFactory factory);
        IUserModel LoginWithEmail(string email, string password, IUserDomainFactory factory);
        bool HasPassword(long userId, IUserDomainFactory factory);
        void ChangePassword(long userId, string password, IUserDomainFactory factory);
        string GenerateRecoveryHash(long userId, IUserDomainFactory factory);
    }
}
