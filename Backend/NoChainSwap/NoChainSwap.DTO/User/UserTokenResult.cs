using NoChainSwap.DTO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.User
{
    public class UserTokenResult: StatusResult
    {
        public string Token { get; set; }
    }
}
