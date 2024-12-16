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
        WaitingSenderPayment = 3,
        SenderNotConfirmed = 4,
        SenderConfirmed = 5,
        SenderConfirmedReiceiverNotConfirmed = 6,
        Finished = 7,
        InvalidInformation = 8,
        CriticalError = 9,
        Canceled = 10
    }
}
