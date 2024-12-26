import { useState } from "react";
import IUserProvider from "../../DTO/Contexts/IUserProvider";
import UserContext from "./UserContext";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import UserInfo from "../../DTO/Domain/UserInfo";
import ProviderResult from "../../DTO/Contexts/ProviderResult";
import UserFactory from "../../Business/Factory/UserFactory";
import UserAddressInfo from "../../DTO/Domain/UserAddressInfo";
import UserAddressFactory from "../../Business/Factory/UserAddressFactory";
import { UserAddrProvideResult } from "../../DTO/Contexts/UserAddrProviderResult";

export default function UserProvider(props: any) {

    const [loading, setLoading] = useState<boolean>(false);
    const [loadingPassword, setLoadingPassword] = useState<boolean>(false);
    const [loadingUpdate, setLoadingUpdate] = useState<boolean>(false);
    const [loadingUserAddr, setLoadingUserAddr] = useState<boolean>(false);
    const [loadingUpdateAddr, setLoadingUpdateAddr] = useState<boolean>(false);

    const [userHasPassword, setUserHasPassword] = useState<boolean>(false);

    const [user, _setUser] = useState<UserInfo>(null);
    const [userAddress, setUserAddress] = useState<UserAddressInfo>(null);
    const [userAddresses, setUserAddresses] = useState<UserAddressInfo[]>([]);

    const userProviderValue: IUserProvider = {
        loading: loading,
        loadingPassword: loadingPassword,
        loadingUpdate: loadingUpdate,
        loadingUserAddr: loadingUserAddr,
        loadingUpdateAddr: loadingUpdateAddr,
        userHasPassword: userHasPassword,
        user: user,
        userAddress: userAddress,
        userAddresses: userAddresses,
        setUser: (user: UserInfo) => {
            _setUser(user);
        },
        getMe: async () => {
            let ret: Promise<ProviderResult>;
            setLoading(true);
            try {
                let brt = await UserFactory.UserBusiness.getMe();
                if (brt.sucesso) {
                    setLoading(false);
                    _setUser(brt.dataResult);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "User load"
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
        getUserByAddress: async (chain: ChainEnum, address: string) => {
            let ret: Promise<ProviderResult>;
            setLoading(true);
            try {
                let brt = await UserFactory.UserBusiness.getUserByAddress(chain, address);
                if (brt.sucesso) {
                    setLoading(false);
                    _setUser(brt.dataResult);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "User load"
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
        getUserByEmail: async (email: string) => {
            let ret: Promise<ProviderResult>;
            setLoading(true);
            try {
                let brt = await UserFactory.UserBusiness.getUserByEmail(email);
                if (brt.sucesso) {
                    setLoading(false);
                    _setUser(brt.dataResult);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "User load"
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
        insert: async (user: UserInfo) => {
            let ret: Promise<ProviderResult>;
            setLoadingUpdate(true);
            try {
                let brt = await UserFactory.UserBusiness.insert(user);
                if (brt.sucesso) {
                    setLoadingUpdate(false);
                    _setUser(brt.dataResult);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "User inseted"
                    };
                }
                else {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingUpdate(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        update: async (user: UserInfo) => {
            let ret: Promise<ProviderResult>;
            setLoadingUpdate(true);
            try {
                let brt = await UserFactory.UserBusiness.update(user);
                if (brt.sucesso) {
                    setLoadingUpdate(false);
                    _setUser(brt.dataResult);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "User updated"
                    };
                }
                else {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingUpdate(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        loginWithEmail: async (email: string, password: string) => {
            let ret: Promise<ProviderResult>;
            setLoading(true);
            try {
                let brt = await UserFactory.UserBusiness.update(user);
                if (brt.sucesso) {
                    setLoading(false);
                    _setUser(brt.dataResult);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "User updated"
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
        hasPassword: async () => {
            let ret: Promise<ProviderResult>;
            setLoadingPassword(true);
            setUserHasPassword(false);
            try {
                let brt = await UserFactory.UserBusiness.hasPassword();
                if (brt.sucesso) {
                    setUserHasPassword(true);
                    setLoadingPassword(false);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "Password changed"
                    };
                }
                else {
                    setLoadingPassword(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingPassword(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        changePassword: async (oldPassword: string, newPassword: string) => {
            let ret: Promise<ProviderResult>;
            setLoadingUpdate(true);
            try {
                let brt = await UserFactory.UserBusiness.changePassword(oldPassword, newPassword);
                console.log("changePassword: ", JSON.stringify(brt));
                if (brt.sucesso) {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: brt.mensagem
                    };
                }
                else {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingUpdate(false);
                console.log("Error change password: ", err);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        sendRecoveryEmail: async (email: string) => {
            let ret: Promise<ProviderResult>;
            setLoadingUpdate(true);
            try {
                let brt = await UserFactory.UserBusiness.sendRecoveryEmail(email);
                if (brt.sucesso) {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "Recovery email sent successfully"
                    };
                }
                else {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingUpdate(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        changePasswordUsingHash: async (recoveryHash: string, newPassword: string) => {
            let ret: Promise<ProviderResult>;
            setLoadingUpdate(true);
            try {
                let brt = await UserFactory.UserBusiness.changePasswordUsingHash(recoveryHash, newPassword);
                if (brt.sucesso) {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "Recovery email sent successfully"
                    };
                }
                else {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingUpdate(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        listAddressByUser: async () => {
            let ret: Promise<ProviderResult>;
            setLoadingUserAddr(true);
            try {
                let brt = await UserAddressFactory.UserAddressBusiness.listAddressByUser();
                if (brt.sucesso) {
                    setUserAddresses(brt.dataResult);
                    setLoadingUserAddr(false);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "Recovery email sent successfully"
                    };
                }
                else {
                    setLoadingUserAddr(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingUserAddr(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        addOrChangeAddress: async (userId: number, chainId: number, address: string) => {
            let ret: Promise<ProviderResult>;
            setLoadingUpdateAddr(true);
            try {
                let brt = await UserAddressFactory.UserAddressBusiness.addOrChangeAddress(userId, chainId, address);
                if (brt.sucesso) {
                    setUserAddresses([]);
                    setLoadingUpdateAddr(false);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "Recovery email sent successfully"
                    };
                }
                else {
                    setLoadingUpdateAddr(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingUpdateAddr(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        removeAddress: async (chainId: number) => {
            let ret: Promise<ProviderResult>;
            setLoadingUpdateAddr(true);
            try {
                let brt = await UserAddressFactory.UserAddressBusiness.removeAddress(chainId);
                if (brt.sucesso) {
                    setUserAddresses([]);
                    setLoadingUpdateAddr(false);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "Recovery email sent successfully"
                    };
                }
                else {
                    setLoadingUpdateAddr(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingUpdateAddr(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        },
        getAddressByChain: async (chainId: number) => {
            let ret: Promise<UserAddrProvideResult>;
            setLoadingUserAddr(true);
            try {
                let brt = await UserAddressFactory.UserAddressBusiness.getAddressByChain(chainId);
                if (brt.sucesso) {
                    setUserAddress(brt.dataResult);
                    setLoadingUserAddr(false);
                    return {
                        ...ret,
                        UserAddress: brt.dataResult.address,
                        sucesso: true,
                        mensagemSucesso: "Get user address successfully"
                    };
                }
                else {
                    setLoadingUserAddr(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
            }
            catch (err) {
                setLoadingUserAddr(false);
                return {
                    ...ret,
                    sucesso: false,
                    mensagemErro: JSON.stringify(err)
                };
            }
        }
    }

    return (
        <UserContext.Provider value={userProviderValue}>
            {props.children}
        </UserContext.Provider>
    );
}