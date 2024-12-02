import UserInfo from "../../DTO/Domain/UserInfo";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import AuthResult from "../../DTO/Services/AuthResult";
import UserResult from "../../DTO/Services/UserResult";
import IHttpClient from "../../Infra/Interface/IHttpClient"; 
import IUserService from "../Interfaces/IUserService";

let _httpClient : IHttpClient;

const UserService : IUserService = {
    init: function (htppClient: IHttpClient): void {
        _httpClient = htppClient;
    },
    getUserById: async (userId: number) => {
        let ret: UserResult;
        let url = "/api/User/getbyid/" + userId;
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
    getUserByAddress: async (chain: ChainEnum, address: string) => {
        let ret: UserResult;
        let url = "/api/User/" + chain + "/" + address;
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
        let url = "/api/User/getByEmail/" + email;
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
    insert: async (user: UserInfo) => {
        let ret: UserResult;
        let request = await _httpClient.doPost<AuthResult>("api/User/insert", user);
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
    update: async (user: UserInfo) => {
        let ret: UserResult;
        let request = await _httpClient.doPost<AuthResult>("api/User/update", user);
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
    changePassword: async (userId: number, oldPassword: string, newPassword: string) => {
        let ret: UserResult;
        let request = await _httpClient.doPost<UserResult>("/api/User/changepassword", {
            userId: userId,
            oldPassword: oldPassword,
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
    },
    sendRecoveryEmail: async (email: string) => {
        let ret: UserResult;
        let request = await _httpClient.doPost<UserResult>("/api/User/sendrecoveryemail", {
            email: email
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
    changePasswordUsingHash: async (recoveryHash: string, newPassword: string) => {
        let ret: UserResult;
        let request = await _httpClient.doPost<UserResult>("/api/User/changepasswordusinghash", {
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