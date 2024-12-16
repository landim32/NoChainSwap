import AuthSession from "../Domain/AuthSession";
import { ChainEnum } from "../Enum/ChainEnum";
import ProviderResult from "./ProviderResult";


interface IAuthProvider {
    chain: ChainEnum;
    loading: boolean;
    sessionInfo: AuthSession;
    //bindMetaMaskWallet: (name: string, email: string, fromReferralCode: string) => Promise<ProviderResult>;
    //checkUserRegister: () => Promise<ProviderResult>;
    setChain: (chain: ChainEnum) => void;
    setSession: (session: AuthSession) => void;
    loginWithEmail: (email: string, password: string) => Promise<ProviderResult>;
    loginCallback: (callback?: any) => void;
    loginEther: () => Promise<ProviderResult>;
    logout: () => ProviderResult;
    loadUserSession: () => void;
    //updateUser: (name: string, email: string) => Promise<ProviderResult>;
}

export default IAuthProvider;