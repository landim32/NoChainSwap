using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.Transaction
{
    public enum TransactionStatusEnum
    {
        Initialized = 1,
        Calculated = 2,
        //BtcNotConfirmed = 3,
        //StxNotConfirmed = 4,
        SenderNotConfirmed = 3,
        //BtcConfirmed = 5,
        //StxConfirmed = 6,
        SenderConfirmed = 5,
        //BtcConfirmedStxNotConfirmed = 7,
        //StxConfirmedBtcNotConfirmed = 8,
        SenderConfirmedReiceiverNotConfirmed = 7,
        //BtcConfirmedStxConfirmed = 9,
        //StxConfirmedBtcConfirmed = 10,
        Finished = 9,
        InvalidInformation = 11,
        CriticalError = 12
    }
}
