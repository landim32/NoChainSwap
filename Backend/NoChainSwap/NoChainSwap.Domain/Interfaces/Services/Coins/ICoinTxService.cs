﻿using NoChainSwap.Domain.Impl.Models;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using NoChainSwap.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Interfaces.Services.Coins
{
    public interface ICoinTxService
    {
        string GetSlug();
        string GetPoolAddress();
        string GetAddressUrl(string address);
        string GetTransactionUrl(string txId);
        string ConvertToString(long coin);
        Task<bool> VerifyTransaction(ITransactionModel tx);
        void AddLog(long txId, string msg, LogTypeEnum t, ITransactionLogDomainFactory txLogFactory);
        Task<bool> ProcessTransaction(ITransactionModel tx, ICoinTxService receiverService);
    }
}
