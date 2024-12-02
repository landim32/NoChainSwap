import ServiceFactory from '../../Services/ServiceFactory';
import UserAddressBusiness from '../Impl/UserAddressBusiness';
import IUserAddressBusiness from '../Interfaces/IUserAddressBusiness';

const userAddrService = ServiceFactory.UserAddressService;

const userAddrBusinessImpl: IUserAddressBusiness = UserAddressBusiness;
userAddrBusinessImpl.init(userAddrService);

const UserAddressFactory = {
  UserAddressBusiness: userAddrBusinessImpl
};

export default UserAddressFactory;
