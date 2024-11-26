using Core.Domain.Repository;
using DB.Infra.Context;
using NoChainSwap.Domain.Impl.Models;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infra.Repository
{
    public class UserRecipientRepository : IUserAddressRepository<IUserRecipientModel, IUserRecipientDomainFactory>
    {
        private NoChainSwapContext _ccsContext;

        public UserRecipientRepository(NoChainSwapContext ccsContext)
        {
            _ccsContext = ccsContext;
        }
        private IUserRecipientModel DbToModel(IUserRecipientDomainFactory factory, UserRecipient u)
        {
            var md = factory.BuildUserRecipientModel();
            md.Id = u.RecipientId;
            md.UserId = u.UserId;
            md.Chain = (ChainEnum) u.ChainId;
            md.Address = u.Address;
            md.CreateAt = u.CreatedAt;
            md.UpdateAt = u.UpdatedAt;
            return md;
        }

        private void ModelToDb(IUserRecipientModel u, UserRecipient md)
        {
            md.RecipientId = u.Id;
            md.UserId = u.UserId;
            md.ChainId = (int) u.Chain;
            md.Address = u.Address;
            md.CreatedAt = u.CreateAt;
            md.UpdatedAt = u.UpdateAt;
        }

        public IEnumerable<IUserRecipientModel> ListByUser(long userId, IUserRecipientDomainFactory factory)
        {
            var rows = _ccsContext.UserRecipients.Where(x => x.UserId == userId).ToList();
            return rows.Select(x => DbToModel(factory, x));
        }

        public IUserRecipientModel GetById(long addressId, IUserRecipientDomainFactory factory)
        {
            var row = _ccsContext.UserRecipients.Find(addressId);
            if (row == null)
                return null;
            return DbToModel(factory, row);
        }

        public IUserRecipientModel Insert(IUserRecipientModel model)
        {
            var u = new UserRecipient();
            ModelToDb(model, u);
            u.CreatedAt = DateTime.Now;
            u.UpdatedAt = DateTime.Now;
            _ccsContext.Add(u);
            _ccsContext.SaveChanges();
            model.Id = u.RecipientId;
            return model;
        }

        public IUserRecipientModel Update(IUserRecipientModel model)
        {
            var row = _ccsContext.UserRecipients.Find(model.Id);
            if (row == null)
            {
                throw new Exception("User address not found");
            }
            ModelToDb(model, row);
            row.UpdatedAt = DateTime.Now;
            _ccsContext.UserRecipients.Update(row);
            _ccsContext.SaveChanges();
            return model;
        }

        public void Delete(long id)
        {
            var row = _ccsContext.UserRecipients.Find(id);
            if (row == null)
            {
                throw new Exception("User address not found");
            }
            _ccsContext.UserRecipients.Remove(row);
            _ccsContext.SaveChanges();
        }
    }
}
