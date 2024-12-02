import UserAddressInfo from "../Domain/UserAddressInfo";
import UserInfo from "../Domain/UserInfo";
import { ChainEnum } from "../Enum/ChainEnum";
import ProviderResult from "./ProviderResult";


interface IUserProvider {
    loading: boolean;
    loadingUpdate: boolean;
    loadingUserAddr: boolean;
    loadingUpdateAddr: boolean;
    user: UserInfo;
    userAddresses: UserAddressInfo[]

    setUser: (user: UserInfo) => void;
    getUserById: (userId: number) => Promise<ProviderResult>;
    getUserByAddress: (chain: ChainEnum, address: string) => Promise<ProviderResult>;
    getUserByEmail: (email: string) => Promise<ProviderResult>;
    insert: (user: UserInfo) => Promise<ProviderResult>;
    update: (user: UserInfo) => Promise<ProviderResult>;
    loginWithEmail: (email: string, password: string) => Promise<ProviderResult>;
    changePassword: (userId: number, oldPassword: string, newPassword: string) => Promise<ProviderResult>;
    sendRecoveryEmail: (email: string) => Promise<ProviderResult>;
    changePasswordUsingHash: (recoveryHash: string, newPassword: string) => Promise<ProviderResult>; 

    listAddressByUser: (userId: number) => Promise<ProviderResult>;
    addOrChangeAddress: (userId: number, chainId: number, address: string) => Promise<ProviderResult>;
    removeAddress: (userId: number, chainId: number) => Promise<ProviderResult>;
}

export default IUserProvider;