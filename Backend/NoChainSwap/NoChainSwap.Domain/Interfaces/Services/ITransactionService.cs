﻿using NoChainSwap.Domain.Interfaces.Models;
using NoChainSwap.Domain.Interfaces.Services.Coins;
using NoChainSwap.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Interfaces.Services
{
    public interface ITransactionService
    {
        ITransactionModel GetTx(long txId);
        Task<ITransactionModel> CreateTx(TransactionParamInfo param);
        ITransactionModel Update(TransactionInfo tx);
        IEnumerable<ITransactionModel> ListByStatusActive();
        IEnumerable<ITransactionModel> ListAll();
        IEnumerable<ITransactionModel> ListByAddress(string senderAddr);
        IEnumerable<ITransactionLogModel> ListLogById(long txid);
        Task<bool> ProcessTransaction(ITransactionModel tx);
        Task<bool> ProcessAllTransaction();
    }
}
