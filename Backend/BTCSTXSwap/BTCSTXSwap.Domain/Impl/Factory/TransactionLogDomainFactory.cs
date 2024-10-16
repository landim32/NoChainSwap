﻿using BTCSTXSwap.Domain.Impl.Models;
using BTCSTXSwap.Domain.Interfaces.Factory;
using BTCSTXSwap.Domain.Interfaces.Models;
using Core.Domain.Repository;
using Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTCSTXSwap.Domain.Impl.Factory
{
    public class TransactionLogDomainFactory: ITransactionLogDomainFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionLogRepository<ITransactionLogModel, ITransactionLogDomainFactory> _repositoryTxLog;

        public TransactionLogDomainFactory(IUnitOfWork unitOfWork, ITransactionLogRepository<ITransactionLogModel, ITransactionLogDomainFactory> repositoryTxLog)
        {
            _unitOfWork = unitOfWork;
            _repositoryTxLog = repositoryTxLog;
        }

        public ITransactionLogModel BuildTransactionLogModel()
        {
            return new TransactionLogModel(_unitOfWork, _repositoryTxLog);
        }
    }
}