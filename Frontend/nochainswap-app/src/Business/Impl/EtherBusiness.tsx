import BusinessResult from "../../DTO/Business/BusinessResult";
import AuthSession from "../../DTO/Domain/AuthSession";
import IAuthBusiness from "../Interfaces/IAuthBusiness";
import Web3 from 'web3';
import env from 'react-dotenv';
import IEtherBusiness from "../Interfaces/IEtherBusiness";
import ContractFactory from "../../Contracts/ContractFactory";

const LS_KEY = 'login-with-metamask:auth';

//let _authService : IAuthService;

const EtherBusiness : IEtherBusiness = {
  checkNetwork: async () => {
    let ret: BusinessResult<boolean>;

    let web3: Web3 | undefined = undefined;
    let ethereum: any = null;
    let buildErro = (msg: string) => {
      return ret = {
        ...ret,
        sucesso: false,
        mensagem: msg
      };
    };

    if (!(window as any).ethereum) {
      return buildErro('Please install MetaMask first.');
    }

    if (!web3) {
      try {
        await (window as any).ethereum.enable();
        ethereum = (window as any).ethereum;
        web3 = new Web3((window as any).ethereum);
      } catch (error) {
        return buildErro('You need to allow MetaMask.');
      }
    }

    try {
      await ethereum.request({
        method: 'wallet_switchEthereumChain',
        params: [{ chainId: '0x' + parseInt(env.REACT_APP_NETWORK).toString(16) }],
      });
    } catch (switchError: any) {
      // This error code indicates that the chain has not been added to MetaMask.
      if (switchError.code === 4902) {
        try {
          await ethereum.request({
            method: 'wallet_addEthereumChain',
            params: [
              { 
                chainId: '0x'+parseInt(env.REACT_APP_NETWORK).toString(16), 
                chainName: env.REACT_APP_CHAIN_NAME,
                rpcUrls: [env.REACT_APP_CHAIN_URL],
                blockExplorerUrls: [env.REACT_APP_CHAIN_EXPLORER],
                nativeCurrency: {
                  name: "BNB",
                  symbol: "BNB",
                  decimals: 18
                }
              }
            ],
          });
        } catch (addError) {
          // handle "add" error
        }
      }
      // handle other "switch" errors
    }

    return {
      ...ret,
      dataResult: true,
      sucesso: true,
    };
  },
  logIn: async (chainId: number) => {
    var ret: BusinessResult<string>;

    let web3: Web3 | undefined = undefined;

    if (!(window as any).ethereum) {
      return {
        ...ret,
        sucesso: false,
        dataResult: "",
        mensagem: 'Please install MetaMask first.'
      };
    }

    if (!web3) {
      try {
        await (window as any).ethereum.enable();

        web3 = new Web3((window as any).ethereum);
      } catch (error) {
        return {
          ...ret,
          sucesso: false,
          dataResult: "",
          mensagem: 'You need to allow MetaMask.'
        };
      }
    }
    let _chainId = await web3.eth.net.getId();
    if(parseInt(_chainId.toString()) != chainId) {
      return {
        ...ret,
        sucesso: false,
        dataResult: "",
        mensagem: "You need to connect on Binance Net"
      };
    }
    const publicAddress = await web3.eth.getCoinbase();
    console.log("publicAddress: ", publicAddress);
    if (!publicAddress) {
      return {
        ...ret,
        sucesso: false,
        dataResult: "",
        mensagem: 'Please activate MetaMask first'
      };
    }
    return {
      ...ret,
      sucesso: true,
      dataResult: publicAddress
    };
  },
  transferUSDT: async (to: string, value: number) => {
    let ret: BusinessResult<boolean>;
    let price: BigInt = BigInt(value) * BigInt(10000000000);
    let retCont = await ContractFactory.USDTContract.transfer(to, price);
    if (retCont.success) {
        return {
            ...ret,
            sucesso: true,
            dataResult: retCont.data,
            mensagem: "Transfer successfully"
        };
    }
    else {
        return {
            ...ret,
            sucesso: false,
            dataResult: null,
            mensagem: retCont.message
        };
    }
  }
}

export default EtherBusiness;