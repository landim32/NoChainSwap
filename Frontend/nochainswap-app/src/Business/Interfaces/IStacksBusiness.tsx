import BusinessResult from "../../DTO/Business/BusinessResult";
import AuthSession from "../../DTO/Domain/AuthSession";

export default interface IStacksBusiness {
  logIn: (callback?: any) => void;
  logOut: () => void;
  getSession: () => Promise<BusinessResult<AuthSession>>;
}