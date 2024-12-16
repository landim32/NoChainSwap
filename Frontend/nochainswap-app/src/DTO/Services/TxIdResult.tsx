import TxInfo from "../Domain/TxInfo";
import StatusRequest from "./StatusRequest";

export interface TxIdResult extends StatusRequest {
  hash: string;
}