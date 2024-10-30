import TxInfo from "../Domain/TxInfo";
import TxLogInfo from "../Domain/TxLogInfo";
import ProviderResult from "./ProviderResult";


interface ITxProvider {
    loadingTxInfo: boolean;
    loadingTxInfoList: boolean;
    loadingTxLogs: boolean;
    reloadingTx: boolean;
    txInfo?: TxInfo;
    txInfoList?: TxInfo[];
    txLogs?: TxLogInfo[];
    setTxInfo: (txInfo: TxInfo) => void;
    loadTx: (txid: number) => Promise<ProviderResult>;
    loadListAllTx: () => Promise<ProviderResult>;
    loadListMyTx: () => Promise<ProviderResult>;
    loadTxLogs: (txid: number) => Promise<ProviderResult>;
    reloadTx: (txid: number) => Promise<ProviderResult>;
}

export default ITxProvider;