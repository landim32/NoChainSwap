import StacksBusiness from '../Impl/StacksBusiness';
import IStacksBusiness from '../Interfaces/IStacksBusiness';

const stacksBusinessImpl: IStacksBusiness = StacksBusiness;

const StacksFactory = {
  StacksBusiness: stacksBusinessImpl
};

export default StacksFactory;
