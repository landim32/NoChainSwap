import React, { useContext, useState } from 'react';
import ProviderResult from '../../DTO/Contexts/ProviderResult';
import SwapContext from './SwapContext';
import ISwapProvider from '../../DTO/Contexts/ISwapProvider';
import { CoinEnum } from '../../DTO/Enum/CoinEnum';
import PriceFactory from '../../Business/Factory/PriceFactory';
import TxFactory from '../../Business/Factory/TxFactory';
import TxParamInfo from '../../DTO/Domain/TxParamInfo';
import AuthFactory from '../../Business/Factory/AuthFactory';
import { openSTXTransfer } from '@stacks/connect';
import { AnchorMode, PostConditionMode } from '@stacks/transactions';

export default function SwapProvider(props: any) {

    const [loadingPrice, setLoadingPrice] = useState<boolean>(false);
    const [loadingExecute, setLoadingExecute] = useState<boolean>(false);
    const [senderCoin, _setSenderCoin] = useState<CoinEnum>(CoinEnum.Bitcoin);
    const [receiverCoin, _setReceiverCoin] = useState<CoinEnum>(CoinEnum.Stacks);
    const [senderPrice, setSenderPrice] = useState<number>(0);
    const [receiverPrice, setReceiverPrice] = useState<number>(0);
    const [senderAmount, _setSenderAmount] = useState<number>(0);
    const [receiverAmount, _setReceiverAmount] = useState<number>(0);
    const [senderProportion, setSenderProportion] = useState<number>(0);
    const [receiverProportion, setReceiverProportion] = useState<number>(0);
    //const [btcToStxText, setBtcToStxText] = useState<string>(null);
    //const [stxToBtcText, setStxToBtcText] = useState<string>(null);
    const [senderPoolAddress, setSenderPoolAddress] = useState<string>(null);
    const [receiverPoolAddress, setReceiverPoolAddress] = useState<string>(null);
    const [senderPoolBalance, setSenderPoolBalance] = useState<BigInt>(BigInt(0));
    const [receiverPoolBalance, setReceiverPoolBalance] = useState<BigInt>(BigInt(0));
    const [currentTxId, setCurrentTxId] = useState<string>(null);

    const reverseAllWithoutCoinHandler = () => {
        let amount = senderAmount;
        _setSenderAmount(receiverAmount);
        _setReceiverAmount(amount);

        let poolAddr = senderPoolAddress;
        setSenderPoolAddress(receiverPoolAddress);
        setReceiverPoolAddress(poolAddr);

        let poolBalance = senderPoolBalance;
        setSenderPoolBalance(receiverPoolBalance);
        setReceiverPoolBalance(poolBalance);

        let price = senderPrice;
        setSenderPrice(receiverPrice);
        setReceiverPrice(price);

        let proportion = senderProportion;
        setSenderProportion(receiverProportion);
        setReceiverProportion(proportion);
    };

    const swapProviderValue: ISwapProvider = {
        loadingPrice: loadingPrice,
        senderCoin: senderCoin,
        receiverCoin: receiverCoin,
        senderPrice: senderPrice,
        receiverPrice: receiverPrice,
        senderAmount: senderAmount,
        receiverAmount: receiverAmount,
        senderProportion: senderProportion,
        receiverProportion: receiverProportion,
        //btcToStxText: btcToStxText,
        //stxToBtcText: stxToBtcText,
        senderPoolAddress: senderPoolAddress,
        receiverPoolAddress: receiverPoolAddress,
        senderPoolBalance: senderPoolBalance,
        receiverPoolBalance: receiverPoolBalance,
        currentTxId: currentTxId,
        getFormatedSenderAmount: () => {
            if (senderAmount) {
                if (senderCoin == CoinEnum.Bitcoin) {
                    return Number(senderAmount).toFixed(5).toString() + " BTC";
                }
                else {
                    return Number(senderAmount).toFixed(5).toString() + " STX";
                }
            }
            return "~";
        },
        getFormatedReceiverAmount: () => {
            if (receiverAmount) {
                if (receiverCoin == CoinEnum.Bitcoin) {
                    return Number(receiverAmount).toFixed(5).toString() + " BTC";
                }
                else {
                    return Number(receiverAmount).toFixed(5).toString() + " STX";
                }
            }
            return "~";
        },
        setSenderCoin: (value: CoinEnum) => {
            _setSenderCoin(value);
            if (value == CoinEnum.Bitcoin) {
                if (receiverCoin != CoinEnum.Stacks) {
                    _setReceiverCoin(CoinEnum.Stacks);
                    reverseAllWithoutCoinHandler();
                }
            }
            else {
                if (receiverCoin != CoinEnum.Bitcoin) {
                    _setReceiverCoin(CoinEnum.Bitcoin);
                    reverseAllWithoutCoinHandler();
                }
            }
        },
        setReceiverCoin: (value: CoinEnum) => {
            _setReceiverCoin(value);
            if (value == CoinEnum.Bitcoin) {
                if (senderCoin != CoinEnum.Stacks) {
                    _setSenderCoin(CoinEnum.Stacks);
                    reverseAllWithoutCoinHandler();
                }
            }
            else {
                if (senderCoin != CoinEnum.Bitcoin) {
                    _setSenderCoin(CoinEnum.Bitcoin);
                    reverseAllWithoutCoinHandler();
                }
            }
        },
        setSenderAmount: (value: number) => {
            _setSenderAmount(value);
            let price = senderProportion * value;
            _setReceiverAmount(parseFloat(price.toFixed(5)));
        },
        getFormatedSenderPrice: () => {
            if (senderPrice) {
                return senderPrice.toLocaleString('en-US', {
                    style: 'currency',
                    currency: 'USD',
                });
            }
            return "~";
        },
        getFormatedReceiverPrice: () => {
            if (receiverPrice) {
                return receiverPrice.toLocaleString('en-US', {
                    style: 'currency',
                    currency: 'USD',
                });
            }
            return "~";
        },
        getFormatedSenderBalance: () => {
            if (senderPoolBalance) {
                if (senderCoin == CoinEnum.Bitcoin) {
                    return (Number(senderPoolBalance) / 10000000).toFixed(5).toString() + " BTC";
                }
                else {
                    return (Number(senderPoolBalance) / 10000000).toFixed(5).toString() + " STX";
                }
            }
            return "~";
        },
        getFormatedReceiverBalance: () => {
            if (receiverPoolBalance) {
                if (receiverCoin == CoinEnum.Bitcoin) {
                    return (Number(receiverPoolBalance) / 10000000).toFixed(5).toString() + " BTC";
                }
                else {
                    return (Number(receiverPoolBalance) / 10000000).toFixed(5).toString() + " STX";
                }
            }
            return "~";
        },
        getCoinText: () => {
            //return (destCoin == CoinEnum.Bitcoin) ? btcToStxText : stxToBtcText;
            return "";
        },
        loadCurrentPrice: async () => {
            let ret: Promise<ProviderResult>;
            setLoadingPrice(false);
            try {
                let retPrice = await PriceFactory.PriceBusiness.getCurrentPrice(senderCoin, receiverCoin);
                setLoadingPrice(true);
                console.log("Price:", retPrice);
                if (retPrice.sucesso) {
                    setSenderPoolAddress(retPrice.dataResult.senderPoolAddr);
                    setReceiverPoolAddress(retPrice.dataResult.receiverPoolAddr);
                    setSenderPoolBalance(retPrice.dataResult.senderPoolBalance);
                    setReceiverPoolBalance(retPrice.dataResult.receiverPoolBalance);
                    setSenderPrice(retPrice.dataResult.senderPrice);
                    setReceiverPrice(retPrice.dataResult.receiverPrice);
                    setSenderProportion(retPrice.dataResult.senderProportion);
                    setReceiverProportion(retPrice.dataResult.receiverProportion);
                    //setBtcToStxText(retPrice.dataResult.btcToStxText);
                    //setStxToBtcText(retPrice.dataResult.stxToBtcText);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: retPrice.mensagem
                    };
                }
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: retPrice.mensagem
                };
            } catch (err) {
                setLoadingPrice(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        reverseAllWithoutCoin: () => {
            reverseAllWithoutCoinHandler();
        },
        reverseCoin: () => {
            let coin = senderCoin;
            _setSenderCoin(receiverCoin);
            _setReceiverCoin(coin);
            reverseAllWithoutCoinHandler();
        },
        execute: async (callback: any) => {
            let ret: Promise<ProviderResult>;
            setLoadingExecute(true);
            let retSession = await AuthFactory.AuthBusiness.getSession();
            if (!retSession.sucesso) {
                let retErro = {
                    ...ret,
                    sucesso: false,
                    mensagemErro: retSession.mensagem
                };
                setLoadingExecute(false);
                callback(retErro);
                return;
            }
            let userSession = retSession.dataResult;
            if (senderCoin == CoinEnum.Bitcoin) {
                let amount = parseInt((senderAmount * 100000000).toFixed(0));
                window.transferBitcoin(senderPoolAddress, amount, "testnet", (txid: string) => {
                    setCurrentTxId(txid);
                    let param: TxParamInfo = {
                        sendercoin: "btc",
                        receivercoin: "stx",
                        senderaddress: "", //userSession.btcAddress,
                        receiveraddress: "", //userSession.stxAddress,
                        sendertxid: txid
                    }
                    TxFactory.TxBusiness.createTx(param).then((retTx: any) => {
                        if (retTx.sucesso) {
                            let retSuccess = {
                                ...ret,
                                sucesso: true,
                                mensagemSucesso: "Transaction started successfully"
                            };
                            setLoadingExecute(false);
                            callback(retSuccess);
                        }
                        else {
                            let retErro = {
                                ...ret,
                                sucesso: false,
                                mensagemErro: retTx.mensagem
                            };
                            setLoadingExecute(false);
                            callback(retErro);
                        }
                    });
                });
            }
            else {
                // transaction in STX
                let amount = parseInt((senderAmount * 1000000).toFixed(0));
                openSTXTransfer({
                    network: 'testnet', // which network to use; ('mainnet' or 'testnet')
                    anchorMode: AnchorMode.Any, // which type of block the tx should be mined in
                  
                    recipient: senderPoolAddress, // which address we are sending to
                    amount: BigInt(amount), // tokens, denominated in micro-STX
                  
                    onFinish: (response: any) => {
                      // WHEN user confirms pop-up
                      console.log(response.txid); // the response includes the txid of the transaction
                      setCurrentTxId(response.txid);
                      let param: TxParamInfo = {
                        sendercoin: "btc",
                        receivercoin: "stx",
                        senderaddress: "", //userSession.stxAddress,
                        receiveraddress: "", //userSession.btcAddress,
                        sendertxid: response.txid
                    }
                      TxFactory.TxBusiness.createTx(param).then((retTx: any) => {
                          if (retTx.sucesso) {
                              let retSuccess = {
                                  ...ret,
                                  sucesso: true,
                                  mensagemSucesso: "Transaction started successfully"
                              };
                              setLoadingExecute(false);
                              callback(retSuccess);
                          }
                          else {
                              let retErro = {
                                  ...ret,
                                  sucesso: false,
                                  mensagemErro: retTx.mensagem
                              };
                              setLoadingExecute(false);
                              callback(retErro);
                          }
                      });
                    },
                    onCancel: () => {
                      // WHEN user cancels/closes pop-up
                      console.log('User canceled');
                    },
                });
            }
            return ret;
        }
    };

    return (
        <SwapContext.Provider value={swapProviderValue}>
            {props.children}
        </SwapContext.Provider>
    );
}
