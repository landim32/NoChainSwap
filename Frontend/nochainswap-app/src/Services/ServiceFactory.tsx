import env from 'react-dotenv';
import { HttpClient } from '../Infra/Impl/HttpClient';
import IHttpClient from '../Infra/Interface/IHttpClient';
import IPriceService from './Interfaces/IPriceService';
import PriceService from './Impl/PriceService';
import ITxService from './Interfaces/ITxService';
import TxService from './Impl/TxService';
import IUserService from './Interfaces/IUserService';
import UserService from './Impl/UserService';
import IUserAddressService from './Interfaces/IUserAddressService';
import UserAddressService from './Impl/UserAddressService';

const httpClientAuth : IHttpClient = HttpClient();
httpClientAuth.init(env.API_BASE_URL);

const userServiceImpl : IUserService = UserService;
userServiceImpl.init(httpClientAuth);

const userAddrServiceImpl : IUserAddressService = UserAddressService;
userAddrServiceImpl.init(httpClientAuth);

const priceServiceImpl : IPriceService = PriceService;
priceServiceImpl.init(httpClientAuth);

const txServiceImpl : ITxService = TxService;
txServiceImpl.init(httpClientAuth);

const ServiceFactory = {
  UserService: userServiceImpl,
  UserAddressService: userAddrServiceImpl,
  PriceService: priceServiceImpl,
  TxService: txServiceImpl,
  setLogoffCallback: (cb : () => void) => {
    httpClientAuth.setLogoff(cb);
  }
};

export default ServiceFactory;