import BusinessResult from "../../DTO/Business/BusinessResult";
import UserInfo from "../../DTO/Domain/UserInfo";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import IUserService from "../../Services/Interfaces/IUserService";

export default interface IUserBusiness {
  init: (userService: IUserService) => void;
  getUserById: (userId: number) => Promise<BusinessResult<UserInfo>>;
  getUserByAddress: (chain: ChainEnum, address: string) => Promise<BusinessResult<UserInfo>>;
  getUserByEmail: (email: string) => Promise<BusinessResult<UserInfo>>;
  insert: (user: UserInfo) => Promise<BusinessResult<UserInfo>>;
  update: (user: UserInfo) => Promise<BusinessResult<UserInfo>>;
  loginWithEmail: (email: string, password: string) => Promise<BusinessResult<UserInfo>>;
  hasPassword: (userId: number) => Promise<BusinessResult<boolean>>;
  changePassword: (userId: number, oldPassword: string, newPassword: string) => Promise<BusinessResult<boolean>>;
  sendRecoveryEmail: (email: string) => Promise<BusinessResult<boolean>>;
  changePasswordUsingHash: (recoveryHash: string, newPassword: string) => Promise<BusinessResult<boolean>>; 
}