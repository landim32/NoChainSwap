import BusinessResult from "../../DTO/Business/BusinessResult";
import ContractTxInfo from "../../DTO/Contracts/ContractTxInfo";

export default interface IEtherBusiness {
    checkNetwork: () => Promise<BusinessResult<boolean>>;
    logIn: (chainId: number) => Promise<BusinessResult<string>>;
    transferUSDT: (to: string, value: number) => Promise<BusinessResult<ContractTxInfo>>;
}