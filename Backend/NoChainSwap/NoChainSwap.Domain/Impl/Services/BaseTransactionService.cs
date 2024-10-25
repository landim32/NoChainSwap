using NoChainSwap.Domain.Impl.Models;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Services;
using NoChainSwap.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Impl.Services
{
    public class BaseTransactionService
    {
        public string GetTransactionEnumToString(TransactionStatusEnum status)
        {
            string str = string.Empty;
            switch (status)
            {
                case TransactionStatusEnum.Initialized:
                    str = "Initialized";
                    break;
                case TransactionStatusEnum.Calculated:
                    str = "Calculated";
                    break;
                case TransactionStatusEnum.BtcNotConfirmed:
                    str = "Btc Not Confirmed";
                    break;
                case TransactionStatusEnum.StxNotConfirmed:
                    str = "Stx Not Confirmed";
                    break;
                case TransactionStatusEnum.BtcConfirmed:
                    str = "Btc Confirmed";
                    break;
                case TransactionStatusEnum.StxConfirmed:
                    str = "Stx Confirmed";
                    break;
                case TransactionStatusEnum.BtcConfirmedStxNotConfirmed:
                    str = "Btc Confirmed, Stx Not Confirmed";
                    break;
                case TransactionStatusEnum.StxConfirmedBtcNotConfirmed:
                    str = "Stx Confirmed, Btc Not Confirmed";
                    break;
                case TransactionStatusEnum.BtcConfirmedStxConfirmed:
                    str = "Btc Confirmed and Stx Confirmed";
                    break;
                case TransactionStatusEnum.StxConfirmedBtcConfirmed:
                    str = "Stx Confirmed and Btc Confirmed";
                    break;
                case TransactionStatusEnum.InvalidInformation:
                    str = "Invalid Information";
                    break;
                case TransactionStatusEnum.CriticalError:
                    str = "Critical Error";
                    break;
            }
            return str;
        }

        protected void AddLog(long txId, string msg, LogTypeEnum t, ITransactionLogDomainFactory txLogFactory)
        {
            var md = txLogFactory.BuildTransactionLogModel();
            md.TxId = txId;
            md.Date = DateTime.Now;
            md.LogType = t;
            md.Message = msg;
            md.Insert();
        }
    }
}
