import ServiceFactory from '../../Services/ServiceFactory';
import TxBusiness from '../Impl/TxBusiness';
import ITxBusiness from '../Interfaces/ITxBusiness';

const txService = ServiceFactory.TxService;

const txBusinessImpl: ITxBusiness = TxBusiness;
txBusinessImpl.init(txService);

const TxFactory = {
  TxBusiness: txBusinessImpl
};

export default TxFactory;
