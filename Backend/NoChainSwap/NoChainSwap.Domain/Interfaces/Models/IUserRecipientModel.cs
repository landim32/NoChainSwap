using NoChainSwap.Domain.Impl.Factory;
using NoChainSwap.Domain.Impl.Models;
using NoChainSwap.Domain.Interfaces.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Interfaces.Models
{
    public interface IUserRecipientModel
    {
        long Id { get; set; }
        long UserId { get; set; }
        ChainEnum Chain { get; set; }
        DateTime CreateAt { get; set; }
        DateTime UpdateAt { get; set; }
        string Address {  get; set; }
        IUserAddressModel Save();
        IUserAddressModel Update();
        IEnumerable<IUserAddressModel> ListByUser(long userId, IUserDomainFactory factory);
        IUserAddressModel GetById(long id, UserDomainFactory factory);

    }
}
