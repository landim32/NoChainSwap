import IHttpClient from "../../Infra/Interface/IHttpClient";


export default interface IAuthService {
    init: (httpClient : IHttpClient) => void;
    //getAuthHash: (publicAdddress: string) => Promise<AuthResult>;
    //checkUserRegister: (chain: ChainEnum, adddress: string) => Promise<AuthResult>;
    //register: (publicAdddress: string, stxAdddress: string) => Promise<AuthResult>;
}