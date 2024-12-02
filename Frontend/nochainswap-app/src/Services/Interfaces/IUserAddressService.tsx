import StatusRequest from "../../DTO/Services/StatusRequest";
import UserAddressListResult from "../../DTO/Services/UserAddressListResult";
import UserAddressResult from "../../DTO/Services/UserAddressResult";
import IHttpClient from "../../Infra/Interface/IHttpClient";

export default interface IUserAddressService {
    init: (httpClient : IHttpClient) => void;
    listAddressByUser: (userId: number) => Promise<UserAddressListResult>;
    getAddressByChain: (userId: number, chainId: number) => Promise<UserAddressResult>;
    addOrChangeAddress: (userId: number, chainId: number, address: string) => Promise<StatusRequest>;
    removeAddress: (userId: number, chainId: number) => Promise<StatusRequest>;
}