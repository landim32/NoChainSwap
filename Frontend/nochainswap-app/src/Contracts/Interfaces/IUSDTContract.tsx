import ContractResponse from "../../DTO/Contracts/ContractResponse";
import ContractTxInfo from "../../DTO/Contracts/ContractTxInfo";

export default interface IUSDTContract {
    getContract: () => Promise<any>;
    transfer: (to: string, value: BigInt) => Promise<ContractResponse<ContractTxInfo>>;
}