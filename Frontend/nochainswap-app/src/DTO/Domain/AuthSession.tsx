import { ChainEnum } from "../Enum/ChainEnum";
import { SignInEnum } from "../Enum/SignInEnum";

export default interface AuthSession {
  id: number;
  email: string;
  name: string;
  hash: string;
  loginWith: SignInEnum;
  chain: ChainEnum;
  address: string;
}