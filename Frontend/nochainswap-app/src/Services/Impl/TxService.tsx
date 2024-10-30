import TxInfo from "../../DTO/Domain/TxInfo";
import TxLogInfo from "../../DTO/Domain/TxLogInfo";
import TxParamInfo from "../../DTO/Domain/TxParamInfo";
import StatusRequest from "../../DTO/Services/StatusRequest";
import { TxListResult } from "../../DTO/Services/TxListResult";
import { TxLogListResult } from "../../DTO/Services/TxLogListResult";
import { TxResult } from "../../DTO/Services/TxResult";
import IHttpClient from "../../Infra/Interface/IHttpClient";
import { ITxService } from "../Interfaces/ITxService";

let _httpClient : IHttpClient;

const TxService : ITxService = {
    init: function (htppClient: IHttpClient): void {
        _httpClient = htppClient;
    },
    createTx: async (param: TxParamInfo) => {
        let ret: StatusRequest;
        let request = await _httpClient.doPost<boolean>("api/Transaction/createTx", param);
        if (request.success) {
            return {
                mensagem: request.data ? "Transaction created" : "Transaction not created" ,
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
    getTx: async (txid: number) => {
        let ret: TxResult;
        let request = await _httpClient.doGet<TxInfo>("api/Transaction/gettransaction/" + txid, {});
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
        let ret: TxLogListResult;
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
    }
}

export { TxService }