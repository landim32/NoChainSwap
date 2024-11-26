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
    public class UserAddressRepository : IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory>
    {
        private NoChainSwapContext _ccsContext;

        public UserAddressRepository(NoChainSwapContext ccsContext)
        {
            _ccsContext = ccsContext;
        }
        private IUserAddressModel DbToModel(IUserAddressDomainFactory factory, UserAddress u)
        {
            var md = factory.BuildUserAddressModel();
            md.Id = u.AddressId;
            md.UserId = u.UserId;
            md.Chain = (ChainEnum) u.ChainId;
            md.Address = u.Address;
            md.CreateAt = u.CreateAt;
            md.UpdateAt = u.UpdateAt;
            return md;
        }

        private void ModelToDb(IUserAddressModel u, UserAddress md)
        {
            md.AddressId = u.Id;
            md.UserId = u.UserId;
            md.ChainId = (int) u.Chain;
            md.Address = u.Address;
            md.CreateAt = u.CreateAt;
            md.UpdateAt = u.UpdateAt;
        }

        public IEnumerable<IUserAddressModel> ListByUser(long userId, IUserAddressDomainFactory factory)
        {
            var rows = _ccsContext.UserAddresses.Where(x => x.UserId == userId).ToList();
            return rows.Select(x => DbToModel(factory, x));
        }

        public IUserAddressModel GetById(long addressId, IUserAddressDomainFactory factory)
        {
            var row = _ccsContext.UserAddresses.Find(addressId);
            if (row == null)
                return null;
            return DbToModel(factory, row);
        }

        public IUserAddressModel Insert(IUserAddressModel model)
        {
            var u = new UserAddress();
            ModelToDb(model, u);
            u.CreateAt = DateTime.Now;
            u.UpdateAt = DateTime.Now;
            _ccsContext.Add(u);
            _ccsContext.SaveChanges();
            model.Id = u.AddressId;
            return model;
        }

        public IUserAddressModel Update(IUserAddressModel model)
        {
            var row = _ccsContext.UserAddresses.Find(model.Id);
            if (row == null)
            {
                throw new Exception("User address not found");
            }
            ModelToDb(model, row);
            row.UpdateAt = DateTime.Now;
            _ccsContext.UserAddresses.Update(row);
            _ccsContext.SaveChanges();
            return model;
        }

        public void Delete(long id)
        {
            var row = _ccsContext.UserAddresses.Find(id);
            if (row == null)
            {
                throw new Exception("User address not found");
            }
            _ccsContext.UserAddresses.Remove(row);
            _ccsContext.SaveChanges();
        }
    }
}
