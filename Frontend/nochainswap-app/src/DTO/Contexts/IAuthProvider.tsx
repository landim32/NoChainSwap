import { AuthSession } from "../Domain/AuthSession";
import ProviderResult from "./ProviderResult";


interface IAuthProvider {
    //bindMetaMaskWallet: (name: string, email: string, fromReferralCode: string) => Promise<ProviderResult>;
    //checkUserRegister: () => Promise<ProviderResult>;
    login: (callback?: any) => void;
    logout: () => ProviderResult;
    loadUserSession: () => void;
    //updateUser: (name: string, email: string) => Promise<ProviderResult>;
    loading: boolean;
    sessionInfo: AuthSession;
}

export default IAuthProvider;