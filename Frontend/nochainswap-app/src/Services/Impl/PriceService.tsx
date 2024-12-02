import { PriceResult } from "../../DTO/Services/PriceResult";
import IHttpClient from "../../Infra/Interface/IHttpClient";
import IPriceService from "../Interfaces/IPriceService";

let _httpClient : IHttpClient;

const PriceService : IPriceService = {
    init: function (htppClient: IHttpClient): void {
        _httpClient = htppClient;
    },
    getCurrentPrice: async (senderCoin: string, receiverCoin: string) => {
        let ret: PriceResult;
        let url = "api/CoinMarketCap/getcurrentprice/" + senderCoin + "/" + receiverCoin;
        let request = await _httpClient.doGet<PriceResult>(url, {});
        if (request.success) {
            request.data.sucesso = true;
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
}

export default PriceService;