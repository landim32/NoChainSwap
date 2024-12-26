import BusinessResult from "../../DTO/Business/BusinessResult";
import UserAddressInfo from "../../DTO/Domain/UserAddressInfo";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import IUserAddressService from "../../Services/Interfaces/IUserAddressService";

export default interface IUserAddressBusiness {
  init: (userAddrService: IUserAddressService) => void;
  listAddressByUser: () => Promise<BusinessResult<UserAddressInfo[]>>;
  getAddressByChain: (chainId: ChainEnum) => Promise<BusinessResult<UserAddressInfo>>;
  addOrChangeAddress: (userId: number, chainId: ChainEnum, address: string) => Promise<BusinessResult<boolean>>;
  removeAddress: (chainId: ChainEnum) => Promise<BusinessResult<boolean>>;
}