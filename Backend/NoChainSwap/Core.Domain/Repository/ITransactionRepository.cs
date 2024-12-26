using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Repository
{
    public interface ITransactionRepository<TModel, TFactory>
    {
        TModel SaveTx(TModel model);
        TModel GetBySenderAddr(string senderAddr, TFactory factory);
        TModel GetByRecipientAddr(string recipientAddr, TFactory factory);
        TModel GetById(long txId, TFactory factory);
        TModel GetByHash(string hash, TFactory factory);
        TModel UpdateTx(TModel model);
        IEnumerable<TModel> ListByAddress(string address, TFactory factory);
        TModel GetBySenderTxId(string txid, TFactory factory);
        TModel GetByReceiverTxId(string txid, TFactory factory);
        IEnumerable<TModel> ListByStatus(IList<int> status, TFactory factory);
        IEnumerable<TModel> ListAll(TFactory factory);
        IEnumerable<TModel> ListByUser(long userId, TFactory factory);
        IEnumerable<TModel> ListToDetect(TFactory factory);
    }
}
