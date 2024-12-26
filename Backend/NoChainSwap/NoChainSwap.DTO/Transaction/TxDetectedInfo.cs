using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.Transaction
{
    public class TxDetectedInfo
    {
        public string RecipientAddress { get; set; }
        public string SenderAddress { get; set; }
        public string SenderTxId { get; set; }

    }
}
