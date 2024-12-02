import UserAddressInfo from "../Domain/UserAddressInfo";
import StatusRequest from "./StatusRequest";

export default interface UserAddressListResult extends StatusRequest {
  userAddresses? : UserAddressInfo[];
}