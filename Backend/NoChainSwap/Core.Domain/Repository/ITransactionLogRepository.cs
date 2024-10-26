using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Repository
{
    public interface ITransactionLogRepository<TModel, TFactory>
    {
        TModel Insert(TModel model);
        IEnumerable<TModel> ListById(long logId, TFactory factory);
        IEnumerable<TModel> ListBySenderTx(string txId, TFactory factory);
        IEnumerable<TModel> ListByReceiverTx(string txId, TFactory factory);

    }
}
