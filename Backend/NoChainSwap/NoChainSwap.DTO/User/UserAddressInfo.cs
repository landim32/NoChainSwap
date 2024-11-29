using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.User
{
    public class UserAddressInfo
    {
        public long Id { get; set; }
        public int ChainId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string Address { get; set; }
    }
}
