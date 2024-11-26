using System;
using System.Collections.Generic;

namespace Core.Domain.Repository
{
    public interface IUserRepository<TModel, TFactory>
    {
        TModel Insert(TModel model, TFactory factory);
        TModel Update(TModel model, TFactory factory);
        IEnumerable<TModel> ListUsers(TFactory factory);
        TModel GetById(long userId, TFactory factory);
        TModel GetByEmail(string email, TFactory factory);
        TModel GetByAddress(int chainId, string address, TFactory factory);
    }
}
