import { ChainEnum } from "../Enum/ChainEnum";
import { SignInEnum } from "../Enum/SignInEnum";

export default interface AuthSession {
  id: number;
  email: string;
  name: string;
  hash: string;
  token: string;
  isAdmin: boolean;
  loginWith: SignInEnum;
  chain: ChainEnum;
  address: string;
}