using NoChainSwap.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Interfaces.Factory
{
    public interface IUserRecipientDomainFactory
    {
        IUserRecipientModel BuildUserRecipientModel();
    }
}
