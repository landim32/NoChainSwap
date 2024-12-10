import { CoinEnum } from "../Enum/CoinEnum";

export default interface TxParamInfo {
  userid: number;
  sendercoin: string;
  receivercoin: string;
  senderaddress: string;
  receiveraddress: string;
  sendertxid: string;
  senderamount: number;
  receiveramount: number;
  senderfee: number;
  receiverfee: number; 
}