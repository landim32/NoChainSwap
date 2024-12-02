import BusinessResult from "../../DTO/Business/BusinessResult";
import UserAddressInfo from "../../DTO/Domain/UserAddressInfo";
import IUserAddressService from "../../Services/Interfaces/IUserAddressService";

export default interface IUserAddressBusiness {
  init: (userAddrService: IUserAddressService) => void;
  listAddressByUser: (userId: number) => Promise<BusinessResult<UserAddressInfo[]>>;
  getAddressByChain: (userId: number, chainId: number) => Promise<BusinessResult<UserAddressInfo>>;
  addOrChangeAddress: (userId: number, chainId: number, address: string) => Promise<BusinessResult<boolean>>;
  removeAddress: (userId: number, chainId: number) => Promise<BusinessResult<boolean>>;
}