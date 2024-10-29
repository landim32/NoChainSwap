import React, { useContext, useState } from 'react';
import ProviderResult from '../../DTO/Contexts/ProviderResult';
import ITxProvider from '../../DTO/Contexts/ITxProvider';
import TxContext from './TxContext';
import TxInfo from '../../DTO/Domain/TxInfo';
import TxLogInfo from '../../DTO/Domain/TxLogInfo';
import TxFactory from '../../Business/Factory/TxFactory';

export default function TxProvider(props: any) {

  const [loadingTxInfo, setLoadingTxInfo] = useState<boolean>(false);
  const [loadingAllTxInfo, setLoadingAllTxInfo] = useState<boolean>(false);
  const [loadingTxLogs, setLoadingTxLogs] = useState<boolean>(false);
  const [reloadingTx, setReloadingTx] = useState<boolean>(false);
  const [txInfo, _setTxInfo] = useState<TxInfo>(null);
  const [allTxInfo, setAllTxInfo] = useState<TxInfo[]>(null);
  const [txLogs, setTxLogs] = useState<TxLogInfo[]>(null);

  const txProviderValue: ITxProvider = {
    loadingTxInfo: loadingTxInfo,
    loadingAllTxInfo: loadingAllTxInfo,
    loadingTxLogs: loadingTxLogs,
    reloadingTx: reloadingTx,
    txInfo: txInfo,
    allTxInfo: allTxInfo,
    txLogs: txLogs,
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
    loadListAllTx: async () => {
      let ret: Promise<ProviderResult>;
      setLoadingAllTxInfo(true);
      try {
        let brt = await TxFactory.TxBusiness.listAllTx();
        if (brt.sucesso) {
          setLoadingAllTxInfo(false);
          setAllTxInfo(brt.dataResult);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: "Transactions load"
          };
        }
        else {
          setLoadingAllTxInfo(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem
          };
        }
      }
      catch (err) {
        setLoadingAllTxInfo(false);
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
