import UserInfo from "../../DTO/Domain/UserInfo";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import AuthResult from "../../DTO/Services/AuthResult";
import StatusRequest from "../../DTO/Services/StatusRequest";
import UserResult from "../../DTO/Services/UserResult";
import UserTokenResult from "../../DTO/Services/UserTokenResult";
import IHttpClient from "../../Infra/Interface/IHttpClient"; 
import IUserService from "../Interfaces/IUserService";

let _httpClient : IHttpClient;

const UserService : IUserService = {
    init: function (htppClient: IHttpClient): void {
        _httpClient = htppClient;
    },
    getMe: async (token: string) => {
        let ret: UserResult;
        let url = "/api/User/getme";
        let request = await _httpClient.doGetAuth<UserResult>(url, token);
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    getUserByAddress: async (chain: ChainEnum, address: string) => {
        let ret: UserResult;
        let url = "/api/User/getbyaddress/" + chain + "/" + address;
        console.log("url: ", url);
        let request = await _httpClient.doGet<UserResult>(url, {});
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    getUserByEmail: async (email: string) => {
        let ret: UserResult;
        let url = "/api/User/getbyemail/" + email;
        let request = await _httpClient.doGet<UserResult>(url, {});
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    getTokenUnauthorized: async (chainId: number, address: string) => {
        let ret: UserTokenResult;
        let url = "/api/User/gettokenunauthorized/" + chainId + "/" + address;
        let request = await _httpClient.doGet<UserTokenResult>(url, {});
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    getTokenAuthorized: async (email: string, password: string) => {
        let ret: UserTokenResult;
        let request = await _httpClient.doPost<UserTokenResult>("/api/User/gettokenauthorized", {
            email: email,
            password: password
        });
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    insert: async (user: UserInfo) => {
        let ret: UserResult;
        let request = await _httpClient.doPost<UserResult>("api/User/insert", user);
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    update: async (user: UserInfo, token: string) => {
        let ret: UserResult;
        let request = await _httpClient.doPostAuth<UserResult>("api/User/update", user, token);
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    loginWithEmail: async (email: string, password: string) => {
        let ret: UserResult;
        let request = await _httpClient.doPost<UserResult>("/api/User/loginwithemail", {
            email: email,
            password: password
        });
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    hasPassword: async (token: string) => {
        let ret: StatusRequest;
        let url = "/api/User/haspassword";
        let request = await _httpClient.doGetAuth<StatusRequest>(url, token);
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;        
    },
    changePassword: async (oldPassword: string, newPassword: string, token: string) => {
        let ret: StatusRequest;
        let request = await _httpClient.doPostAuth<StatusRequest>("/api/User/changepassword", {
            oldPassword: oldPassword,
            newPassword: newPassword
        }, token);
        console.log("request: ", request);
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    sendRecoveryEmail: async (email: string) => {
        let ret: StatusRequest;
        let url = "/api/User/sendrecoveryemail/" + email;
        let request = await _httpClient.doGet<StatusRequest>(url, {});
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    changePasswordUsingHash: async (recoveryHash: string, newPassword: string) => {
        let ret: StatusRequest;
        let request = await _httpClient.doPost<StatusRequest>("/api/User/changepasswordusinghash", {
            recoveryHash: recoveryHash,
            newPassword: newPassword
        });
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    }
}

export default UserService;