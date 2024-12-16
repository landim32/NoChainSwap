import BusinessResult from "../../DTO/Business/BusinessResult";
import TxInfo from "../../DTO/Domain/TxInfo";
import TxLogInfo from "../../DTO/Domain/TxLogInfo";
import TxParamInfo from "../../DTO/Domain/TxParamInfo";
import TxRevertInfo from "../../DTO/Domain/TxRevertInfo";
import { CoinEnum } from "../../DTO/Enum/CoinEnum";
import { TransactionStatusEnum } from "../../DTO/Enum/TransactionStatusEnum";
import TxPaybackParam from "../../DTO/Services/TxPaybackParam";
import ITxService from "../../Services/Interfaces/ITxService";
import ITxBusiness from "../Interfaces/ITxBusiness";

let _txService: ITxService;

const CoinToStr = (coin: CoinEnum) => {
  let str: string = "";
  switch (coin) {
    case CoinEnum.Bitcoin:
      str = "btc";
      break;
    case CoinEnum.Stacks:
      str = "stx";
      break;
    case CoinEnum.USDT:
      str = "usdt";
      break;
    case CoinEnum.BRL:
      str = "brl";
      break;
  }
  return str;
};

const TxBusiness: ITxBusiness = {
  init: function (txService: ITxService): void {
    _txService = txService;
  },
  createTx: async (param: TxParamInfo) => {
    let ret: BusinessResult<string> = null;
    let retServ = await _txService.createTx(param);

    if (retServ.sucesso) {
      return {
        ...ret,
        dataResult: retServ.hash,
        sucesso: true
      };
    } else {
      return {
        ...ret,
        sucesso: false,
        mensagem: retServ.mensagem
      };
    }
  },
  getByHash: async (hash: string) => {
    let ret: BusinessResult<TxInfo> = null;
    let retServ = await _txService.getByHash(hash);
    if (retServ.sucesso) {
      return {
        ...ret,
        dataResult: retServ.transaction,
        sucesso: true
      };
    } else {
      return {
        ...ret,
        sucesso: false,
        mensagem: retServ.mensagem
      };
    }
  },
  changeStatus: async (txid: number, status: TransactionStatusEnum, message: string) => {
    let ret: BusinessResult<boolean> = null;
    let param: TxRevertInfo;
    let retServ = await _txService.changeStatus({
      ...param,
      txId: txid,
      status: status,
      message: message
    });

    if (retServ.sucesso) {
      return {
        ...ret,
        dataResult: true,
        sucesso: true
      };
    } else {
      return {
        ...ret,
        sucesso: false,
        mensagem: retServ.mensagem
      };
    }
  },
  listAllTx: async () => {
    try {
      let ret: BusinessResult<TxInfo[]> = null;
      let retServ = await _txService.listAllTx();
      console.log("ret: ", retServ);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.transactions,
          sucesso: true
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem
        };
      }
    } catch {
      throw new Error("Failed to list transactions");
    }
  },
  listMyTx: async (address: string) => {
    try {
      let ret: BusinessResult<TxInfo[]> = null;
      let retServ = await _txService.listMyTx(address);
      console.log("ret: ", retServ);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.transactions,
          sucesso: true
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem
        };
      }
    } catch {
      throw new Error("Failed to list transactions");
    }
  },
  listTxLogs: async (txid: number) => {
    try {
      let ret: BusinessResult<TxLogInfo[]> = null;
      let retServ = await _txService.listTxLogs(txid);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.logs,
          sucesso: true
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem
        };
      }
    } catch {
      throw new Error("Failed to list logs");
    }
  },
  processTx: async (txid: number) => {
    try {
      let ret: BusinessResult<boolean>;
      let retServ = await _txService.proccessTx(txid);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.sucesso,
          sucesso: true
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem
        };
      }
    } catch {
      throw new Error("Failed to process transaction");
    }
  },
  payback: async (txid: number, receiverTxId: string, receiverFee: number) => {
    let ret: BusinessResult<boolean>;
    let param: TxPaybackParam;
    let retServ = await _txService.payback({
      ...param,
      txId: txid,
      receiverTxId: receiverTxId,
      receiverFee: receiverFee
    });
    if (retServ.sucesso) {
      return {
        ...ret,
        dataResult: retServ.sucesso,
        sucesso: true
      };
    } else {
      return {
        ...ret,
        sucesso: false,
        mensagem: retServ.mensagem
      };
    }
  }
}

export default TxBusiness;