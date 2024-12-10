import BusinessResult from "../../DTO/Business/BusinessResult";
import { CoinEnum } from "../../DTO/Enum/CoinEnum";
import { PriceResult } from "../../DTO/Services/PriceResult";
import IPriceService from "../../Services/Interfaces/IPriceService";
import IPriceBusiness from "../Interfaces/IPriceBusiness";

let _priceService: IPriceService;

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

const PriceBusiness: IPriceBusiness = {
  init: function (priceService: IPriceService): void {
    _priceService = priceService;
  },
  getCurrentPrice: async (senderCoin: CoinEnum, receiverCoin: CoinEnum) => {
    try {
      let ret: BusinessResult<PriceResult> = null;
      let retPool = await _priceService.getCurrentPrice(CoinToStr(senderCoin), CoinToStr(receiverCoin));
      //console.log("Price: ", JSON.stringify(retPool));
      if (retPool.sucesso) {
        return {
          ...ret,
          dataResult: retPool,
          sucesso: true
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retPool.mensagem
        };
      }
    } catch {
      throw new Error("Failed to get price information");
    }
  }
}

export default PriceBusiness;