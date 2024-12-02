import UserAddressInfo from "../Domain/UserAddressInfo";
import StatusRequest from "./StatusRequest";

export default interface UserAddressResult extends StatusRequest {
  userAddress? : UserAddressInfo;
}