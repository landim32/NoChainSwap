import './App.css';
import { Routes, Route, Outlet, Link } from "react-router-dom";
import Menu from "./Components/Menu";
import SwapForm from './Pages/SwapForm';
import ContextBuilder from './Contexts/Utils/ContextBuilder';
import AuthProvider from './Contexts/Auth/AuthProvider';
import SwapProvider from './Contexts/Swap/SwapProvider';
import ListTxPage from './Pages/ListTxPage';
import TxProvider from './Contexts/Transaction/TxProvider';
import UserPage from './Pages/UserPage';
import PasswordPage from './Pages/PasswordPage';
import LoginPage from './Pages/LoginPage';
import RecoveryPage from './Pages/RecoveryPage';
import UserProvider from './Contexts/User/UserProvider';
import TxPage from './Pages/TxPage';

function Error404() {
  return (
    <div>
      <h2>Error 404</h2>
    </div>
  );
}

function Layout() {
  return (
    <div>
      <Menu />
      <Outlet />
    </div>
  );
}

function App() {
  const ContextContainer = ContextBuilder([AuthProvider, UserProvider, SwapProvider, TxProvider]);

  return (
    <ContextContainer>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<SwapForm />} />
          <Route path="my-swaps">
            <Route index element={<ListTxPage OnlyMyTx={true} />} />
            <Route path=":txid" element={<ListTxPage OnlyMyTx={true} />} />
          </Route>
          <Route path="all-swaps">
            <Route index element={<ListTxPage OnlyMyTx={false} />} />
            <Route path=":txid" element={<ListTxPage OnlyMyTx={false} />} />
          </Route>
          <Route path="tx/:txId" element={<TxPage />} />
          <Route path="login" element={<LoginPage />} />
          <Route path="edit-account" element={<UserPage />} />
          <Route path="new-account" element={<UserPage />} />
          <Route path="recovery-password" element={<RecoveryPage />} />
          <Route path="change-password" element={<PasswordPage />} />
          <Route path="*" element={<Error404 />} />
        </Route>
      </Routes>
    </ContextContainer>
  );
}

export default App;
