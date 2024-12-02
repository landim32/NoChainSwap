import BusinessResult from "../../DTO/Business/BusinessResult";
import { AuthSession } from "../../DTO/Domain/AuthSession";
import UserInfo from "../../DTO/Domain/UserInfo";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import IAuthService from "../../Services/Interfaces/IAuthService";

export default interface IAuthBusiness {
  init: (authService: IAuthService) => void;
  //bindMetaMaskWallet: (name: string, email: string, fromReferralCode: string) => Promise<BusinessResult<AuthSession>>;
  //checkUserRegister: () => Promise<BusinessResult<boolean>>;
  //logIn: (authSession: AuthSession) => void;
  logIn: (callback?: any) => void;
  logOut: () => void;
  getSession: () => Promise<BusinessResult<AuthSession>>;
  //getGokenSession: () => string;
  //checkNetwork: () => Promise<BusinessResult<boolean>>;
}