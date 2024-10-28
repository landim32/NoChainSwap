import env from 'react-dotenv';
import { HttpClient } from '../Infra/Impl/HttpClient';
import IHttpClient from '../Infra/Interface/IHttpClient';
import { IAuthService } from './Interfaces/IAuthService';
import { AuthService } from './Impl/AuthService';
import { IPriceService } from './Interfaces/IPriceService';
import { PriceService } from './Impl/PriceService';
import { ITxService } from './Interfaces/ITxService';
import { TxService } from './Impl/TxService';

const httpClientAuth : IHttpClient = HttpClient();
httpClientAuth.init(env.API_BASE_URL);

const authServiceImpl : IAuthService = AuthService;
authServiceImpl.init(httpClientAuth);

const priceServiceImpl : IPriceService = PriceService;
priceServiceImpl.init(httpClientAuth);

const txServiceImpl : ITxService = TxService;
txServiceImpl.init(httpClientAuth);

const ServiceFactory = {
  AuthService: authServiceImpl,
  PriceService: priceServiceImpl,
  TxService: txServiceImpl,
  setLogoffCallback: (cb : () => void) => {
    httpClientAuth.setLogoff(cb);
  }
};

export default ServiceFactory;