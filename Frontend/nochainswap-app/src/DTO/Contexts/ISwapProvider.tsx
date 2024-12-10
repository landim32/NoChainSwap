import { ChainEnum } from "../Enum/ChainEnum";
import { CoinEnum } from "../Enum/CoinEnum";
import ProviderResult from "./ProviderResult";
import { TxProvideResult } from "./TxProviderResult";

interface ISwapProvider {
    loadingPrice: boolean;
    loadingExecute: boolean;
    senderCoin: CoinEnum;
    receiverCoin: CoinEnum;
    senderPrice: number;
    receiverPrice: number;
    senderAmount: number;
    receiverAmount: number;
    senderProportion: number;
    receiverProportion: number;
    senderPoolAddress: string;
    receiverPoolAddress: string;
    senderPoolBalance: BigInt;
    receiverPoolBalance: BigInt;
    currentTxId: string;
    senderFee: number;
    receiverFee: number;
    getFormatedSenderAmount: () => string;
    getFormatedReceiverAmount: () => string;
    getFormatedSenderPrice: () => string;
    getFormatedReceiverPrice: () => string;
    getFormatedSenderBalance: () => string;
    getFormatedReceiverBalance: () => string;
    getFormatedSenderFee: () => string;
    getFormatedReceiverFee: () => string;
    setSenderCoin: (value: CoinEnum) => Promise<ProviderResult>;
    setReceiverCoin: (value: CoinEnum) => Promise<ProviderResult>;
    setSenderAmount: (value: number) => void;
    getCoinText: () => string;
    loadCurrentPrice: (sender: CoinEnum, receiver: CoinEnum) => Promise<ProviderResult>;
    reverseCoin: () => void;
    createTx: (chain: ChainEnum, email?: string, receiverAddr?: string) => Promise<TxProvideResult>;
    payWithWallet: (callback: any) => void;
}

export default ISwapProvider;