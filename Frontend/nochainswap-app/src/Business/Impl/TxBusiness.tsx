import BusinessResult from "../../DTO/Business/BusinessResult";
import AuthSession from "../../DTO/Domain/AuthSession";
import TxInfo from "../../DTO/Domain/TxInfo";
import TxLogInfo from "../../DTO/Domain/TxLogInfo";
import TxParamInfo from "../../DTO/Domain/TxParamInfo";
import TxRevertInfo from "../../DTO/Domain/TxRevertInfo";
import { CoinEnum } from "../../DTO/Enum/CoinEnum";
import { TransactionStatusEnum } from "../../DTO/Enum/TransactionStatusEnum";
import TxPaybackParam from "../../DTO/Services/TxPaybackParam";
import TxPaymentParam from "../../DTO/Services/TxPaymentParam";
import ITxService from "../../Services/Interfaces/ITxService";
import AuthFactory from "../Factory/AuthFactory";
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
    let session: AuthSession = AuthFactory.AuthBusiness.getSession();
    if (!session) {
      return {
        ...ret,
        sucesso: false,
        mensagem: "Not logged"
      };
    }
    let retServ = await _txService.changeStatus({
      ...param,
      txId: txid,
      status: status,
      message: message
    }, session.token);

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
      let session: AuthSession = AuthFactory.AuthBusiness.getSession();
      if (!session) {
        return {
          ...ret,
          sucesso: false,
          mensagem: "Not logged"
        };
      }
      let retServ = await _txService.listAllTx(session.token);
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
  listMyTx: async () => {
    try {
      let ret: BusinessResult<TxInfo[]> = null;
      let session: AuthSession = AuthFactory.AuthBusiness.getSession();
      if (!session) {
        return {
          ...ret,
          sucesso: false,
          mensagem: "Not logged"
        };
      }
      let retServ = await _txService.listMyTx(session.token);
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
      let session: AuthSession = AuthFactory.AuthBusiness.getSession();
      if (!session) {
        return {
          ...ret,
          sucesso: false,
          mensagem: "Not logged"
        };
      }
      let retServ = await _txService.proccessTx(txid, session.token);
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
    let session: AuthSession = AuthFactory.AuthBusiness.getSession();
    if (!session) {
      return {
        ...ret,
        sucesso: false,
        mensagem: "Not logged"
      };
    }
    let param: TxPaybackParam;
    let retServ = await _txService.payback({
      ...param,
      txId: txid,
      receiverTxId: receiverTxId,
      receiverFee: receiverFee
    }, session.token);
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
  },
  confirmSendPayment: async (txid: number, senderTxId: string) => {
    let ret: BusinessResult<boolean>;
    let param: TxPaymentParam;
    let retServ = await _txService.confirmSendPayment({
      ...param,
      txId: txid,
      senderTxId: senderTxId,
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
  },
  confirmPayment: async (txid: number) => {
    let ret: BusinessResult<boolean>;
    let session: AuthSession = AuthFactory.AuthBusiness.getSession();
    if (!session) {
      return {
        ...ret,
        sucesso: false,
        mensagem: "Not logged"
      };
    }
    let retServ = await _txService.confirmPayment(txid, session.token);
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