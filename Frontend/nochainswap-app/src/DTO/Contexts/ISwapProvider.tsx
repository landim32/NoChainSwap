import { CoinEnum } from "../Enum/CoinEnum";
import ProviderResult from "./ProviderResult";

interface ISwapProvider {
    loadingPrice: boolean;
    senderCoin: CoinEnum;
    receiverCoin: CoinEnum;
    senderPrice: number;
    receiverPrice: number;
    senderAmount: number;
    receiverAmount: number;
    senderProportion: number;
    receiverProportion: number;
    //btcToStxText: string;
    //stxToBtcText: string;
    senderPoolAddress: string;
    receiverPoolAddress: string;
    senderPoolBalance: BigInt;
    receiverPoolBalance: BigInt;
    currentTxId: string;
    getFormatedSenderAmount: () => string;
    getFormatedReceiverAmount: () => string;
    getFormatedSenderPrice: () => string;
    getFormatedReceiverPrice: () => string;
    getFormatedSenderBalance: () => string;
    getFormatedReceiverBalance: () => string;
    setSenderCoin: (value: CoinEnum) => void;
    setReceiverCoin: (value: CoinEnum) => void;
    setSenderAmount: (value: number) => void;
    getCoinText: () => string;
    loadCurrentPrice: () => Promise<ProviderResult>;
    reverseAllWithoutCoin: () => void;
    reverseCoin: () => void;
    execute: (callback: any) => void;
}

export default ISwapProvider;