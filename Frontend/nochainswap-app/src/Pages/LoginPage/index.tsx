import { useContext, useState } from "react";
import Col from "react-bootstrap/esm/Col";
import Container from "react-bootstrap/esm/Container";
import Row from "react-bootstrap/esm/Row";
import Form from 'react-bootstrap/Form';
import InputGroup from 'react-bootstrap/InputGroup';
import AuthContext from "../../Contexts/Auth/AuthContext";
import Button from "react-bootstrap/esm/Button";
import Card from 'react-bootstrap/Card';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBitcoinSign, faClose, faEnvelope, faLock, faMailBulk, faSave, faSign, faSignIn, faSignInAlt, faTrash, faUser, faUserAlt } from '@fortawesome/free-solid-svg-icons';
import Table from "react-bootstrap/esm/Table";
import { Link, useNavigate } from "react-router-dom";
import UserContext from "../../Contexts/User/UserContext";
import MessageToast from "../../Components/MessageToast";
import { MessageToastEnum } from "../../DTO/Enum/MessageToastEnum";

export default function LoginPage() {

    const [email, setEmail] = useState<string>("");
    const [password, setPassword] = useState<string>("");

    const [showMessage, setShowMessage] = useState<boolean>(false);
    const [messageText, setMessageText] = useState<string>("");
  
    const throwError = (message: string) => {
      setMessageText(message);
      setShowMessage(true);
    };

    const authContext = useContext(AuthContext);

    let navigate = useNavigate();

    return (
        <>
              <MessageToast
        dialog={MessageToastEnum.Error}
        showMessage={showMessage}
        messageText={messageText}
        onClose={() => setShowMessage(false)}
      ></MessageToast>
        <Container>
            <Row>
                <Col md="6" className='offset-md-3'>
                    <Card>
                        <Card.Header>
                            <h3 className="text-center">Login</h3>
                        </Card.Header>
                        <Card.Body>
                            <Form>
                                <div className="text-center mb-3">
                                    Log in with your email and password. You can also log in with your digital wallet.
                                </div>
                                <Form.Group as={Row} className="mb-3">
                                    <Form.Label column sm="3">Email:</Form.Label>
                                    <Col sm="9">
                                        <InputGroup>
                                            <InputGroup.Text><FontAwesomeIcon icon={faUser} fixedWidth /></InputGroup.Text>
                                            <Form.Control type="email" size="lg" placeholder="Your email" value={email} onChange={(e) => {
                                                setEmail(e.target.value);
                                            }} />
                                        </InputGroup>
                                    </Col>
                                </Form.Group>
                                <Form.Group as={Row} className="mb-3">
                                    <Form.Label column sm="3">Password:</Form.Label>
                                    <Col sm="9">
                                        <InputGroup>
                                            <InputGroup.Text><FontAwesomeIcon icon={faLock} fixedWidth /></InputGroup.Text>
                                            <Form.Control type="password" size="lg" placeholder="Your password" value={password} onChange={(e) => {
                                                setPassword(e.target.value);
                                            }} />
                                        </InputGroup>
                                    </Col>
                                </Form.Group>
                                <Form.Group as={Row} className="mb-3">
                                    <Col sm="9" className="offset-sm-3">
                                        <Form.Check type="checkbox" label="Remember password?" />
                                    </Col>
                                </Form.Group>
                                <div className="d-grid gap-2 d-md-flex justify-content-md-end">
                                    <Button variant="secondary" onClick={() => {
                                        navigate("/recovery-password");
                                    }}><FontAwesomeIcon icon={faEnvelope} fixedWidth /> Recovery Password?</Button>
                                    <Button variant="danger" onClick={() => {
                                        navigate("/new-account");
                                    }}><FontAwesomeIcon icon={faUserAlt} fixedWidth /> Create Account</Button>
                                    <Button variant="success" disabled={authContext.loading} onClick={async (e) => {
                                        e.preventDefault();
                                        if (!email) {
                                            throwError("Email is empty");
                                            return;
                                        }
                                        if (!password) {
                                            throwError("Password is empty");
                                            return;
                                        }
                                        let ret = await authContext.loginWithEmail(email, password);  
                                        if (ret.sucesso) {
                                            navigate("/");
                                        }  
                                        else {
                                            throwError(ret.mensagemErro);
                                        }
                                    }}>
                                        <FontAwesomeIcon icon={faSignInAlt} fixedWidth /> {authContext.loading ? "Loading..." : "Login"}
                                    </Button>
                                </div>
                            </Form>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
        </>
    );
}