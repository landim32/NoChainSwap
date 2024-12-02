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
import { faBitcoinSign, faBoltLightning, faBrazilianRealSign, faCircleUser, faClose, faCoins, faEthernet, faLock, faPencil, faSignInAlt, faUserCircle } from '@fortawesome/free-solid-svg-icons';
import ErrorToast from './ErrorToast';

export default function Menu() {

  const [show, setShow] = useState(true);
  const [showError, setShowError] = useState<boolean>(false);
  const [messageError, setMessageError] = useState<string>("");

  const throwError = (message: string) => {
    setMessageError(message);
    setShowError(true);
  };

  let navigate = useNavigate();

  const authContext = useContext(AuthContext);
  useEffect(() => {
    authContext.loadUserSession();
  }, []);
  return (
    <>
      <ErrorToast
        showError={showError}
        messageError={messageError}
        onClose={() => setShowError(false)}
      ></ErrorToast>
      <Navbar expand="lg" className="navbar-dark bg-dark mb-3">
        <Container>
          <Navbar.Brand href="/">NoChain Swap</Navbar.Brand>
          <Navbar.Toggle aria-controls="basic-navbar-nav" />
          <Navbar.Collapse id="basic-navbar-nav">
            {authContext.sessionInfo &&
              <Nav className="me-auto">
                <Link className='nav-link' to="/">Swap</Link>
                <Link className='nav-link' to="/my-swaps">My Swaps</Link>
                <Link className='nav-link' to="/all-swaps">All Swaps</Link>
              </Nav>
            }
          </Navbar.Collapse>
          <Navbar.Collapse>
            <Nav className="ms-auto justify-content-end">
              <Nav.Link href='https://metamask.io/download/' target='_blank'>Install Metamask</Nav.Link>
              <Link className='nav-link' to="/login"><FontAwesomeIcon icon={faUserCircle} fixedWidth /> Login</Link>
              <Link className='nav-link' to="/new-account"><FontAwesomeIcon icon={faSignInAlt} fixedWidth /> Sign In</Link>
              <NavDropdown title={
                <>
                  <FontAwesomeIcon icon={faCoins} />&nbsp;
                  <span>No Chain</span>
                </>
              } id="basic-nav-dropdown">
                <NavDropdown.Item onClick={async () => {
                  navigate("/edit-account");
                }}><FontAwesomeIcon icon={faBitcoinSign} fixedWidth /> Stacks Chain</NavDropdown.Item>
                <NavDropdown.Item onClick={async () => {
                  navigate("/edit-account");
                }}><FontAwesomeIcon icon={faBoltLightning} fixedWidth /> BNB Chain</NavDropdown.Item>
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
                  <Nav.Item>
                    <Button variant="danger" onClick={async () => {
                      let handleCallback = () => {
                        authContext.loadUserSession();
                      }
                      authContext.login(handleCallback);
                    }}><FontAwesomeIcon icon={faBoltLightning} fixedWidth /> Connect</Button>
                  </Nav.Item>
              }
            </Nav>
          </Navbar.Collapse>
        </Container>
      </Navbar>
      <Container>
        <Alert key="danger" variant="danger" onClose={() => setShow(false)} dismissible>
          <FontAwesomeIcon icon={faWarning} /> This app is using the <strong>TestNet Network</strong>. Coins have no value here!
        </Alert>
      </Container>
    </>
  );
}
