import BusinessResult from "../../DTO/Business/BusinessResult";
import UserInfo from "../../DTO/Domain/UserInfo";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import IUserService from "../../Services/Interfaces/IUserService";
import IUserBusiness from "../Interfaces/IUserBusiness";

let _userService : IUserService;

const UserBusiness : IUserBusiness = {
  init: function (userService: IUserService): void {
    _userService = userService;
  },
  getUserById: async (userId: number) => {
    try {
      let ret: BusinessResult<UserInfo>;
      let retServ = await _userService.getUserById(userId);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
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
  getUserByAddress: async (chain: ChainEnum, address: string) => {
    try {
      let ret: BusinessResult<UserInfo>;
      let retServ = await _userService.getUserByAddress(chain, address);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
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
  getUserByEmail: async (email: string) => {
    try {
      let ret: BusinessResult<UserInfo>;
      let retServ = await _userService.getUserByEmail(email);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
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
      throw new Error("Failed to get user by email");
    }
  },
  insert: async (user: UserInfo) => {
    try {
      let ret: BusinessResult<UserInfo>;
      let retServ = await _userService.insert(user);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
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
      throw new Error("Failed to insert");
    }
  },
  update: async (user: UserInfo) => {
    try {
      let ret: BusinessResult<UserInfo>;
      let retServ = await _userService.update(user);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
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
      throw new Error("Failed to update");
    }
  },
  loginWithEmail: async (email: string, password: string) => {
    try {
      let ret: BusinessResult<UserInfo>;
      let retServ = await _userService.loginWithEmail(email, password);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
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
      throw new Error("Failed to login with email");
    }
  },
  changePassword: async (userId: number, oldPassword: string, newPassword: string) => {
    try {
      let ret: BusinessResult<boolean>;
      let retServ = await _userService.changePassword(userId, oldPassword, newPassword);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: ret.sucesso,
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
      throw new Error("Failed to change password");
    }
  },
  sendRecoveryEmail: async (email: string) => {
    try {
      let ret: BusinessResult<boolean>;
      let retServ = await _userService.sendRecoveryEmail(email);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: ret.sucesso,
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
      throw new Error("Failed to send recovery email");
    }
  },
  changePasswordUsingHash: async (recoveryHash: string, newPassword: string) => {
    try {
      let ret: BusinessResult<boolean>;
      let retServ = await _userService.changePasswordUsingHash(recoveryHash, newPassword);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: ret.sucesso,
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
      throw new Error("Failed to change password using hash");
    }
  }
}

export default UserBusiness;