import { useContext, useEffect, useState } from "react";
import Col from "react-bootstrap/esm/Col";
import Container from "react-bootstrap/esm/Container";
import Row from "react-bootstrap/esm/Row";
import Form from 'react-bootstrap/Form';
import AuthContext from "../../Contexts/Auth/AuthContext";
import Button from "react-bootstrap/esm/Button";
import Card from 'react-bootstrap/Card';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import InputGroup from 'react-bootstrap/InputGroup';
import { faBitcoinSign, faClose, faLock, faMailBulk, faSave, faTrash, faUser, faUserEdit, faWarning } from '@fortawesome/free-solid-svg-icons';
import Table from "react-bootstrap/esm/Table";
import { Link, useNavigate } from "react-router-dom";
import MessageToast from "../../Components/MessageToast";
import { MessageToastEnum } from "../../DTO/Enum/MessageToastEnum";
import Alert from 'react-bootstrap/Alert';
import UserContext from "../../Contexts/User/UserContext";

export default function PasswordPage() {

    const authContext = useContext(AuthContext);
    const userContext = useContext(UserContext);

    const [showAlert, setShowAlert] = useState<boolean>(true);

    const [dialog, setDialog] = useState<MessageToastEnum>(MessageToastEnum.Error);
    const [showMessage, setShowMessage] = useState<boolean>(false);
    const [messageText, setMessageText] = useState<string>("");

    const [oldPassword, setOldPassword] = useState<string>("");
    const [newPassword, setNewPassword] = useState<string>("");
    const [confirmPassword, setConfirmPassword] = useState<string>("");

    const throwError = (message: string) => {
        setDialog(MessageToastEnum.Error)
        setMessageText(message);
        setShowMessage(true);
    };
    const showSuccessMessage = (message: string) => {
        setDialog(MessageToastEnum.Success)
        setMessageText(message);
        setShowMessage(true);
    };

    let navigate = useNavigate();

    useEffect(() => {
        if (authContext.sessionInfo && authContext.sessionInfo?.id > 0) {
            userContext.getUserById(authContext.sessionInfo.id).then((ret) => {
                if (!ret.sucesso) {
                    throwError(ret.mensagemErro);
                    return;
                }
                userContext.hasPassword(authContext.sessionInfo.id);
            });
        }
        else {
            throwError("User not found");
        }
    }, []);

    return (
        <>
            <MessageToast
                dialog={dialog}
                showMessage={showMessage}
                messageText={messageText}
                onClose={() => setShowMessage(false)}
            ></MessageToast>
            <Container>
                <Row>
                    <Col md="6" className='offset-md-3'>
                        <Card>
                            <Card.Header>
                                <h3 className="text-center">Change Password</h3>
                            </Card.Header>
                            <Card.Body>
                                <Form>
                                    <div className="text-center mb-3">
                                        A password is not required, it is only used to log in via email. You can log in via your digital wallet.
                                    </div>
                                    <Form.Group as={Row} className="mb-3">
                                        <Form.Label column sm="3">Email:</Form.Label>
                                        <Col sm="9">
                                            <InputGroup>
                                                <InputGroup.Text><FontAwesomeIcon icon={faUser} fixedWidth /></InputGroup.Text>
                                                <Form.Control type="email" readOnly={true} disabled={true} size="lg" value={userContext.user?.email} />
                                            </InputGroup>
                                        </Col>
                                    </Form.Group>
                                    {userContext.userHasPassword &&
                                        <Form.Group as={Row} className="mb-3">
                                            <Form.Label column sm="3">Old Password:</Form.Label>
                                            <Col sm="9">
                                                <InputGroup>
                                                    <InputGroup.Text><FontAwesomeIcon icon={faLock} fixedWidth /></InputGroup.Text>
                                                    <Form.Control type="password" size="lg" placeholder="Your old password" value={oldPassword} onChange={(e) => {
                                                        setOldPassword(e.target.value);
                                                    }} />
                                                </InputGroup>
                                            </Col>
                                        </Form.Group>
                                    }
                                    <Form.Group as={Row} className="mb-3">
                                        <Form.Label column sm="3">New Password:</Form.Label>
                                        <Col sm="9">
                                            <InputGroup>
                                                <InputGroup.Text><FontAwesomeIcon icon={faLock} fixedWidth /></InputGroup.Text>
                                                <Form.Control type="password" size="lg" placeholder="Your new password" value={newPassword} onChange={(e) => {
                                                    setNewPassword(e.target.value);
                                                }} />
                                            </InputGroup>
                                        </Col>
                                    </Form.Group>
                                    <Form.Group as={Row} className="mb-4">
                                        <Form.Label column sm="3">Confirm Password:</Form.Label>
                                        <Col sm="9">
                                            <InputGroup>
                                                <InputGroup.Text><FontAwesomeIcon icon={faLock} fixedWidth /></InputGroup.Text>
                                                <Form.Control type="password" size="lg" placeholder="Confirm you new password" value={confirmPassword} onChange={(e) => {
                                                    setConfirmPassword(e.target.value);
                                                }} />
                                            </InputGroup>
                                        </Col>
                                    </Form.Group>
                                    {showAlert && !userContext.user?.email &&
                                        <Alert key="danger" variant="danger" onClose={() => setShowAlert(false)} dismissible>
                                            <FontAwesomeIcon icon={faWarning} /> To register your <strong>password</strong>, first enter your <strong>email</strong>!
                                        </Alert>
                                    }
                                    <div className="d-grid gap-2 d-md-flex justify-content-md-end">
                                        {userContext.user?.email ?
                                            <Button variant="success" disabled={userContext.loadingUpdate} size="lg" onClick={async (e) => {
                                                e.preventDefault();
                                                if (userContext.userHasPassword) {
                                                    if (!oldPassword) {
                                                        throwError("Old password is empty!");
                                                        return;
                                                    }
                                                }
                                                if (!newPassword) {
                                                    throwError("New password is empty!");
                                                    return;
                                                }
                                                if (!confirmPassword) {
                                                    throwError("Confirm password is empty!");
                                                    return;
                                                }
                                                if (newPassword != confirmPassword) {
                                                    throwError("New password and Confirmation are not equal!");
                                                    return;
                                                }
                                                let ret = await userContext.changePassword(userContext.user?.id, oldPassword, newPassword);
                                                console.log("ret:", JSON.stringify(ret));
                                                if (ret.sucesso) {
                                                    showSuccessMessage(ret.mensagemSucesso);
                                                }
                                                else {
                                                    throwError(ret.mensagemErro);
                                                }
                                            }}>
                                                <FontAwesomeIcon icon={faSave} fixedWidth />&nbsp;
                                                {userContext.loadingUpdate ? "Loading..." : "Change Password"}
                                            </Button>
                                            :
                                            <>
                                                <Button variant="danger" size="lg" onClick={(e) => {
                                                    e.preventDefault();
                                                    navigate("/edit-account");
                                                }}>
                                                    <FontAwesomeIcon icon={faUserEdit} fixedWidth /> Edit Account
                                                </Button>
                                                <Button variant="success" size="lg" disabled={true}>
                                                    <FontAwesomeIcon icon={faSave} fixedWidth /> Change Password
                                                </Button>
                                            </>
                                        }
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