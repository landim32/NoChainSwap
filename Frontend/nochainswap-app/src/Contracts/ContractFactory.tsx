import USDTContract from "./Impl/USDTContract";
import IUSDTContract from "./Interfaces/IUSDTContract";

const usdtContractImpl : IUSDTContract = USDTContract;

const ContractFactory = {
  USDTContract: usdtContractImpl
};

export default ContractFactory;