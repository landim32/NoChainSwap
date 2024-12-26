import TxParamInfo from "../../DTO/Domain/TxParamInfo";
import TxRevertInfo from "../../DTO/Domain/TxRevertInfo";
import StatusRequest from "../../DTO/Services/StatusRequest";
import { TxIdResult } from "../../DTO/Services/TxIdResult";
import { TxListResult } from "../../DTO/Services/TxListResult";
import { TxLogListResult } from "../../DTO/Services/TxLogListResult";
import TxPaybackParam from "../../DTO/Services/TxPaybackParam";
import TxPaymentParam from "../../DTO/Services/TxPaymentParam";
import { TxResult } from "../../DTO/Services/TxResult";
import IHttpClient from "../../Infra/Interface/IHttpClient";


export default interface ITxService {
    init: (httpClient : IHttpClient) => void;
    createTx: (param: TxParamInfo) => Promise<TxIdResult>;
    getByHash: (hash: string) => Promise<TxResult>;
    changeStatus: (param: TxRevertInfo, token: string) => Promise<StatusRequest>;
    listAllTx: (token: string) => Promise<TxListResult>;
    listMyTx: (token: string) => Promise<TxListResult>;
    listTxLogs: (txid: number) => Promise<TxLogListResult>;
    proccessTx: (txid: number, token: string) => Promise<StatusRequest>;
    payback: (param: TxPaybackParam, token: string) => Promise<StatusRequest>;
    confirmSendPayment: (param: TxPaymentParam) => Promise<StatusRequest>;
    confirmPayment: (txid: number, token: string) => Promise<StatusRequest>;
}