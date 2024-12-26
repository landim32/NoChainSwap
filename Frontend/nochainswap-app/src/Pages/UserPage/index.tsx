import { useContext, useEffect, useState } from "react";
import Col from "react-bootstrap/esm/Col";
import Container from "react-bootstrap/esm/Container";
import Row from "react-bootstrap/esm/Row";
import Form from 'react-bootstrap/Form';
import AuthContext from "../../Contexts/Auth/AuthContext";
import Button from "react-bootstrap/esm/Button";
import Card from 'react-bootstrap/Card';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBitcoinSign, faCalendar, faCalendarAlt, faCancel, faClose, faEnvelope, faEthernet, faLock, faSave, faSignInAlt, faTrash, faUser } from '@fortawesome/free-solid-svg-icons';
import Table from "react-bootstrap/esm/Table";
import { Link, useNavigate } from "react-router-dom";
import InputGroup from 'react-bootstrap/InputGroup';
import UserContext from "../../Contexts/User/UserContext";
import MessageToast from "../../Components/MessageToast";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import UserAddressInfo from "../../DTO/Domain/UserAddressInfo";
import Moment from 'moment';
import { MessageToastEnum } from "../../DTO/Enum/MessageToastEnum";

export default function UserPage() {

    const authContext = useContext(AuthContext);
    const userContext = useContext(UserContext);

    const [insertMode, setInsertMode] = useState<boolean>(false);

    const [dialog, setDialog] = useState<MessageToastEnum>(MessageToastEnum.Error);
    const [showMessage, setShowMessage] = useState<boolean>(false);
    const [messageText, setMessageText] = useState<string>("");

    let navigate = useNavigate();
    Moment.locale('en');

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

    const chainToStr = (chain: ChainEnum) => {
        switch (chain) {
            case ChainEnum.NoChain:
                return (
                    <>
                        <FontAwesomeIcon icon={faCancel} fixedWidth /> No Chain
                    </>
                );
                break;
            case ChainEnum.BitcoinAndStack:
                return (
                    <>
                        <FontAwesomeIcon icon={faBitcoinSign} fixedWidth /> Bitcoin & Stack
                    </>
                );
                break;
            case ChainEnum.BNBChain:
                return (
                    <>
                        <FontAwesomeIcon icon={faEthernet} fixedWidth /> BNB Chain
                    </>
                );
                break;
        }
    };

    useEffect(() => {
        if (authContext.sessionInfo) {
            if (authContext.sessionInfo?.id > 0) {
                userContext.getMe().then((ret) => {
                    if (ret.sucesso) {
                        setInsertMode(false);
                        userContext.listAddressByUser().then((retAddr) => {
                            if (!retAddr.sucesso) {
                                throwError(retAddr.mensagemErro);
                            }
                        });
                    }
                    else {
                        setInsertMode(true);
                    }
                });
            }
            else {
                setInsertMode(true);
            }
        }
        else {
            setInsertMode(true);
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
                    <Col md="8" className='offset-md-2'>
                        <Card>
                            <Card.Header>
                                <h3 className="text-center">User registration</h3>
                            </Card.Header>
                            <Card.Body>
                                <Form>
                                    <div className="text-center mb-3">
                                        Registration is not required to make swaps, but you can do so anyway to access your transaction history.
                                    </div>
                                    {!insertMode &&
                                        <Form.Group as={Row} className="mb-3">
                                            <Form.Label column sm="2">Hash:</Form.Label>
                                            <Col sm="10">
                                                <InputGroup>
                                                    <InputGroup.Text><FontAwesomeIcon icon={faLock} fixedWidth /></InputGroup.Text>
                                                    <Form.Control type="text" size="lg" className="readonly"
                                                        disabled={true} readOnly={true}
                                                        value={userContext.user?.hash}
                                                    />
                                                </InputGroup>
                                            </Col>
                                        </Form.Group>
                                    }
                                    <Form.Group as={Row} className="mb-3">
                                        <Form.Label column sm="2">Name:</Form.Label>
                                        <Col sm="10">
                                            <InputGroup>
                                                <InputGroup.Text><FontAwesomeIcon icon={faUser} fixedWidth /></InputGroup.Text>
                                                <Form.Control type="text" size="lg"
                                                    placeholder="Your name"
                                                    value={userContext.user?.name}
                                                    onChange={(e) => {
                                                        userContext.setUser({
                                                            ...userContext.user,
                                                            name: e.target.value
                                                        });
                                                    }} />
                                            </InputGroup>
                                        </Col>
                                    </Form.Group>
                                    <Form.Group as={Row} className="mb-3">
                                        <Form.Label column sm="2">Email:</Form.Label>
                                        <Col sm="10">
                                            <InputGroup>
                                                <InputGroup.Text><FontAwesomeIcon icon={faEnvelope} fixedWidth /></InputGroup.Text>
                                                <Form.Control type="text" size="lg"
                                                    placeholder="Your email"
                                                    value={userContext.user?.email}
                                                    onChange={(e) => {
                                                        userContext.setUser({
                                                            ...userContext.user,
                                                            email: e.target.value
                                                        });
                                                    }} />
                                            </InputGroup>
                                        </Col>
                                    </Form.Group>
                                    {!insertMode &&
                                        <Form.Group as={Row} className="mb-3">
                                            <Form.Label column sm="2">Create At:</Form.Label>
                                            <Col sm="4">
                                                <InputGroup>
                                                    <InputGroup.Text><FontAwesomeIcon icon={faCalendarAlt} fixedWidth /></InputGroup.Text>
                                                    <Form.Control type="text" size="lg" disabled={true} readOnly={true}
                                                        value={Moment(userContext.user?.createAt).format("MMM DD YYYY")} />
                                                </InputGroup>
                                            </Col>
                                            <Form.Label column sm="2">Update At:</Form.Label>
                                            <Col sm="4">
                                                <InputGroup>
                                                    <InputGroup.Text><FontAwesomeIcon icon={faCalendarAlt} fixedWidth /></InputGroup.Text>
                                                    <Form.Control type="text" size="lg" disabled={true} readOnly={true}
                                                        value={Moment(userContext.user?.updateAt).format("MMM DD YYYY")} />
                                                </InputGroup>
                                            </Col>
                                        </Form.Group>
                                    }
                                    {!insertMode &&
                                        <>
                                            <hr />
                                            <Table striped bordered hover size="lg">
                                                <thead>
                                                    <tr>
                                                        <th scope="col">Chain</th>
                                                        <th scope="col">Address</th>
                                                        <th scope="col" style={{ textAlign: "center" }}>-</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    {userContext.loadingUserAddr &&
                                                        <tr>
                                                            <td colSpan={5}>
                                                                <div className="d-flex justify-content-center">
                                                                    <div className="spinner-border" role="status">
                                                                        <span className="visually-hidden">Loading...</span>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    }
                                                    {userContext.userAddresses && userContext.userAddresses.map((addr: UserAddressInfo) => {
                                                        return (
                                                            <tr>
                                                                <td scope="col" style={{ whiteSpace: "nowrap" }}>
                                                                    {chainToStr(addr.chainId)}
                                                                </td>
                                                                <td scope="col">{addr.address}</td>
                                                                <td scope="col" style={{ textAlign: "center" }}>
                                                                    <a href="#" onClick={async (e) => {
                                                                        e.preventDefault();
                                                                        let ret = await userContext.removeAddress(addr.chainId);
                                                                        if (!ret.sucesso) {
                                                                            throwError(ret.mensagemErro);
                                                                        }
                                                                    }}><FontAwesomeIcon icon={faTrash} fixedWidth /></a>
                                                                </td>
                                                            </tr>
                                                        )
                                                    })}
                                                </tbody>
                                            </Table>
                                        </>
                                    }
                                    <div className="d-grid gap-2 d-md-flex justify-content-md-end">
                                        <Button variant="danger" size="lg" onClick={() => {
                                            navigate("/login");
                                        }}><FontAwesomeIcon icon={faSignInAlt} fixedWidth /> Sign In</Button>
                                        <Button variant="success" size="lg" onClick={async (e) => {
                                            if (insertMode) {
                                                let ret = await userContext.insert(userContext.user);
                                                if (ret.sucesso) {
                                                    showSuccessMessage(ret.mensagemSucesso);
                                                    //alert(userContext.user?.id);
                                                }
                                                else {
                                                    throwError(ret.mensagemErro);
                                                }
                                            }
                                            else {
                                                let ret = await userContext.update(userContext.user);
                                                if (ret.sucesso) {
                                                    //alert(userContext.user?.id);
                                                    showSuccessMessage(ret.mensagemSucesso);
                                                }
                                                else {
                                                    throwError(ret.mensagemErro);
                                                }
                                            }
                                        }}
                                            disabled={userContext.loadingUpdate}
                                        ><FontAwesomeIcon icon={faSave} fixedWidth />
                                            {userContext.loadingUpdate ? "Loading..." : "Save"}
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