import StatusRequest from "./StatusRequest";

export interface PriceResult extends StatusRequest {
    senderPrice: number,
    senderProportion: number,
    senderPoolAddr: string,
    senderPoolBalance: BigInt,
    sender: {
        id: string,
        convertCurrency: string,
        lastUpdated: string,
        marketCapConvert: number,
        marketCapUsd: number,
        name: string,
        percentChange1h: number,
        percentChange24h: number,
        percentChange7d: number,
        price: number,
        rank: string,
        symbol: string,
        volume24hUsd: number
    },
    receiverPrice: number,
    receiverProportion: number,
    receiverPoolAddr: string,
    receiverPoolBalance: BigInt,
    receiver: {
        id: string,
        convertCurrency: string,
        lastUpdated: string,
        marketCapConvert: number,
        marketCapUsd: number,
        name: string,
        percentChange1h: number,
        percentChange24h: number,
        percentChange7d: number,
        price: number,
        rank: string,
        symbol: string,
        volume24hUsd: number
    }
}
