import { useContext, useEffect, useState } from "react";
import Col from "react-bootstrap/esm/Col";
import Container from "react-bootstrap/esm/Container";
import Row from "react-bootstrap/esm/Row";
import Form from 'react-bootstrap/Form';
import AuthContext from "../../Contexts/Auth/AuthContext";
import Button from "react-bootstrap/esm/Button";
import Card from 'react-bootstrap/Card';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBitcoinSign, faCalendar, faCalendarAlt, faCancel, faClose, faEnvelope, faEthernet, faLock, faSave, faTrash, faUser } from '@fortawesome/free-solid-svg-icons';
import Table from "react-bootstrap/esm/Table";
import { Link } from "react-router-dom";
import InputGroup from 'react-bootstrap/InputGroup';
import UserContext from "../../Contexts/User/UserContext";
import ErrorToast from "../../Components/ErrorToast";
import { ChainEnum } from "../../DTO/Enum/ChainEnum";
import UserAddressInfo from "../../DTO/Domain/UserAddressInfo";

export default function UserPage() {

    const authContext = useContext(AuthContext);
    const userContext = useContext(UserContext);

    const [insertMode, setInsertMode] = useState<boolean>(false);
    const [showError, setShowError] = useState<boolean>(false);
    const [messageError, setMessageError] = useState<string>("");

    const throwError = (message: string) => {
        setMessageError(message);
        setShowError(true);
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
                userContext.getUserById(authContext.sessionInfo.id).then((ret) => {
                    if (!ret.sucesso) {
                        setInsertMode(false);
                    }
                    else {
                        throwError(ret.mensagemErro);
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
            <ErrorToast
                showError={showError}
                messageError={messageError}
                onClose={() => setShowError(false)}
            ></ErrorToast>
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
                                                <Form.Control type="text" size="lg" value={userContext.user?.name} onChange={(e) => {
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
                                                <Form.Control type="text" size="lg" value={userContext.user?.email} onChange={(e) => {
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
                                                        value={userContext.user?.createAt} />
                                                </InputGroup>
                                            </Col>
                                            <Form.Label column sm="2">Update At:</Form.Label>
                                            <Col sm="4">
                                                <InputGroup>
                                                    <InputGroup.Text><FontAwesomeIcon icon={faCalendarAlt} fixedWidth /></InputGroup.Text>
                                                    <Form.Control type="text" size="lg" disabled={true} readOnly={true}
                                                        value={userContext.user?.updateAt} />
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
                                                                        let ret = await userContext.removeAddress(addr.userId, addr.chainId);
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
                                        <Button variant="success" size="lg" onClick={async (e) => {
                                            if (insertMode) {
                                                let ret = await userContext.insert(userContext.user);
                                                if (ret.sucesso) {
                                                    alert(userContext.user?.id);
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