import BusinessResult from "../../DTO/Business/BusinessResult";
import AuthSession from "../../DTO/Domain/AuthSession";
import { AppConfig, getUserData, UserData, UserSession, showConnect, disconnect } from '@stacks/connect';
import IStacksBusiness from "../Interfaces/IStacksBusiness";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import { SignInEnum } from "../../DTO/Enum/SignInEnum";
import env from "react-dotenv";

const StacksBusiness : IStacksBusiness = {
  logIn: (callback?: any) => {
    var ret: BusinessResult<boolean>;
    const appConfig = new AppConfig(['store_write', 'publish_data']);
    const userSession = new UserSession({ appConfig });
    showConnect({
      userSession, // `userSession` from previous step, to access storage
      appDetails: {
        name: env.PROJECT_NAME,
        icon: window.location.origin + '/public/logo192.png'
      },
      onFinish: () => {
        if (callback) {
          callback();
        }
      },
      onCancel: () => {
        console.log('oops'); 
      },
    });
  },
  logOut: () => {
    disconnect();
  },
  getSession: async () => {
    let ret: BusinessResult<boolean>;
    let lUserData = await getUserData();
    //console.log("UserData: ", JSON.stringify(lUserData));
    if (lUserData) {
      let userSession: AuthSession;
      let btcAddr = lUserData.profile?.btcAddress?.p2wpkh?.testnet;
      let userAddr = btcAddr.substr(0, 6) + '...' + btcAddr.substr(-4);
      return {
        ...ret,
        sucesso: true,
        dataResult: {
          ...userSession,
          name: userAddr,
          address: btcAddr,
          loginWith: SignInEnum.WebWallet,
          chain: ChainEnum.BitcoinAndStack
        },
      };
    }
    return {
      ...ret,
      sucesso: false,
      dataResult: null,
    };
  }
}

export default StacksBusiness;