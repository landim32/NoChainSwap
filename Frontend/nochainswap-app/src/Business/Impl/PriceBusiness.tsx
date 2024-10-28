import BusinessResult from "../../DTO/Business/BusinessResult";
import { CoinEnum } from "../../DTO/Enum/CoinEnum";
import { PriceResult } from "../../DTO/Services/PriceResult";
import { IPriceService } from "../../Services/Interfaces/IPriceService";
import { IPriceBusiness } from "../Interfaces/IPriceBusiness";

let _priceService: IPriceService;

const PriceBusiness: IPriceBusiness = {
  init: function (priceService: IPriceService): void {
    _priceService = priceService;
  },
  getCurrentPrice: async (senderCoin: CoinEnum, receiverCoin: CoinEnum) => {
    try {
      let ret: BusinessResult<PriceResult> = null;
      let senderStr = "", receiverStr = "";
      switch (senderCoin) {
        case CoinEnum.Bitcoin:
          senderStr = "btc";
          break;
        case CoinEnum.Stacks:
          senderStr = "stx";
          break;
      }
      switch (receiverCoin) {
        case CoinEnum.Bitcoin:
          receiverStr = "btc";
          break;
        case CoinEnum.Stacks:
          receiverStr = "stx";
          break;
      }
      let retPool = await _priceService.getCurrentPrice(senderStr, receiverStr);
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

export { PriceBusiness };