import IHttpClient from "../../Infra/Interface/IHttpClient";
import IAuthService from "../Interfaces/IAuthService"; 

let _httpClient : IHttpClient;

const AuthService : IAuthService = {
    init: function (htppClient: IHttpClient): void {
        _httpClient = htppClient;
    },
    /*
    getAuthHash: async (publicAdddress: string) => {
        let ret: AuthResult;
        console.log(publicAdddress);
        let url = "/api/Auth/" + publicAdddress;
        let request = await _httpClient.doGet<AuthResult>(url, {});
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
    checkUserRegister: async (publicAdddress: string) => {
        let ret: AuthResult;
        console.log(publicAdddress);
        let request = await _httpClient.doGet<AuthResult>("/api/Auth/checkUserRegister/" + publicAdddress, {});
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
    register: async (publicAdddress: string, stxAddress: string) => {
        let ret: AuthResult;
        console.log(publicAdddress);
        let request = await _httpClient.doPost<AuthResult>("api/Auth", { 
            PublicAddress : publicAdddress, 
            StxAddress: stxAddress
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
    */
}

export default AuthService;