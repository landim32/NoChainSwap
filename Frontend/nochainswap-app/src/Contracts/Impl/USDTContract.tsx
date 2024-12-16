import Web3, { Web3EthInterface } from "web3";
import ContractResponse from "../../DTO/Contracts/ContractResponse";
import env from "react-dotenv";
import USDT_ABI from "../ABIs/USDT.json";
import IUSDTContract from "../Interfaces/IUSDTContract";
import ContractTxInfo from "../../DTO/Contracts/ContractTxInfo";

const USDTContract: IUSDTContract = {
    getContract: async () => {
        let web3: Web3 | undefined = undefined;
        if (!(window as any).ethereum) {
            throw new Error(`No MetaMask found!`);
        }


        if (!web3) {
            await (window as any).ethereum.enable();
            web3 = new Web3((window as any).ethereum);
        }
        const accounts = await web3.eth.requestAccounts();
        if (!accounts || !accounts.length) {
            throw new Error('Wallet not found/allowed!');
        }

        return new web3.eth.Contract(USDT_ABI, env.USDT_CONTRACT, { from: accounts[0] });
    },
    transfer: async (to: string, value: BigInt) => {
        let _ret: ContractResponse<ContractTxInfo>;
        let _info: ContractTxInfo;
        let _contract = await USDTContract.getContract();
        try {
            let ret = await _contract.methods.transfer(to, value).send();
            console.log("transfer: ", JSON.stringify(ret, (key, value) =>
                typeof value === 'bigint'
                    ? value.toString()
                    : value // return everything else unchanged
            ));
            _ret = {
                ..._ret,
                success: true,
                data: {
                    ..._info,
                    transactionHash: ret.transactionHash,
                    cumulativeGasUsed: ret.cumulativeGasUsed
                }
            };
        }
        catch (err: any) {
            let msg = "";
            if (err.data?.message) {
                msg = err.data?.message;
            }
            else if (err.message) {
                msg = err.message;
            }
            else {
                msg = JSON.stringify(err);
            }

            _ret = {
                ..._ret,
                success: false,
                message: msg
            };
        }
        return _ret;
    }
}

export default USDTContract;