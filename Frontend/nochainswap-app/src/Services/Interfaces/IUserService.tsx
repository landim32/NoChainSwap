import UserInfo from "../../DTO/Domain/UserInfo";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import AuthResult from "../../DTO/Services/AuthResult";
import StatusRequest from "../../DTO/Services/StatusRequest";
import UserResult from "../../DTO/Services/UserResult";
import UserTokenResult from "../../DTO/Services/UserTokenResult";
import IHttpClient from "../../Infra/Interface/IHttpClient";


export default interface IUserService {
    init: (httpClient : IHttpClient) => void;
    getMe: (token: string) => Promise<UserResult>;
    getUserByAddress: (chain: ChainEnum, address: string) => Promise<UserResult>;
    getUserByEmail: (email: string) => Promise<UserResult>;
    getTokenUnauthorized: (chainId: number, address: string) => Promise<UserTokenResult>;
    getTokenAuthorized: (email: string, password: string) => Promise<UserTokenResult>;
    insert: (user: UserInfo) => Promise<UserResult>;
    update: (user: UserInfo, token: string) => Promise<UserResult>;
    loginWithEmail: (email: string, password: string) => Promise<UserResult>;
    hasPassword: (token: string) => Promise<StatusRequest>;
    changePassword: (oldPassword: string, newPassword: string, token: string) => Promise<StatusRequest>;
    sendRecoveryEmail: (email: string) => Promise<StatusRequest>;
    changePasswordUsingHash: (recoveryHash: string, newPassword: string) => Promise<StatusRequest>; 
}