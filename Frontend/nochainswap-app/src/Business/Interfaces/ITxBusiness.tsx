import BusinessResult from "../../DTO/Business/BusinessResult";
import TxInfo from "../../DTO/Domain/TxInfo";
import TxLogInfo from "../../DTO/Domain/TxLogInfo";
import TxParamInfo from "../../DTO/Domain/TxParamInfo";
import ITxService from "../../Services/Interfaces/ITxService";

export default interface ITxBusiness {
  init: (priceService: ITxService) => void;
  createTx: (param: TxParamInfo) => Promise<BusinessResult<number>>;
  getTx: (txid: number) => Promise<BusinessResult<TxInfo>>;
  listAllTx: () => Promise<BusinessResult<TxInfo[]>>;
  listMyTx: (address: string) => Promise<BusinessResult<TxInfo[]>>;
  listTxLogs: (txid: number) => Promise<BusinessResult<TxLogInfo[]>>;
  processTx: (txid: number) => Promise<BusinessResult<boolean>>;
}