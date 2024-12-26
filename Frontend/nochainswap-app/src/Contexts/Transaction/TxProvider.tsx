import React, { useContext, useState } from 'react';
import ProviderResult from '../../DTO/Contexts/ProviderResult';
import ITxProvider from '../../DTO/Contexts/ITxProvider';
import TxContext from './TxContext';
import TxInfo from '../../DTO/Domain/TxInfo';
import TxLogInfo from '../../DTO/Domain/TxLogInfo';
import TxFactory from '../../Business/Factory/TxFactory';
import AuthFactory from '../../Business/Factory/AuthFactory';
import { CoinEnum } from '../../DTO/Enum/CoinEnum';
import { TransactionStatusEnum } from '../../DTO/Enum/TransactionStatusEnum';
import ContractFactory from '../../Contracts/ContractFactory';
import EtherBusiness from '../../Business/Impl/EtherBusiness';
import EtherFactory from '../../Business/Factory/EtherFactory';

export default function TxProvider(props: any) {

  const [loadingTxInfo, setLoadingTxInfo] = useState<boolean>(false);
  const [loadingTxInfoList, setLoadingTxInfoList] = useState<boolean>(false);
  const [loadingTxLogs, setLoadingTxLogs] = useState<boolean>(false);
  const [reloadingTx, setReloadingTx] = useState<boolean>(false);
  const [loadingUpdate, setLoadingUpdate] = useState<boolean>(false);
  const [loadingPay, setLoadingPay] = useState<boolean>(false);
  const [txInfo, _setTxInfo] = useState<TxInfo>(null);
  const [txInfoList, setTxInfoList] = useState<TxInfo[]>(null);
  const [txLogs, setTxLogs] = useState<TxLogInfo[]>(null);

  const CoinToText = (coin: string) => {
    let str: string = "";
    switch (coin) {
      case "btc":
        str = "Bitcoin";
        break;
      case "stx":
        str = "Stacks";
        break;
      case "usdt":
        str = "USDT";
        break;
      case "brl":
        str = "Real (PIX)";
        break;
    }
    return str;
  };

  const StrToCoin = (str: string) => {
    let coin: CoinEnum;
    switch (str) {
      case "btc":
        coin = CoinEnum.Bitcoin;
        break;
      case "stx":
        coin = CoinEnum.Stacks;
        break;
      case "usdt":
        coin = CoinEnum.USDT;
        break;
      case "brl":
        coin = CoinEnum.BRL;
        break;
    }
    return coin;
  };

  const getFormatedAmount = (coin: CoinEnum, amount: number) => {
    if (amount) {
      let retorno: string;
      switch (coin) {
        case CoinEnum.Bitcoin:
          retorno = (amount / 100000000).toFixed(5).toString() + " BTC";
          break;
        case CoinEnum.Stacks:
          retorno = (amount / 100000000).toFixed(5).toString() + " STX";
          break;
        case CoinEnum.USDT:
          retorno = (amount / 100000000).toFixed(5).toString() + " USDT";
          break;
        case CoinEnum.BRL:
          retorno = (amount / 100000000).toFixed(2).toString();
          break;
      }
      return retorno;
    }
    return "~";
  };

  const txProviderValue: ITxProvider = {
    loadingTxInfo: loadingTxInfo,
    loadingTxInfoList: loadingTxInfoList,
    loadingTxLogs: loadingTxLogs,
    reloadingTx: reloadingTx,
    loadingUpdate: loadingUpdate,
    loadingPay: loadingPay,
    txInfo: txInfo,
    txInfoList: txInfoList,
    txLogs: txLogs,
    getTitle: () => {
      if (txInfo) {
        return CoinToText(txInfo.sendercoin) + " x " + CoinToText(txInfo.receivercoin);
      }
      return "";
    },
    getStatus: (status: number) => {
      let str: string;
      switch (status) {
        case TransactionStatusEnum.Initialized:
          str = "Initialized";
          break;
        case TransactionStatusEnum.Calculated:
          str = "Calculated";
          break;
        case TransactionStatusEnum.WaitingSenderPayment:
          str = "Waiting Payment";
          break;
        case TransactionStatusEnum.DetectedSenderPayment:
          str = "Detect Payment";
          break;
        case TransactionStatusEnum.SenderNotConfirmed:
          str = "Sender Not Confirmed";
          break;
        case TransactionStatusEnum.SenderConfirmed:
          str = "Sender Confirmed";
          break;
        case TransactionStatusEnum.SenderConfirmedReiceiverPaymentWaiting:
          str = "Sender Confirmed, Waiting Payment to Receiver";
          break;
        case TransactionStatusEnum.SenderConfirmedReiceiverNotConfirmed:
          str = "Receiver not Confirmed";
          break;
        case TransactionStatusEnum.Finished:
          str = "Finished";
          break;
        case TransactionStatusEnum.InvalidInformation:
          str = "Invalid Information";
          break;
        case TransactionStatusEnum.CriticalError:
          str = "Critical Error";
          break;
        case TransactionStatusEnum.Canceled:
          str = "Canceled";
          break;
      }
      return str;
    },
    changeStatus: async (txId: number, status: TransactionStatusEnum, message: string) => {
      let ret: Promise<ProviderResult>;
      setLoadingUpdate(true);
      try {
        let brt = await TxFactory.TxBusiness.changeStatus(txId, status, message);
        if (brt.sucesso) {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: "Transaction status changed"
          };
        }
        else {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem
          };
        }
      }
      catch (err) {
        setLoadingUpdate(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err)
        };
      }
    },
    /*
    getFormatedSenderAmount: () => {
      if (txInfo) {
        console.log("coin: ", StrToCoin(txInfo.sendercoin), txInfo.senderamount);
        return getFormatedAmount(StrToCoin(txInfo.sendercoin), txInfo.senderamount);
      }
      return "";
    },
    getFormatedReceiverAmount: () => {
      if (txInfo) {
        return getFormatedAmount(StrToCoin(txInfo.receivercoin), txInfo.receiveramount);
      }
      return "";
    },
    */
    setTxInfo: (txInfo: TxInfo) => {
      _setTxInfo(txInfo);
    },
    loadTx: async (hash: string) => {
      let ret: Promise<ProviderResult>;
      setLoadingTxInfo(true);
      try {
        let brt = await TxFactory.TxBusiness.getByHash(hash);
        if (brt.sucesso) {
          setLoadingTxInfo(false);
          _setTxInfo(brt.dataResult);
          let retLog = await txProviderValue.loadTxLogs(brt.dataResult.txid);
          if (retLog.sucesso) {
            return {
              ...ret,
              sucesso: true,
              mensagemSucesso: "Transaction load"
            };
          }
          else {
            return {
              ...ret,
              sucesso: false,
              mensagemSucesso: retLog.mensagemErro
            };
          }
        }
        else {
          setLoadingTxInfo(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem
          };
        }
      }
      catch (err) {
        setLoadingTxInfo(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err)
        };
      }
    },
    loadListMyTx: async () => {
      let ret: Promise<ProviderResult>;
      setLoadingTxInfoList(true);
      setTxInfoList([]);
      try {
        let retTx = await TxFactory.TxBusiness.listMyTx();
        if (retTx.sucesso) {
          setLoadingTxInfoList(false);
          setTxInfoList(retTx.dataResult);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: "Transactions load"
          };
        }
        else {
          setLoadingTxInfoList(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: retTx.mensagem
          };
        }
      }
      catch (err) {
        setLoadingTxInfoList(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err)
        };
      }
    },
    loadListAllTx: async () => {
      let ret: Promise<ProviderResult>;
      setLoadingTxInfoList(true);
      setTxInfoList([]);
      try {
        let brt = await TxFactory.TxBusiness.listAllTx();
        if (brt.sucesso) {
          setLoadingTxInfoList(false);
          setTxInfoList(brt.dataResult);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: "Transactions load"
          };
        }
        else {
          setLoadingTxInfoList(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem
          };
        }
      }
      catch (err) {
        setLoadingTxInfoList(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err)
        };
      }
    },
    loadTxLogs: async (txid: number) => {
      let ret: Promise<ProviderResult>;
      setLoadingTxLogs(true);
      try {
        let brt = await TxFactory.TxBusiness.listTxLogs(txid);
        if (brt.sucesso) {
          setLoadingTxLogs(false);
          setTxLogs(brt.dataResult);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: "Transaction logs load"
          };
        }
        else {
          setLoadingTxLogs(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem
          };
        }
      }
      catch (err) {
        setLoadingTxLogs(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err)
        };
      }
    },
    reloadTx: async () => {
      let ret: Promise<ProviderResult>;
      if (!txInfo) {
        return {
          ...ret,
          sucesso: false,
          mensagemErro: "Transaction is not load"
        };
      }
      setReloadingTx(true);
      try {
        let retPx = await TxFactory.TxBusiness.processTx(txInfo.txid);
        if (!retPx.sucesso) {
          setReloadingTx(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: retPx.mensagem
          };
        }
        let retTx = await TxFactory.TxBusiness.getByHash(txInfo.hash);
        if (retTx.sucesso) {
          _setTxInfo(retTx.dataResult);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: "Transaction successfully realoaded"
          };
        }
        else {
          setReloadingTx(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: "Cant reload transaction"
          };
        }
      }
      catch (err) {
        setReloadingTx(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err)
        };
      }
    },
    payTx: async () => {
      let ret: Promise<ProviderResult>;
      setLoadingPay(true);
      if (!txInfo) {
        setLoadingPay(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: "Transaction is not loaded"
        };
      }
      if (txInfo.status != TransactionStatusEnum.WaitingSenderPayment) {
        setLoadingPay(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: "Transaction invalid for payment"
        };
      }
      if (StrToCoin(txInfo.sendercoin) == CoinEnum.USDT) {
        let retTx = await EtherFactory.EtherBusiness.transferUSDT(txInfo.recipientaddress, txInfo.senderamountvalue);
        if (retTx.sucesso) {
          let retPB = await TxFactory.TxBusiness.confirmSendPayment(txInfo.txid, retTx.dataResult.transactionHash);
          if (retPB.sucesso) {
            setLoadingPay(false);
            return {
              ...ret,
              sucesso: true,
              mensagemSucesso: "Transaction successful"
            }
          }
          else {
            setLoadingPay(false);
            return {
              ...ret,
              sucesso: false,
              mensagemErro: retPB.mensagem
            }
          }
        }
        else {
          setLoadingPay(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: retTx.mensagem
          }
        }
      }
      else {
        setLoadingPay(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: "Coin not suported"
        }
      }
    },
    paybackTx: async () => {
      let ret: Promise<ProviderResult>;
      setLoadingPay(true);
      if (!txInfo) {
        setLoadingPay(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: "Transaction is not loaded"
        };
      }
      if (
        txInfo.status != TransactionStatusEnum.WaitingSenderPayment &&
        txInfo.status != TransactionStatusEnum.SenderConfirmed
      ) {
        setLoadingPay(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: "Transaction invalid for payback"
        };
      }
      if (StrToCoin(txInfo.receivercoin) == CoinEnum.USDT) {
        let retTx = await EtherFactory.EtherBusiness.transferUSDT(txInfo.receiveraddress, txInfo.receiverpayback);
        if (retTx.sucesso) {
          let retPB = await TxFactory.TxBusiness.payback(txInfo.txid, retTx.dataResult.transactionHash, parseInt(retTx.dataResult.cumulativeGasUsed));
          if (retPB.sucesso) {
            setLoadingPay(false);
            return {
              ...ret,
              sucesso: true,
              mensagemSucesso: "Transaction successful"
            }
          }
          else {
            setLoadingPay(false);
            return {
              ...ret,
              sucesso: false,
              mensagemErro: retPB.mensagem
            }
          }
        }
        else {
          setLoadingPay(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: retTx.mensagem
          }
        }
      }
      else {
        setLoadingPay(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: "Coin not suported"
        }
      }
    },
    confirmTx: async () => {
      let ret: Promise<ProviderResult>;
      setLoadingPay(true);
      if (!txInfo) {
        setLoadingPay(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: "Transaction is not loaded"
        };
      }
      console.log("txInfo: ", JSON.stringify(txInfo));
      if (
        txInfo?.status != TransactionStatusEnum.WaitingSenderPayment &&
        txInfo?.status != TransactionStatusEnum.SenderNotConfirmed &&
        txInfo?.status != TransactionStatusEnum.SenderConfirmed &&
        txInfo?.status != TransactionStatusEnum.SenderConfirmedReiceiverNotConfirmed &&
        txInfo?.status != TransactionStatusEnum.SenderConfirmedReiceiverPaymentWaiting
      ) {
        setLoadingPay(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: "Transaction invalid for payback"
        };
      }
      let retPB = await TxFactory.TxBusiness.confirmPayment(txInfo.txid);
      if (retPB.sucesso) {
        setLoadingPay(false);
        return {
          ...ret,
          sucesso: true,
          mensagemSucesso: "Transaction completed successfully"
        }
      }
      else {
        setLoadingPay(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: retPB.mensagem
        }
      }
    }
  };

  return (
    <TxContext.Provider value={txProviderValue} >
      {props.children}
    </TxContext.Provider >
  );
}
