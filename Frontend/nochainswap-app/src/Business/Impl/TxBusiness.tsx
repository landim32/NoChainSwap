import BusinessResult from "../../DTO/Business/BusinessResult";
import TxInfo from "../../DTO/Domain/TxInfo";
import TxLogInfo from "../../DTO/Domain/TxLogInfo";
import TxParamInfo from "../../DTO/Domain/TxParamInfo";
import { ITxService } from "../../Services/Interfaces/ITxService";
import { ITxBusiness } from "../Interfaces/ITxBusiness";

let _txService: ITxService;

const TxBusiness: ITxBusiness = {
  init: function (txService: ITxService): void {
    _txService = txService;
  },
  createTx: async (param: TxParamInfo) => {
    try {
      let ret: BusinessResult<boolean> = null;
      let retServ = await _txService.createTx(param);
      
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
      throw new Error("Failed to create transaction");
    }
  },
  getTx: async (txid: number) => {
    try {
      let ret: BusinessResult<TxInfo> = null;
      let retServ = await _txService.getTx(txid);
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
    } catch {
      throw new Error("Failed to get transaction");
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
  }
}

export { TxBusiness };