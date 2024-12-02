import ServiceFactory from '../../Services/ServiceFactory';
import AuthBusiness from '../Impl/AuthBusiness';
import IAuthBusiness from '../Interfaces/IAuthBusiness';

const authService = ServiceFactory.AuthService;

const authBusinessImpl: IAuthBusiness = AuthBusiness;
authBusinessImpl.init(authService);

const AuthFactory = {
  AuthBusiness: authBusinessImpl
};

export default AuthFactory;
