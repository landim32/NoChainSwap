import React, { useContext, useEffect, useState } from 'react';
import Container from 'react-bootstrap/Container';
import Button from 'react-bootstrap/esm/Button';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import { Link, useNavigate } from 'react-router-dom';
import AuthContext from '../Contexts/Auth/AuthContext';
import NavDropdown from 'react-bootstrap/NavDropdown';
import Alert from 'react-bootstrap/Alert';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faWarning } from '@fortawesome/free-solid-svg-icons/faWarning'
import { faBitcoinSign, faBoltLightning, faBrazilianRealSign, faCancel, faCircleUser, faClose, faCoins, faEthernet, faLock, faPencil, faSignInAlt, faUserCircle } from '@fortawesome/free-solid-svg-icons';
import MessageToast from './MessageToast';
import { ChainEnum } from '../DTO/Enum/ChainEnum';
import { MessageToastEnum } from '../DTO/Enum/MessageToastEnum';

export default function Menu() {

  const [showAlert, setShowAlert] = useState<boolean>(true);

  const [showMessage, setShowMessage] = useState<boolean>(false);
  const [messageText, setMessageText] = useState<string>("");

  const throwError = (message: string) => {
    setMessageText(message);
    setShowMessage(true);
  };

  const showChainText = (chain: ChainEnum) => {
    switch (chain) {
      case ChainEnum.NoChain:
        return (
          <>
            <FontAwesomeIcon icon={faCancel} fixedWidth />&nbsp;No Chain
          </>
        );
        break;
      case ChainEnum.BitcoinAndStack:
        return (
          <>
            <FontAwesomeIcon icon={faBitcoinSign} fixedWidth />&nbsp;Stacks & Bitcoin
          </>
        );
        break;
      case ChainEnum.BNBChain:
        return (
          <>
            <FontAwesomeIcon icon={faBoltLightning} fixedWidth />&nbsp;BNB Chain
          </>
        );
        break;
    }
  };

  let navigate = useNavigate();

  const authContext = useContext(AuthContext);

  useEffect(() => {
    authContext.loadUserSession();
  }, []);
  return (
    <>
      <MessageToast
        dialog={MessageToastEnum.Error}
        showMessage={showMessage}
        messageText={messageText}
        onClose={() => setShowMessage(false)}
      ></MessageToast>
      <Navbar expand="lg" className="navbar-dark bg-dark mb-3">
        <Container>
          <Navbar.Brand href="/">NoChain Swap</Navbar.Brand>
          <Navbar.Toggle aria-controls="basic-navbar-nav" />
          <Navbar.Collapse id="basic-navbar-nav">
            <Nav className="me-auto">
              <Link className='nav-link' to="/">Swap</Link>
              {authContext.sessionInfo &&
                <>
                  <Link className='nav-link' to="/my-swaps">My Swaps</Link>
                  {authContext.sessionInfo?.isAdmin &&
                  <Link className='nav-link' to="/all-swaps">All Swaps</Link>
                  }
                </>
              }
            </Nav>
          </Navbar.Collapse>
          <Navbar.Collapse>
            <Nav className="ms-auto justify-content-end">
              {authContext.chain == ChainEnum.BNBChain &&
                <Nav.Link href='https://metamask.io/download/' target='_blank'>Install Metamask</Nav.Link>
              }
              {authContext.chain == ChainEnum.BitcoinAndStack &&
                <Nav.Link href='https://leather.io/install-extension' target='_blank'>Install Leather</Nav.Link>
              }
              {
                //<Link className='nav-link' to="/login"><FontAwesomeIcon icon={faSignInAlt} fixedWidth /> Sign In</Link>
              }
              <NavDropdown title={showChainText(authContext.chain)} id="basic-nav-dropdown">
                <NavDropdown.ItemText className='small'>Select the chain you will connect to</NavDropdown.ItemText>
                <NavDropdown.Divider />
                <NavDropdown.Item onClick={async () => {
                  authContext.setChain(ChainEnum.NoChain);
                }}>{showChainText(ChainEnum.NoChain)}</NavDropdown.Item>
                <NavDropdown.Item onClick={async () => {
                  authContext.setChain(ChainEnum.BitcoinAndStack);
                }}>{showChainText(ChainEnum.BitcoinAndStack)}</NavDropdown.Item>
                <NavDropdown.Item onClick={async () => {
                  authContext.setChain(ChainEnum.BNBChain);
                }}>{showChainText(ChainEnum.BNBChain)}</NavDropdown.Item>
              </NavDropdown>
              {
                authContext.sessionInfo ?
                  <NavDropdown title={
                    <>
                      <FontAwesomeIcon icon={faCircleUser} />&nbsp;
                      <span>{authContext.sessionInfo.name}</span>
                    </>
                  } id="basic-nav-dropdown">
                    <NavDropdown.Item onClick={async () => {
                      navigate("/edit-account");
                    }}><FontAwesomeIcon icon={faPencil} fixedWidth /> Edit Account</NavDropdown.Item>
                    <NavDropdown.Item onClick={async () => {
                      navigate("/change-password");
                    }}><FontAwesomeIcon icon={faLock} fixedWidth /> Change Password</NavDropdown.Item>
                    <NavDropdown.Divider />
                    <NavDropdown.Item onClick={async () => {
                      let ret = authContext.logout();
                      if (!ret.sucesso) {
                        throwError(ret.mensagemErro);
                      }
                      navigate(0);
                    }}><FontAwesomeIcon icon={faClose} fixedWidth /> Logout</NavDropdown.Item>
                  </NavDropdown>
                  :
                  <>
                    <Nav.Item>
                      <Button variant="danger" onClick={async () => {
                        if (authContext.chain == ChainEnum.NoChain) {
                          navigate("/login");
                        }
                        else if (authContext.chain == ChainEnum.BNBChain) {
                          let ret = await authContext.loginEther();
                          if (ret.sucesso) {
                            authContext.loadUserSession();
                          }
                          else {
                            throwError(ret.mensagemErro);
                          }
                        }
                        else {
                          const handleCallback = () => {
                            authContext.loadUserSession();
                          }
                          authContext.loginCallback(handleCallback);
                        }
                      }}>
                        {authContext.chain == ChainEnum.NoChain ?
                          <>
                            <FontAwesomeIcon icon={faSignInAlt} fixedWidth /> Sign In
                          </>
                          :
                          <>
                            <FontAwesomeIcon icon={faBoltLightning} fixedWidth /> Connect
                          </>
                        }
                      </Button>
                    </Nav.Item>
                  </>
              }
            </Nav>
          </Navbar.Collapse>
        </Container>
      </Navbar>
      {showAlert &&
      <Container>
        <Alert key="danger" variant="danger" onClose={() => setShowAlert(false)} dismissible>
          <FontAwesomeIcon icon={faWarning} /> This app is using the <strong>TestNet Network</strong>. Coins have no value here!
        </Alert>
      </Container>
      }
    </>
  );
}
