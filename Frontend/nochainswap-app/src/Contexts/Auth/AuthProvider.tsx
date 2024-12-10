import React, { useContext, useState } from 'react';
import ProviderResult from '../../DTO/Contexts/ProviderResult';
import IAuthProvider from '../../DTO/Contexts/IAuthProvider';
import AuthContext from './AuthContext';
import AuthFactory from '../../Business/Factory/AuthFactory';
import AuthSession from '../../DTO/Domain/AuthSession';
import { ChainEnum } from '../../DTO/Enum/ChainEnum';
import UserFactory from '../../Business/Factory/UserFactory';
import { SignInEnum } from '../../DTO/Enum/SignInEnum';
import StacksFactory from '../../Business/Factory/StacksFactory';
import UserInfo from '../../DTO/Domain/UserInfo';
import UserAddressFactory from '../../Business/Factory/UserAddressFactory';

export default function AuthProvider(props: any) {

  const [loading, setLoading] = useState(false);
  const [chain, _setChain] = useState<ChainEnum>(ChainEnum.NoChain);
  const [sessionInfo, _setSessionInfo] = useState<AuthSession>(null);

  const authProviderValue: IAuthProvider = {
    loading: loading,
    chain: chain,
    sessionInfo: sessionInfo,

    setChain: (chain: ChainEnum) => {
      _setChain(chain);
    },
    setSession: (session: AuthSession) => {
      console.log(JSON.stringify(session));
      _setSessionInfo(session);
      AuthFactory.AuthBusiness.setSession(session);
    },
    loginWithEmail: async (email: string, password: string) => {
      let ret: Promise<ProviderResult>;
      setLoading(true);
      try {
        let brt = await UserFactory.UserBusiness.loginWithEmail(email, password);
        if (brt.sucesso) {
          setLoading(false);
          authProviderValue.setSession({
            ...sessionInfo,
            id: brt.dataResult.id,
            hash: brt.dataResult.hash,
            name: brt.dataResult.name,
            email: brt.dataResult.email,
            loginWith: SignInEnum.Email,
            chain: chain,
            address: ""
          });
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: "User Logged"
          };
        }
        else {
          setLoading(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem
          };
        }
      }
      catch (err) {
        setLoading(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err)
        };
      }
    },
    loginCallback: (callback?: any) => {
      if (chain == ChainEnum.BitcoinAndStack) {
        StacksFactory.StacksBusiness.logIn(callback);
      }
    },
    logout: function (): ProviderResult {
      try {
        if (chain == ChainEnum.BitcoinAndStack) {
          StacksFactory.StacksBusiness.logOut();
        }
        AuthFactory.AuthBusiness.cleanSession();
        _setSessionInfo(null);
        return {
          sucesso: true,
          mensagemErro: "",
          mensagemSucesso: ""
        };
      } catch (err) {
        return {
          sucesso: false,
          mensagemErro: "Falha ao tenta executar o logout",
          mensagemSucesso: ""
        };
      }
    },
    loadUserSession: async () => {
      if (chain == ChainEnum.BitcoinAndStack) {
        let userSession = await StacksFactory.StacksBusiness.getSession();
        console.log("userSession: ", JSON.stringify(userSession));
        if (userSession.sucesso) {
          let session = AuthFactory.AuthBusiness.getSession();
          console.log("session: ", JSON.stringify(session));
          if (session) {
            authProviderValue.setSession({
              ...userSession.dataResult,
              id: session.id,
              hash: session.hash,
              name: session.name,
              email: session.email,
              loginWith: SignInEnum.WebWallet,
              chain: ChainEnum.BitcoinAndStack
            });
          }
          else {
            console.log("userSession: ", JSON.stringify(userSession.dataResult));
            let user = await UserFactory.UserBusiness.getUserByAddress(ChainEnum.BitcoinAndStack, userSession.dataResult.address);
            console.log("user: ", JSON.stringify(user));
            if (user.sucesso) {
              authProviderValue.setSession({
                ...userSession.dataResult,
                id: user.dataResult.id,
                hash: user.dataResult.hash,
                name: user.dataResult.name,
                email: user.dataResult.email,
                loginWith: SignInEnum.WebWallet,
                chain: ChainEnum.BitcoinAndStack
              });
            }
            else {
              let emptyUser: UserInfo;
              let newUser = await UserFactory.UserBusiness.insert({
                ...emptyUser,
                name: userSession.dataResult.name
              });
              if (newUser.sucesso) {
                let ret = await UserAddressFactory.UserAddressBusiness.addOrChangeAddress(newUser.dataResult.id, chain, userSession.dataResult.address);
                if (!ret.sucesso) {
                  throw new Error(ret.mensagem);
                }
                authProviderValue.setSession({
                  ...userSession.dataResult,
                  id: newUser.dataResult.id,
                  hash: newUser.dataResult.hash,
                  name: newUser.dataResult.name,
                  email: newUser.dataResult.email,
                  loginWith: SignInEnum.WebWallet,
                  chain: ChainEnum.BitcoinAndStack,
                  address: userSession.dataResult.address
                });
              }
              else {
                throw new Error(newUser.mensagem);
              }
            }
          }
        }
      }
      else {
        let session = await AuthFactory.AuthBusiness.getSession();
        if (session) {
          authProviderValue.setSession(session);
        }
      }
    }
  };

  return (
    <AuthContext.Provider value={authProviderValue}>
      {props.children}
    </AuthContext.Provider>
  );
}
