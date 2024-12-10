import UserInfo from "../../DTO/Domain/UserInfo";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import AuthResult from "../../DTO/Services/AuthResult";
import StatusRequest from "../../DTO/Services/StatusRequest";
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
    hasPassword: (userId: number) => Promise<StatusRequest>;
    changePassword: (userId: number, oldPassword: string, newPassword: string) => Promise<StatusRequest>;
    sendRecoveryEmail: (email: string) => Promise<StatusRequest>;
    changePasswordUsingHash: (recoveryHash: string, newPassword: string) => Promise<StatusRequest>; 
}