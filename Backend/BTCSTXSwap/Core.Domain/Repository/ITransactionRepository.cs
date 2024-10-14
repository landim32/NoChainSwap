﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Repository
{
    public interface ITransactionRepository<TModel, TFactory>
    {
        TModel SaveTx(TModel model);
        TModel GetByBtcAddr(string btcAddr, TFactory factory);
        TModel GetById(long txId, TFactory factory);
        TModel UpdateTx(TModel model);
        IEnumerable<TModel> ListTxByBtcAddr(TFactory factory);
    }
}
