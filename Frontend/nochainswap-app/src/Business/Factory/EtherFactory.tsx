import EtherBusiness from '../Impl/EtherBusiness';
import IEtherBusiness from '../Interfaces/IEtherBusiness';

const etherBusinessImpl: IEtherBusiness = EtherBusiness;

const EtherFactory = {
  EtherBusiness: etherBusinessImpl
};

export default EtherFactory;
