import BusinessResult from "../../DTO/Business/BusinessResult";
import { CoinEnum } from "../../DTO/Enum/CoinEnum";
import { PriceResult } from "../../DTO/Services/PriceResult";
import { IPriceService } from "../../Services/Interfaces/IPriceService";

export interface IPriceBusiness {
  init: (priceService: IPriceService) => void;
  getCurrentPrice: (senderCoin: CoinEnum, receiverCoin: CoinEnum) => Promise<BusinessResult<PriceResult>>;
}