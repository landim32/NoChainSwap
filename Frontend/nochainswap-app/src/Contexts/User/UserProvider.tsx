import { useState } from "react";
import IUserProvider from "../../DTO/Contexts/IUserProvider";
import UserContext from "./UserContext";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import UserInfo from "../../DTO/Domain/UserInfo";
import ProviderResult from "../../DTO/Contexts/ProviderResult";
import UserFactory from "../../Business/Factory/UserFactory";
import UserAddressInfo from "../../DTO/Domain/UserAddressInfo";
import UserAddressFactory from "../../Business/Factory/UserAddressFactory";

export default function UserProvider(props: any) {

    const [loading, setLoading] = useState<boolean>(false);
    const [loadingUpdate, setLoadingUpdate] = useState<boolean>(false);
    const [loadingUserAddr, setLoadingUserAddr] = useState<boolean>(false);
    const [loadingUpdateAddr, setLoadingUpdateAddr] = useState<boolean>(false);
    const [user, _setUser] = useState<UserInfo>(null);
    const [userAddresses, setUserAddresses] = useState<UserAddressInfo[]>([]);

    const userProviderValue: IUserProvider = {
        loading: loading,
        loadingUpdate: loadingUpdate,
        loadingUserAddr: loadingUserAddr,
        loadingUpdateAddr: loadingUpdateAddr,
        user: user,
        userAddresses: userAddresses,
        setUser: (user: UserInfo) => {
            _setUser(user);
        },
        getUserById: async (userId: number) => {
            let ret: Promise<ProviderResult>;
            setLoading(true);
            try {
                let brt = await UserFactory.UserBusiness.getUserById(userId);
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
        changePassword: async (userId: number, oldPassword: string, newPassword: string) => {
            let ret: Promise<ProviderResult>;
            setLoadingUpdate(true);
            try {
                let brt = await UserFactory.UserBusiness.changePassword(userId, oldPassword, newPassword);
                if (brt.sucesso) {
                    setLoadingUpdate(false);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "Password changed"
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
        listAddressByUser: async (userId: number) => {
            let ret: Promise<ProviderResult>;
            setLoadingUserAddr(true);
            try {
                let brt = await UserAddressFactory.UserAddressBusiness.listAddressByUser(userId);
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
        removeAddress: async (userId: number, chainId: number) => {
            let ret: Promise<ProviderResult>;
            setLoadingUpdateAddr(true);
            try {
                let brt = await UserAddressFactory.UserAddressBusiness.removeAddress(userId, chainId);
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
        }
    }

    return (
        <UserContext.Provider value={userProviderValue}>
            {props.children}
        </UserContext.Provider>
    );
}