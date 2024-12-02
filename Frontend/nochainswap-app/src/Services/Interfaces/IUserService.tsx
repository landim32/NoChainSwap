import UserInfo from "../../DTO/Domain/UserInfo";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import AuthResult from "../../DTO/Services/AuthResult";
import UserResult from "../../DTO/Services/UserResult";
import IHttpClient from "../../Infra/Interface/IHttpClient";


export default interface IUserService {
    init: (httpClient : IHttpClient) => void;
    getUserById: (userId: number) => Promise<UserResult>;
    getUserByAddress: (chain: ChainEnum, address: string) => Promise<UserResult>;
    getUserByEmail: (email: string) => Promise<UserResult>;
    insert: (user: UserInfo) => Promise<UserResult>;
    update: (user: UserInfo) => Promise<UserResult>;
    loginWithEmail: (email: string, password: string) => Promise<UserResult>;
    changePassword: (userId: number, oldPassword: string, newPassword: string) => Promise<AuthResult>;
    sendRecoveryEmail: (email: string) => Promise<AuthResult>;
    changePasswordUsingHash: (recoveryHash: string, newPassword: string) => Promise<AuthResult>; 
}