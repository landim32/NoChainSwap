import React, { useContext, useState } from 'react';
import ProviderResult from '../../DTO/Contexts/ProviderResult';
import ITxProvider from '../../DTO/Contexts/ITxProvider';
import TxContext from './TxContext';
import TxInfo from '../../DTO/Domain/TxInfo';
import TxLogInfo from '../../DTO/Domain/TxLogInfo';
import TxFactory from '../../Business/Factory/TxFactory';
import AuthFactory from '../../Business/Factory/AuthFactory';
import { CoinEnum } from '../../DTO/Enum/CoinEnum';

export default function TxProvider(props: any) {

  const [loadingTxInfo, setLoadingTxInfo] = useState<boolean>(false);
  const [loadingTxInfoList, setLoadingTxInfoList] = useState<boolean>(false);
  const [loadingTxLogs, setLoadingTxLogs] = useState<boolean>(false);
  const [reloadingTx, setReloadingTx] = useState<boolean>(false);
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
    txInfo: txInfo,
    txInfoList: txInfoList,
    txLogs: txLogs,
    getTitle: () => {
      if (txInfo) {
        return CoinToText(txInfo.sendercoin) + " x " + CoinToText(txInfo.receivercoin);
      }
      return "";
    },
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
    setTxInfo: (txInfo: TxInfo) => {
      _setTxInfo(txInfo);
    },
    loadTx: async (txid: number) => {
      let ret: Promise<ProviderResult>;
      setLoadingTxInfo(true);
      try {
        let brt = await TxFactory.TxBusiness.getTx(txid);
        if (brt.sucesso) {
          setLoadingTxInfo(false);
          _setTxInfo(brt.dataResult);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: "Transaction load"
          };
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
      try {
        let session = await AuthFactory.AuthBusiness.getSession();
        if (!session) {
          let retErro = {
            ...ret,
            sucesso: false,
            mensagemErro: "Not logged"
          };
          return retErro;
        }
        let userSession = session;
        /*
        let brt = await TxFactory.TxBusiness.listMyTx(userSession.btcAddress);
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
        */
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
    reloadTx: async (txid: number) => {
      let ret: Promise<ProviderResult>;
      setReloadingTx(true);
      try {
        let brt = await TxFactory.TxBusiness.processTx(txid);
        let newTx = await TxFactory.TxBusiness.getTx(txid);
        if (newTx.sucesso) {
          _setTxInfo(newTx.dataResult);
          let logs = await TxFactory.TxBusiness.listTxLogs(txid);
          if (logs.sucesso) {
            setTxLogs(logs.dataResult);
          }
          else {
            throw new Error("Failed to reload transaction logs");
          }
        }
        else {
          throw new Error("Failed to reload transaction");
        }
        if (!brt.sucesso) {
          setReloadingTx(false);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: brt.mensagem
          };
        }
        else {
          setReloadingTx(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem
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
    }
  };

  return (
    <TxContext.Provider value={txProviderValue}>
      {props.children}
    </TxContext.Provider>
  );
}
