import BusinessResult from "../../DTO/Business/BusinessResult";
import TxInfo from "../../DTO/Domain/TxInfo";
import TxLogInfo from "../../DTO/Domain/TxLogInfo";
import TxParamInfo from "../../DTO/Domain/TxParamInfo";
import { TransactionStatusEnum } from "../../DTO/Enum/TransactionStatusEnum";
import ITxService from "../../Services/Interfaces/ITxService";

export default interface ITxBusiness {
  init: (priceService: ITxService) => void;
  createTx: (param: TxParamInfo) => Promise<BusinessResult<string>>;
  getByHash: (hash: string) => Promise<BusinessResult<TxInfo>>;
  changeStatus: (txid: number, status: TransactionStatusEnum, message: string) => Promise<BusinessResult<boolean>>;
  listAllTx: () => Promise<BusinessResult<TxInfo[]>>;
  listMyTx: (address: string) => Promise<BusinessResult<TxInfo[]>>;
  listTxLogs: (txid: number) => Promise<BusinessResult<TxLogInfo[]>>;
  processTx: (txid: number) => Promise<BusinessResult<boolean>>;
  payback: (txid: number, receiverTxId: string, receiverFee: number) => Promise<BusinessResult<boolean>>;
}