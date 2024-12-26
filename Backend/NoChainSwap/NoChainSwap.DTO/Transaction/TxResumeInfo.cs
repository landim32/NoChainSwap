using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.Transaction
{
    public class TxResumeInfo
    {
        public string TxId { get; set; }
        public string SenderAddress { get; set; }
        public string ReceiverAddress { get; set; }
        public long Amount { get; set; }
        public long Fee { get; set; }
        public bool Success { get; set; }
    }
}
