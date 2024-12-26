import BusinessResult from "../../DTO/Business/BusinessResult";
import AuthSession from "../../DTO/Domain/AuthSession";
import UserAddressInfo from "../../DTO/Domain/UserAddressInfo";
import UserInfo from "../../DTO/Domain/UserInfo";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import IUserAddressService from "../../Services/Interfaces/IUserAddressService";
import IUserService from "../../Services/Interfaces/IUserService";
import AuthFactory from "../Factory/AuthFactory";
import IUserAddressBusiness from "../Interfaces/IUserAddressBusiness";
import IUserBusiness from "../Interfaces/IUserBusiness";

let _userAddrService : IUserAddressService;

const UserAddressBusiness : IUserAddressBusiness = {
  init: function (userAddrService: IUserAddressService): void {
    _userAddrService = userAddrService;
  },
  listAddressByUser: async () => {
    try {
        let ret: BusinessResult<UserAddressInfo[]>;
        let session: AuthSession = AuthFactory.AuthBusiness.getSession();
        if (!session) {
          return {
            ...ret,
            sucesso: false,
            mensagem: "Not logged"
          };
        }
        let retServ = await _userAddrService.listAddressByUser(session.token);
        if (retServ.sucesso) {
          return {
            ...ret,
            dataResult: retServ.userAddresses,
            sucesso: true
          };
        } else {
          return {
            ...ret,
            sucesso: false,
            mensagem: retServ.mensagem
          };
        }
      } catch {
        throw new Error("Failed to get user by address");
      }
  },
  getAddressByChain: async (chainId: ChainEnum) => {
    try {
        let ret: BusinessResult<UserAddressInfo>;
        let session: AuthSession = AuthFactory.AuthBusiness.getSession();
        if (!session) {
          return {
            ...ret,
            sucesso: false,
            mensagem: "Not logged"
          };
        }
        let retServ = await _userAddrService.getAddressByChain(chainId, session.token);
        if (retServ.sucesso) {
          return {
            ...ret,
            dataResult: retServ.userAddress,
            sucesso: true
          };
        } else {
          return {
            ...ret,
            sucesso: false,
            mensagem: retServ.mensagem
          };
        }
      } catch {
        throw new Error("Failed to get user by address");
      }
  },
  addOrChangeAddress: async (userId: number, chainId: ChainEnum, address: string) => {
    try {
        let ret: BusinessResult<boolean>;
        let retServ = await _userAddrService.addOrChangeAddress(userId, chainId, address);
        if (retServ.sucesso) {
          return {
            ...ret,
            dataResult: retServ.sucesso,
            sucesso: true
          };
        } else {
          return {
            ...ret,
            sucesso: false,
            mensagem: retServ.mensagem
          };
        }
      } catch {
        throw new Error("Failed to get user by address");
      }
  },
  removeAddress: async (chainId: ChainEnum) =>  {
    try {
        let ret: BusinessResult<boolean>;
        let session: AuthSession = AuthFactory.AuthBusiness.getSession();
        if (!session) {
          return {
            ...ret,
            sucesso: false,
            mensagem: "Not logged"
          };
        }
        let retServ = await _userAddrService.removeAddress(chainId, session.token);
        if (retServ.sucesso) {
          return {
            ...ret,
            dataResult: retServ.sucesso,
            sucesso: true
          };
        } else {
          return {
            ...ret,
            sucesso: false,
            mensagem: retServ.mensagem
          };
        }
      } catch {
        throw new Error("Failed to get user by address");
      }
  }
}

export default UserAddressBusiness;