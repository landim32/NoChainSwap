import StatusRequest from "../../DTO/Services/StatusRequest";
import UserAddressListResult from "../../DTO/Services/UserAddressListResult";
import UserAddressResult from "../../DTO/Services/UserAddressResult";
import IHttpClient from "../../Infra/Interface/IHttpClient"; 
import IUserAddressService from "../Interfaces/IUserAddressService";

let _httpClient : IHttpClient;

const UserAddressService : IUserAddressService = {
    init: function (htppClient: IHttpClient): void {
        _httpClient = htppClient;
    },
    listAddressByUser: async (userId: number) => {
        let ret: UserAddressListResult;
        let url = "/api/UserAddress/listaddressbyuser/" + userId;
        let request = await _httpClient.doGet<UserAddressListResult>(url, {});
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
    getAddressByChain: async (userId: number, chainId: number) => {
        let ret: UserAddressResult;
        let url = "/api/UserAddress/getaddressbychain/" + userId + "/" + chainId;
        let request = await _httpClient.doGet<UserAddressResult>(url, {});
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
    addOrChangeAddress: async (userId: number, chainId: number, address: string) => {
        let ret: StatusRequest;
        let url = "/api/UserAddress/addorchangeaddress";
        let request = await _httpClient.doPost<UserAddressResult>(url, {
            userId: userId,
            chainId: chainId,
            address: address
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
    removeAddress: async (userId: number, chainId: number) => {
        let ret: StatusRequest;
        let url = "/api/UserAddress/removeaddress/" + userId + "/" + chainId;
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
    }
}

export default UserAddressService;