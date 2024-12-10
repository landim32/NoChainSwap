import BusinessResult from "../../DTO/Business/BusinessResult";
import UserAddressInfo from "../../DTO/Domain/UserAddressInfo";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import IUserAddressService from "../../Services/Interfaces/IUserAddressService";

export default interface IUserAddressBusiness {
  init: (userAddrService: IUserAddressService) => void;
  listAddressByUser: (userId: number) => Promise<BusinessResult<UserAddressInfo[]>>;
  getAddressByChain: (userId: number, chainId: ChainEnum) => Promise<BusinessResult<UserAddressInfo>>;
  addOrChangeAddress: (userId: number, chainId: ChainEnum, address: string) => Promise<BusinessResult<boolean>>;
  removeAddress: (userId: number, chainId: ChainEnum) => Promise<BusinessResult<boolean>>;
}