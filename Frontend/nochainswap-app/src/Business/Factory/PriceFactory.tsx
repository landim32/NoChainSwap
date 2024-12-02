import ServiceFactory from '../../Services/ServiceFactory';
import PriceBusiness from '../Impl/PriceBusiness';
import IPriceBusiness from '../Interfaces/IPriceBusiness';

const priceService = ServiceFactory.PriceService;

const priceBusinessImpl: IPriceBusiness = PriceBusiness;
priceBusinessImpl.init(priceService);

const PriceFactory = {
  PriceBusiness: priceBusinessImpl
};

export default PriceFactory;
