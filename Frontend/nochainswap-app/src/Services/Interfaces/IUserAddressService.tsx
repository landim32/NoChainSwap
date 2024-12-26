import StatusRequest from "../../DTO/Services/StatusRequest";
import UserAddressListResult from "../../DTO/Services/UserAddressListResult";
import UserAddressResult from "../../DTO/Services/UserAddressResult";
import IHttpClient from "../../Infra/Interface/IHttpClient";

export default interface IUserAddressService {
    init: (httpClient : IHttpClient) => void;
    listAddressByUser: (token: string) => Promise<UserAddressListResult>;
    getAddressByChain: (chainId: number, token: string) => Promise<UserAddressResult>;
    addOrChangeAddress: (userId: number, chainId: number, address: string) => Promise<StatusRequest>;
    removeAddress: (chainId: number, token: string) => Promise<StatusRequest>;
}