import UserAddressInfo from "../Domain/UserAddressInfo";
import UserInfo from "../Domain/UserInfo";
import { ChainEnum } from "../Enum/ChainEnum";
import ProviderResult from "./ProviderResult";
import { UserAddrProvideResult } from "./UserAddrProviderResult";


interface IUserProvider {
    loading: boolean;
    loadingPassword: boolean;
    loadingUpdate: boolean;
    loadingUserAddr: boolean;
    loadingUpdateAddr: boolean;
    userHasPassword: boolean;
    user: UserInfo;
    userAddress: UserAddressInfo;
    userAddresses: UserAddressInfo[];

    setUser: (user: UserInfo) => void;
    getMe: () => Promise<ProviderResult>;
    getUserByAddress: (chain: ChainEnum, address: string) => Promise<ProviderResult>;
    getUserByEmail: (email: string) => Promise<ProviderResult>;
    insert: (user: UserInfo) => Promise<ProviderResult>;
    update: (user: UserInfo) => Promise<ProviderResult>;
    loginWithEmail: (email: string, password: string) => Promise<ProviderResult>;

    hasPassword: () => Promise<ProviderResult>;
    changePassword: (oldPassword: string, newPassword: string) => Promise<ProviderResult>;
    sendRecoveryEmail: (email: string) => Promise<ProviderResult>;
    changePasswordUsingHash: (recoveryHash: string, newPassword: string) => Promise<ProviderResult>; 

    listAddressByUser: () => Promise<ProviderResult>;
    addOrChangeAddress: (userId: number, chainId: number, address: string) => Promise<ProviderResult>;
    removeAddress: (chainId: number) => Promise<ProviderResult>;
    getAddressByChain: (chainId: number) => Promise<UserAddrProvideResult>;
}

export default IUserProvider;