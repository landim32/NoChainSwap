import TxInfo from "../../DTO/Domain/TxInfo";
import TxLogInfo from "../../DTO/Domain/TxLogInfo";
import TxParamInfo from "../../DTO/Domain/TxParamInfo";
import TxRevertInfo from "../../DTO/Domain/TxRevertInfo";
import StatusRequest from "../../DTO/Services/StatusRequest";
import { TxIdResult } from "../../DTO/Services/TxIdResult";
import { TxListResult } from "../../DTO/Services/TxListResult";
import { TxLogListResult } from "../../DTO/Services/TxLogListResult";
import TxPaybackParam from "../../DTO/Services/TxPaybackParam";
import { TxResult } from "../../DTO/Services/TxResult";
import IHttpClient from "../../Infra/Interface/IHttpClient";
import ITxService from "../Interfaces/ITxService";

let _httpClient : IHttpClient;

const TxService : ITxService = {
    init: function (htppClient: IHttpClient): void {
        _httpClient = htppClient;
    },
    createTx: async (param: TxParamInfo) => {
        let ret: TxIdResult;
        console.log("createTx: ", JSON.stringify(param));
        let request = await _httpClient.doPost<string>("api/Transaction/createTx", param);
        if (request.success) {
            ret = {
                mensagem: "Transaction created",
                hash: request.data,
                sucesso: true,
                ...ret
            };
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
    getByHash: async (hash: string) => {
        let ret: TxResult;
        let request = await _httpClient.doGet<TxInfo>("api/Transaction/gettransaction/" + hash, {});
        if (request.success) {
            return {
                sucesso: true,
                transaction: request.data,
                ...ret
            };
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
    changeStatus: async (param: TxRevertInfo) => {
        let ret: StatusRequest;
        let request = await _httpClient.doPost<number>("api/Transaction/changestatus", param);
        if (request.success) {
            ret = {
                mensagem: "Transaction status changed",
                sucesso: true,
                ...ret
            };
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
    listAllTx: async () => {
        let ret: TxListResult;
        let request = await _httpClient.doGet<TxInfo[]>("api/Transaction/listalltransactions", {});
        if (request.success) {
            return {
                sucesso: true,
                transactions: request.data,
                ...ret
            };
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
    listMyTx: async (address: string) => {
        let ret: TxListResult;
        let request = await _httpClient.doGet<TxInfo[]>("api/Transaction/listmytransactions/" + address, {});
        if (request.success) {
            return {
                sucesso: true,
                transactions: request.data,
                ...ret
            };
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
    listTxLogs: async (txid: number) => {
        let ret: TxLogListResult;
        let request = await _httpClient.doGet<TxLogInfo[]>("api/Transaction/listtransactionlog/" + txid, {});
        if (request.success) {
            return {
                sucesso: true,
                logs: request.data,
                ...ret
            };
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
    proccessTx:  async (txid: number) => {
        let ret: StatusRequest;
        let request = await _httpClient.doGet<boolean>("api/Transaction/processtransaction/" + txid, {});
        if (request.success) {
            return {
                mensagem: request.data ? "Transaction processed" : "Transaction not processed" ,
                sucesso: request.data,
                ...ret
            };
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
    payback: async (param: TxPaybackParam) => {
        let ret: StatusRequest;
        let request = await _httpClient.doPost<number>("api/Transaction/payback", param);
        if (request.success) {
            ret = {
                mensagem: "Payback proccess successfully",
                sucesso: true,
                ...ret
            };
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

export default TxService;