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
import { AnchorMode, PostConditionMode, setFee } from '@stacks/transactions';
import UserFactory from '../../Business/Factory/UserFactory';
import UserInfo from '../../DTO/Domain/UserInfo';
import UserAddressFactory from '../../Business/Factory/UserAddressFactory';
import { ChainEnum } from '../../DTO/Enum/ChainEnum';
import { address } from '@stacks/transactions/dist/cl';
import TxInfo from '../../DTO/Domain/TxInfo';
import { TxProvideResult } from '../../DTO/Contexts/TxProviderResult';

interface AddOrChangeUserReturn {
    success: boolean,
    message: string,
    user?: UserInfo
};

export default function SwapProvider(props: any) {

    const [loadingPrice, setLoadingPrice] = useState<boolean>(false);
    const [loadingExecute, setLoadingExecute] = useState<boolean>(false);
    const [senderCoin, _setSenderCoin] = useState<CoinEnum>(CoinEnum.BRL);
    const [receiverCoin, _setReceiverCoin] = useState<CoinEnum>(CoinEnum.USDT);
    const [senderPrice, setSenderPrice] = useState<number>(0);
    const [receiverPrice, setReceiverPrice] = useState<number>(0);
    const [senderAmount, _setSenderAmount] = useState<number>(0);
    const [receiverAmount, _setReceiverAmount] = useState<number>(0);
    const [senderProportion, setSenderProportion] = useState<number>(0);
    const [receiverProportion, setReceiverProportion] = useState<number>(0);
    const [senderPoolAddress, setSenderPoolAddress] = useState<string>(null);
    const [receiverPoolAddress, setReceiverPoolAddress] = useState<string>(null);
    const [senderPoolBalance, setSenderPoolBalance] = useState<BigInt>(BigInt(0));
    const [receiverPoolBalance, setReceiverPoolBalance] = useState<BigInt>(BigInt(0));
    const [currentTxId, setCurrentTxId] = useState<string>(null);

    const [senderFee, setSenderFee] = useState<number>(0);
    const [receiverFee, setReceiverFee] = useState<number>(0);

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

    const CoinToText = (coin: CoinEnum) => {
        let str: string = "";
        switch (coin) {
            case CoinEnum.Bitcoin:
                str = "Bitcoin";
                break;
            case CoinEnum.Stacks:
                str = "Stacks";
                break;
            case CoinEnum.USDT:
                str = "USDT";
                break;
            case CoinEnum.BRL:
                str = "Real (PIX)";
                break;
        }
        return str;
    };

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

        let fee = senderFee;
        setSenderFee(receiverFee);
        setReceiverFee(fee);
    };

    const getFormatedAmount = (coin: CoinEnum, amount: number) => {
        if (amount) {
            let retorno: string;
            switch (coin) {
                case CoinEnum.Bitcoin:
                    retorno = Number(amount).toFixed(5).toString() + " BTC";
                    break;
                case CoinEnum.Stacks:
                    retorno = Number(amount).toFixed(5).toString() + " STX";
                    break;
                case CoinEnum.USDT:
                    retorno = Number(amount).toFixed(2).toString() + " USDT";
                    break;
                case CoinEnum.BRL:
                    retorno = "R$ " + Number(amount).toFixed(2).toString();
                    break;
            }
            return retorno;
        }
        return "~";
    };

    const getFormatBalance = (coin: CoinEnum, amount: BigInt) => {
        if (amount) {
            let retorno: string;
            switch (coin) {
                case CoinEnum.Bitcoin:
                    retorno = (Number(amount) / 10000000).toFixed(5).toString() + " BTC";
                    break;
                case CoinEnum.Stacks:
                    retorno = (Number(amount) / 10000000).toFixed(5).toString() + " STX";
                    break;
                case CoinEnum.USDT:
                    retorno = (Number(amount) / 10000000).toFixed(2).toString() + " USDT";
                    break;
                case CoinEnum.BRL:
                    retorno = "R$ " + (Number(amount) / 10000000).toFixed(2).toString();
                    break;
            }
            return retorno;
        }
        return "~";
    };

    const loadCurrentPriceHandle = async (sender: CoinEnum, receiver: CoinEnum) => {
        let ret: Promise<ProviderResult>;
        setLoadingPrice(true);
        try {
            let retPrice = await PriceFactory.PriceBusiness.getCurrentPrice(sender, receiver);
            setLoadingPrice(false);
            //console.log("Price:", retPrice);
            if (retPrice.sucesso) {
                setSenderPoolAddress(retPrice.dataResult.senderPoolAddr);
                setReceiverPoolAddress(retPrice.dataResult.receiverPoolAddr);
                setSenderPoolBalance(retPrice.dataResult.senderPoolBalance);
                setReceiverPoolBalance(retPrice.dataResult.receiverPoolBalance);
                setSenderPrice(retPrice.dataResult.senderPrice);
                setReceiverPrice(retPrice.dataResult.receiverPrice);
                setSenderProportion(retPrice.dataResult.senderProportion);
                setReceiverProportion(retPrice.dataResult.receiverProportion);
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
    };

    const updateAmountHandle = (value: number) => {
        let fee: number = value * 0.03;
        let realValue: number = value - fee;

        let fullReceiverValue: number = senderProportion * value;
        let receiverValue: number = senderProportion * realValue;

        if (senderCoin != CoinEnum.BRL) {
            setSenderFee(value - realValue);
            setReceiverFee(0);
        }
        else {
            setSenderFee(0);
            setReceiverFee(fullReceiverValue - receiverValue);
        }
        _setSenderAmount(value);
        _setReceiverAmount(parseFloat(receiverValue.toFixed(5)));
    };

    const getFormatedPriceHandle = (coin: CoinEnum, price: number) => {
        if (price) {
            if (coin == CoinEnum.BRL) {
                return price.toLocaleString('pt-BR', {
                    style: 'currency',
                    currency: 'BRL',
                });
            }
            else {
                return price.toLocaleString('en-US', {
                    style: 'currency',
                    currency: 'USD',
                });
            }
        }
        return "~";
    };

    const addOrChangeUserHandle = async (chain: ChainEnum, email?: string, receiverAddr?: string) => {
        let ret: AddOrChangeUserReturn;
        let user: UserInfo;
        let authSession = AuthFactory.AuthBusiness.getSession();
        if (authSession && authSession.id) {
            let userRet = await UserFactory.UserBusiness.getMe();
            if (userRet.sucesso) {
                user = userRet.dataResult;
                if (email && !user.email) {
                    //let user: UserInfo = userRet.dataResult;
                    user.email = email;
                    let updateRet = await UserFactory.UserBusiness.update(user);
                    if (!updateRet.sucesso) {
                        return {
                            ...ret,
                            success: false,
                            message: updateRet.mensagem
                        }
                    }
                }
            }
            else {
                return {
                    ...ret,
                    success: false,
                    message: userRet.mensagem
                }
            }
        }
        else {
            if (email) {
                let emailRet = await UserFactory.UserBusiness.getUserByEmail(email);
                if (emailRet.sucesso) {
                    user = emailRet.dataResult;
                }
                else {
                    return {
                        ...ret,
                        success: false,
                        message: emailRet.mensagem
                    };
                }
            }
            if (!user) {
                let insertRet = await UserFactory.UserBusiness.insert({
                    ...user,
                    email: email
                });
                if (insertRet.sucesso) {
                    user = insertRet.dataResult;
                }
                else {
                    return {
                        ...ret,
                        success: false,
                        message: insertRet.mensagem
                    };
                }
            }
        }
        if (!user) {
            return {
                ...ret,
                success: false,
                message: "User not found and cant be created"
            };
        }
        if (receiverAddr) {
            let addrRet = await UserAddressFactory.UserAddressBusiness.addOrChangeAddress(user.id, chain, receiverAddr);
            if (!addrRet.sucesso) {
                return {
                    ...ret,
                    success: false,
                    message: addrRet.mensagem
                }
            }
        }
        return {
            ...ret,
            success: true,
            user: user
        };
    };

    const swapProviderValue: ISwapProvider = {
        loadingPrice: loadingPrice,
        loadingExecute: loadingExecute,
        senderCoin: senderCoin,
        receiverCoin: receiverCoin,
        senderPrice: senderPrice,
        receiverPrice: receiverPrice,
        senderAmount: senderAmount,
        receiverAmount: receiverAmount,
        senderProportion: senderProportion,
        receiverProportion: receiverProportion,
        senderPoolAddress: senderPoolAddress,
        receiverPoolAddress: receiverPoolAddress,
        senderPoolBalance: senderPoolBalance,
        receiverPoolBalance: receiverPoolBalance,
        currentTxId: currentTxId,
        senderFee: senderFee,
        receiverFee: receiverFee,
        getFormatedSenderAmount: () => {
            return getFormatedAmount(senderCoin, senderAmount);
        },
        getFormatedReceiverAmount: () => {
            return getFormatedAmount(receiverCoin, receiverAmount);
        },
        setSenderCoin: async (value: CoinEnum) => {
            let oldCoin: CoinEnum = senderCoin;
            let _receiverCoin: CoinEnum = receiverCoin;
            _setSenderCoin(value);
            if (value == receiverCoin) {
                _receiverCoin = oldCoin;
                _setReceiverCoin(_receiverCoin);
            }
            let ret = await loadCurrentPriceHandle(value, _receiverCoin);
            if (ret.sucesso) {
                updateAmountHandle(senderAmount);
            }
            return ret;
        },
        setReceiverCoin: async (value: CoinEnum) => {
            let oldCoin = receiverCoin;
            let _senderCoin: CoinEnum = senderCoin;
            _setReceiverCoin(value);
            if (value == senderCoin) {
                _senderCoin = oldCoin;
                _setSenderCoin(_senderCoin);
            }
            let ret = await loadCurrentPriceHandle(_senderCoin, value);
            if (ret.sucesso) {
                updateAmountHandle(senderAmount);
            }
            return ret;
        },
        setSenderAmount: (value: number) => {
            updateAmountHandle(value);
        },
        getFormatedSenderPrice: () => {
            return getFormatedPriceHandle(senderCoin, senderPrice);
        },
        getFormatedReceiverPrice: () => {
            return getFormatedPriceHandle(receiverCoin, receiverPrice);
        },
        getFormatedSenderBalance: () => {
            return getFormatBalance(senderCoin, senderPoolBalance);
        },
        getFormatedReceiverBalance: () => {
            return getFormatBalance(receiverCoin, receiverPoolBalance);
        },
        getFormatedSenderFee: () => {
            if (senderFee) {
                return getFormatedAmount(senderCoin, senderFee);
            }
            return "~";
        },
        getFormatedReceiverFee: () => {
            if (receiverFee) {
                return getFormatedAmount(receiverCoin, receiverFee);
            }
            return "~";
        },
        getCoinText: () => {
            //return (destCoin == CoinEnum.Bitcoin) ? btcToStxText : stxToBtcText;
            return "";
        },
        loadCurrentPrice: async () => {
            return await loadCurrentPriceHandle(senderCoin, receiverCoin);
        },
        /*
        reverseAllWithoutCoin: () => {
            reverseAllWithoutCoinHandler();
        },
        */
        reverseCoin: () => {
            let coin = senderCoin;
            _setSenderCoin(receiverCoin);
            _setReceiverCoin(coin);
            reverseAllWithoutCoinHandler();
        },
        createTx: async (chain: ChainEnum, email?: string, receiverAddr?: string) => {
            let ret: Promise<TxProvideResult>;
            let tx: TxParamInfo;
            setLoadingExecute(true);
            let addOrChangeRet = await addOrChangeUserHandle(chain, email, receiverAddr);
            if (!addOrChangeRet.success) {
                setLoadingExecute(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: addOrChangeRet.message
                };
            }
            console.log("addOrChangeRet: ", JSON.stringify(addOrChangeRet));
            let txRet = await TxFactory.TxBusiness.createTx({
                ...tx,
                userid: addOrChangeRet.user.id,
                sendercoin: CoinToStr(senderCoin),
                receivercoin: CoinToStr(receiverCoin),
                senderaddress: null,
                receiveraddress: receiverAddr,
                sendertxid: null,
                senderamount: parseInt((senderAmount * 100000000).toFixed(0)),
                receiveramount: parseInt((receiverAmount * 100000000).toFixed(0)),
                senderfee: parseInt((senderFee * 100000000).toFixed(0)),
                receiverfee: parseInt((receiverFee * 100000000).toFixed(0))
            });
            console.log("txRet: ", JSON.stringify(txRet));
            if (txRet.sucesso) {
                setLoadingExecute(false);
                return {
                    ...ret,
                    sucesso: true,
                    hash: txRet.dataResult,
                    mensagemSucesso: "Transaction successyful created!"
                };
            }
            else {
                setLoadingExecute(false);
                return {
                    ...ret,
                    sucesso: false,
                    txId: 0,
                    mensagemErro: txRet.mensagem
                };
            }
        },
        payWithWallet: async (callback: any) => {
            let ret: Promise<ProviderResult>;
            setLoadingExecute(true);
            let userSession = AuthFactory.AuthBusiness.getSession();
            if (!userSession) {
                let retErro = {
                    ...ret,
                    sucesso: false,
                    mensagemErro: "Not logged"
                };
                setLoadingExecute(false);
                callback(retErro);
                return;
            }
            if (senderCoin == CoinEnum.Bitcoin) {
                let amount = parseInt((senderAmount * 100000000).toFixed(0));
                window.transferBitcoin(senderPoolAddress, amount, "testnet", (txid: string) => {
                    setCurrentTxId(txid);
                    let param: TxParamInfo = {
                        userid: 0,
                        sendercoin: CoinToStr(CoinEnum.Bitcoin),
                        receivercoin: CoinToStr(CoinEnum.Stacks),
                        senderaddress: "",
                        receiveraddress: "",
                        sendertxid: txid,
                        senderamount: senderAmount,
                        receiveramount: receiverAmount,
                        senderfee: senderFee,
                        receiverfee: receiverFee
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
                            userid: 0,
                            sendercoin: CoinToStr(CoinEnum.Bitcoin),
                            receivercoin: CoinToStr(CoinEnum.Stacks),
                            senderaddress: "",
                            receiveraddress: "",
                            senderamount: senderAmount,
                            receiveramount: receiverAmount,
                            senderfee: senderFee,
                            receiverfee: receiverFee,
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
