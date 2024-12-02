import React, {useContext, useState} from 'react';
import ProviderResult from '../../DTO/Contexts/ProviderResult'; 
import IAuthProvider from '../../DTO/Contexts/IAuthProvider';
import AuthContext from './AuthContext';
import AuthFactory from '../../Business/Factory/AuthFactory';
import { AuthSession } from '../../DTO/Domain/AuthSession';

export default function AuthProvider(props : any) {

  const [loading, setLoading] = useState(false);
  const [sessionInfo, setSessionInfo] = useState<AuthSession>(null);

  const authProviderValue: IAuthProvider = {
    loading: loading,
    login: (callback?: any) => {;
      AuthFactory.AuthBusiness.logIn(callback);
    },
    logout: function (): ProviderResult {
      try {
        AuthFactory.AuthBusiness.logOut();
        setSessionInfo(null);
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
    /*
    checkUserRegister: async () => {
      setLoading(true);
      let bindResult = await AuthFactory.AuthBusiness.checkUserRegister();
      setLoading(false);
      return {
        sucesso: bindResult.dataResult,
        mensagemErro: bindResult.mensagem,
        mensagemSucesso: bindResult.mensagem
      };
    },
    */
    loadUserSession: async () => {
      let session = await AuthFactory.AuthBusiness.getSession();
      if (session.sucesso) {
        //console.log(session.dataResult);
        setSessionInfo(session.dataResult);
      }
    },
    sessionInfo: sessionInfo
  };

  return (
    <AuthContext.Provider value={authProviderValue}>
      {props.children}
    </AuthContext.Provider>
  );
}
