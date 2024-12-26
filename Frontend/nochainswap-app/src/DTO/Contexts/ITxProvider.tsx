import TxInfo from "../Domain/TxInfo";
import TxLogInfo from "../Domain/TxLogInfo";
import { TransactionStatusEnum } from "../Enum/TransactionStatusEnum";
import ProviderResult from "./ProviderResult";


interface ITxProvider {
    loadingTxInfo: boolean;
    loadingTxInfoList: boolean;
    loadingTxLogs: boolean;
    reloadingTx: boolean;
    loadingUpdate: boolean;
    loadingPay: boolean;
    txInfo?: TxInfo;
    txInfoList?: TxInfo[];
    txLogs?: TxLogInfo[];
    getTitle: () => string;
    getStatus: (status: number) => string;
    changeStatus: (txId: number, status: TransactionStatusEnum, message: string) => Promise<ProviderResult>;
    /*
    getFormatedSenderAmount: () => string;
    getFormatedReceiverAmount: () => string;
    */
    setTxInfo: (txInfo: TxInfo) => void;
    loadTx: (hash: string) => Promise<ProviderResult>;
    loadListAllTx: () => Promise<ProviderResult>;
    loadListMyTx: () => Promise<ProviderResult>;
    loadTxLogs: (txid: number) => Promise<ProviderResult>;
    reloadTx: () => Promise<ProviderResult>;
    payTx: () => Promise<ProviderResult>;
    paybackTx: () => Promise<ProviderResult>;
    confirmTx: () => Promise<ProviderResult>;
}

export default ITxProvider;