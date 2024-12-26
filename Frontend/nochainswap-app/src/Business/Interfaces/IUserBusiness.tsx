import BusinessResult from "../../DTO/Business/BusinessResult";
import UserInfo from "../../DTO/Domain/UserInfo";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import IUserService from "../../Services/Interfaces/IUserService";

export default interface IUserBusiness {
  init: (userService: IUserService) => void;
  getMe: () => Promise<BusinessResult<UserInfo>>;
  getUserByAddress: (chain: ChainEnum, address: string) => Promise<BusinessResult<UserInfo>>;
  getUserByEmail: (email: string) => Promise<BusinessResult<UserInfo>>;
  getTokenUnauthorized: (chainId: ChainEnum, address: string) => Promise<BusinessResult<string>>;
  getTokenAuthorized: (email: string, password: string) => Promise<BusinessResult<string>>;
  insert: (user: UserInfo) => Promise<BusinessResult<UserInfo>>;
  update: (user: UserInfo) => Promise<BusinessResult<UserInfo>>;
  loginWithEmail: (email: string, password: string) => Promise<BusinessResult<UserInfo>>;
  hasPassword: () => Promise<BusinessResult<boolean>>;
  changePassword: (oldPassword: string, newPassword: string) => Promise<BusinessResult<boolean>>;
  sendRecoveryEmail: (email: string) => Promise<BusinessResult<boolean>>;
  changePasswordUsingHash: (recoveryHash: string, newPassword: string) => Promise<BusinessResult<boolean>>; 
}