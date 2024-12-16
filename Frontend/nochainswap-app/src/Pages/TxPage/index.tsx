import Container from 'react-bootstrap/Container';
import Button from 'react-bootstrap/Button';
import Card from 'react-bootstrap/Card';
import Col from 'react-bootstrap/Col';
import Form from 'react-bootstrap/Form';
import Row from 'react-bootstrap/Row';
import Table from 'react-bootstrap/Table';
import InputGroup from 'react-bootstrap/InputGroup';
import QRCode from "react-qr-code";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCancel, faClipboard, faDollar, faRefresh } from '@fortawesome/free-solid-svg-icons';
import { useContext, useEffect, useState } from 'react';
import TxContext from '../../Contexts/Transaction/TxContext';
import { useParams } from 'react-router-dom';
import MessageToast from '../../Components/MessageToast';
import { MessageToastEnum } from '../../DTO/Enum/MessageToastEnum';
import SwapContext from '../../Contexts/Swap/SwapContext';
import Skeleton from 'react-loading-skeleton';
import ButtonGroup from 'react-bootstrap/ButtonGroup';
import DropdownButton from 'react-bootstrap/DropdownButton';
import Dropdown from 'react-bootstrap/Dropdown';
import { TransactionStatusEnum } from '../../DTO/Enum/TransactionStatusEnum';
import { CoinEnum } from '../../DTO/Enum/CoinEnum';

export default function TxPage() {

    let { txHash } = useParams();

    const [dialog, setDialog] = useState<MessageToastEnum>(MessageToastEnum.Error);
    const [showMessage, setShowMessage] = useState<boolean>(false);
    const [messageText, setMessageText] = useState<string>("");
    const [status, setStatus] = useState<TransactionStatusEnum>(TransactionStatusEnum.Initialized);
    const [logMessage, setLogMessage] = useState<string>("");

    const [showConfirmation, setShowConfirmation] = useState<boolean>(false);

    const txContext = useContext(TxContext);

    const throwError = (message: string) => {
        setDialog(MessageToastEnum.Error);
        setMessageText(message);
        setShowMessage(true);
    };
    const showSuccessMessage = (message: string) => {
        setDialog(MessageToastEnum.Success);
        setMessageText(message);
        setShowMessage(true);
    };

    const changeStatusHandle = (status: TransactionStatusEnum, logMessage: string) => {
        setShowConfirmation(true);
        setStatus(status);
        setLogMessage(logMessage);
    };

    const loadTxHandler = () => {
        txContext.loadTx(txHash).then((ret) => {
            if (!ret.sucesso) {
                throwError(ret.mensagemErro);
            }
        });
    };

    useEffect(() => {
        loadTxHandler();
    }, []);

    return (
        <>
            <MessageToast
                dialog={dialog}
                showMessage={showMessage}
                messageText={messageText}
                onClose={() => setShowMessage(false)}
            ></MessageToast>
            <MessageToast
                dialog={MessageToastEnum.Confirmation}
                showMessage={showConfirmation}
                messageText="Are you sure?"
                onClose={() => setShowConfirmation(false)}
                onYes={async () => {
                    let ret = await txContext.changeStatus(
                        txContext.txInfo?.txid,
                        status,
                        logMessage
                    );
                    if (ret.sucesso) {
                        setShowConfirmation(false);
                        showSuccessMessage("Transaction status successfully changed!");
                        loadTxHandler();
                    }
                    else {
                        setShowConfirmation(false);
                        throwError(ret.mensagemErro);
                    }
                }}
                onNo={() => setShowConfirmation(false)}
            ></MessageToast>
            <Container>
                <Row>
                    <Col md="12">
                        <Card className='shadow'>
                            <Card.Body>
                                <div className='text-end'>
                                    <ButtonGroup>
                                        {(txContext.txInfo?.status == TransactionStatusEnum.WaitingSenderPayment ||
                                            txContext.txInfo?.status == TransactionStatusEnum.SenderConfirmed) &&
                                            <>
                                                {txContext.txInfo?.receivercoin == 'usdt' &&
                                                <Button variant="success" disabled={txContext.loadingPay} onClick={async (e) => {
                                                    e.preventDefault();
                                                    let ret = await txContext.paybackTx();
                                                    if (ret.sucesso) {
                                                        showSuccessMessage("Payback successfully!");
                                                        loadTxHandler();
                                                    }
                                                    else {
                                                        throwError(ret.mensagemErro);
                                                    }
                                                }}><FontAwesomeIcon icon={faDollar} fixedWidth /> {
                                                        txContext.loadingPay ?
                                                            "Loading..." :
                                                            "Payback (" + (txContext.txInfo?.receiverpayback / 100000000).toFixed(5) + ")"
                                                    }</Button>
                                                }
                                            </>

                                        }
                                        {txContext.txInfo?.status == TransactionStatusEnum.WaitingSenderPayment &&
                                                   <Button variant="success" onClick={(e) => {
                                                    e.preventDefault();
                                                    changeStatusHandle(TransactionStatusEnum.SenderConfirmed, "Sender payment confirmed");
                                                }}>
                                                    <FontAwesomeIcon icon={faDollar} fixedWidth /> Confirm Payment
                                                </Button>
                                        }
                                        <Button variant="danger" onClick={(e) => {
                                            e.preventDefault();
                                            changeStatusHandle(TransactionStatusEnum.Initialized, "Revert All by User");
                                        }}>Revert All</Button>
                                        {(txContext.txInfo?.status == TransactionStatusEnum.CriticalError ||
                                            txContext.txInfo?.status == TransactionStatusEnum.InvalidInformation) &&
                                            <>
                                                <Button variant="danger" onClick={(e) => {
                                                    e.preventDefault();
                                                    changeStatusHandle(TransactionStatusEnum.WaitingSenderPayment, "Revert to 'Waiting to payment'");
                                                }}>Revert to "Wait Sender Payment"</Button>
                                            </>
                                        }
                                        <Button variant="secondary" onClick={async (e) => {
                                            e.preventDefault();
                                            let ret = await txContext.reloadTx();
                                            console.log("ret: ", JSON.stringify(ret));
                                            if (!ret.sucesso) {
                                                throwError(ret.mensagemErro);
                                            }
                                        }}>
                                            <FontAwesomeIcon icon={faRefresh} fixedWidth />
                                        </Button>
                                        {txContext.txInfo?.status != TransactionStatusEnum.Finished &&
                                            <Button variant="danger" onClick={(e) => {
                                                e.preventDefault();
                                                changeStatusHandle(TransactionStatusEnum.Canceled, "Transaction canceled by admin");
                                            }}>
                                                <FontAwesomeIcon icon={faCancel} fixedWidth />
                                            </Button>
                                        }
                                    </ButtonGroup>
                                </div>
                                <h4 className="text-center">{txContext.loadingTxInfo ? <Skeleton /> : "Transaction " + txContext.getTitle()}</h4>
                                {txContext.txInfo?.status == TransactionStatusEnum.WaitingSenderPayment &&
                                    <>
                                        {txContext.txInfo?.sendercoin == 'brl' &&
                                            <Card className='mb-3'>
                                                <Card.Header>
                                                    <Card.Title>Send PIX to Atomic Swap Contract</Card.Title>
                                                </Card.Header>
                                                <Card.Body style={{ position: "relative" }}>
                                                    <Row>
                                                        <Col md="8" className="mx-auto border-end">
                                                            <div className='small mb-2'>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.</div>
                                                            <Form>
                                                                <Form.Group as={Row} className='mb-2'>
                                                                    <Form.Label as={Col} sm="3">Valor em Real (R$):</Form.Label>
                                                                    <Col sm="9">
                                                                        <InputGroup>
                                                                            <InputGroup.Text>R$</InputGroup.Text>
                                                                            <Form.Control type="text" readOnly={true} disabled={true} size="sm" value={txContext.txInfo?.senderamount} />
                                                                            <InputGroup.Text><FontAwesomeIcon icon={faClipboard} fixedWidth /></InputGroup.Text>
                                                                        </InputGroup>
                                                                    </Col>
                                                                </Form.Group>
                                                                <Form.Group as={Row} className="mb-3">
                                                                    <Form.Label as={Col} sm="3">Chave PIX:</Form.Label>
                                                                    <Col sm="9">
                                                                        <InputGroup>
                                                                            <Form.Control as="textarea" rows={5} readOnly={true} disabled={true} size="sm" value={txContext.txInfo?.recipientaddress} />
                                                                            <InputGroup.Text><FontAwesomeIcon icon={faClipboard} fixedWidth /></InputGroup.Text>
                                                                        </InputGroup>
                                                                    </Col>
                                                                </Form.Group>
                                                            </Form>
                                                        </Col>
                                                        <Col md="4" className="mx-auto">
                                                            <div className='small mb-2'>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.</div>
                                                            <div className="d-flex align-items-center justify-content-evenly">
                                                                {txContext.loadingTxInfo ?
                                                                    <Skeleton count={7} />
                                                                    :
                                                                    txContext.txInfo &&
                                                                    <QRCode
                                                                        size={256}
                                                                        style={{ height: "auto" }}
                                                                        value={txContext.txInfo?.recipientaddress}
                                                                        viewBox={`0 0 256 256`}
                                                                    />
                                                                }
                                                            </div>
                                                        </Col>
                                                    </Row>
                                                </Card.Body>
                                            </Card>
                                        }
                                        {txContext.txInfo?.sendercoin == 'usdt' &&
                                            <Card className='mb-3'>
                                                <Card.Header>
                                                    <Card.Title>Send USDT to Atomic Swap Contract</Card.Title>
                                                </Card.Header>
                                                <Card.Body style={{ position: "relative" }}>
                                                    <div style={{ position: "absolute", top: "45%", left: "50%", marginLeft: -10, backgroundColor: "#fff" }}>
                                                        <strong>OR</strong>
                                                    </div>
                                                    <Row>
                                                        <Col md="6" className="mx-auto border-end">
                                                            <div className='small mb-2'>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.</div>
                                                            <div className="d-flex align-items-center justify-content-evenly">
                                                                <Button variant='danger'>Send USDT Via Metamask</Button>
                                                            </div>
                                                        </Col>
                                                        <Col md="6" className="mx-auto">
                                                            <div className='small mb-2'>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.</div>
                                                            <div className="d-flex align-items-center justify-content-evenly">
                                                                <QRCode
                                                                    size={256}
                                                                    style={{ height: "auto" }}
                                                                    value={txContext.txInfo?.recipientaddress}
                                                                    viewBox={`0 0 256 256`}
                                                                />
                                                            </div>
                                                            <hr />
                                                            <Form>
                                                                <Form.Group as={Row} className='mb-2'>
                                                                    <Form.Label as={Col} sm="3">USDT Value:</Form.Label>
                                                                    <Col sm="9">
                                                                        <InputGroup>
                                                                            <Form.Control type="text" readOnly={true} disabled={true} size="sm" value={txContext.txInfo?.senderamount} />
                                                                            <InputGroup.Text><FontAwesomeIcon icon={faClipboard} fixedWidth /></InputGroup.Text>
                                                                        </InputGroup>
                                                                    </Col>
                                                                </Form.Group>
                                                                <Form.Group as={Row} className="mb-3">
                                                                    <Form.Label as={Col} sm="3">Copy Address:</Form.Label>
                                                                    <Col sm="9">
                                                                        <InputGroup>
                                                                            <Form.Control type="text" readOnly={true} disabled={true} size="sm" value={txContext.txInfo?.recipientaddress} />
                                                                            <InputGroup.Text><FontAwesomeIcon icon={faClipboard} fixedWidth /></InputGroup.Text>
                                                                        </InputGroup>
                                                                    </Col>
                                                                </Form.Group>
                                                            </Form>
                                                        </Col>
                                                    </Row>
                                                </Card.Body>
                                            </Card>
                                        }
                                    </>
                                }
                                <Card className='mb-3'>
                                    <Card.Header>
                                        <Card.Title>Transaction Info</Card.Title>
                                    </Card.Header>
                                    <Card.Body>
                                        <dl className="row">
                                            {txContext.loadingTxInfo ?
                                                <Skeleton />
                                                :
                                                <>
                                                    <dt className="col-sm-3">Status</dt>
                                                    <dd className="col-sm-9">{txContext.getStatus(txContext.txInfo?.status)}</dd>
                                                </>
                                            }
                                            {txContext.txInfo?.sendercoin != 'brl' &&
                                                txContext.loadingTxInfo ?
                                                <Skeleton />
                                                :
                                                <>
                                                    {txContext.txInfo?.senderaddress &&
                                                        <>
                                                            <dt className="col-sm-3">Sender Address</dt>
                                                            <dd className="col-sm-9"><a href={txContext.txInfo?.senderaddressurl} target="_blank">{txContext.txInfo?.senderaddress}</a></dd>
                                                        </>
                                                    }
                                                    {txContext.txInfo?.sendertxid &&
                                                        <>
                                                            <dt className="col-sm-3">Sender TxID</dt>
                                                            <dd className="col-sm-9"><a href={txContext.txInfo?.sendertxidurl} target="_blank">{txContext.txInfo?.sendertxid}</a></dd>
                                                        </>
                                                    }
                                                </>
                                            }
                                            {txContext.loadingTxInfo ?
                                                <Skeleton />
                                                :
                                                txContext.txInfo?.receiveraddress &&
                                                <>
                                                    <dt className="col-sm-3">Receiver Address</dt>
                                                    <dd className="col-sm-9"><a href={txContext.txInfo?.receiveraddressurl} target="_blank">{txContext.txInfo?.receiveraddress}</a></dd>
                                                </>
                                            }
                                            {txContext.loadingTxInfo ?
                                                <Skeleton />
                                                :
                                                (txContext.txInfo?.receivertxid) &&
                                                <>
                                                    <dt className="col-sm-3">Receiver TxID</dt>
                                                    <dd className="col-sm-9"><a href={txContext.txInfo?.receivertxidurl} target="_blank">{txContext.txInfo?.receivertxid}</a></dd>
                                                </>
                                            }
                                            {txContext.loadingTxInfo ?
                                                <Skeleton />
                                                :
                                                <>
                                                    <dt className="col-sm-3">Amounts</dt>
                                                    <dd className="col-sm-9">{
                                                        txContext.loadingTxInfo ?
                                                            <Skeleton />
                                                            :
                                                            txContext.txInfo?.senderamount + " -> " + txContext.txInfo?.receiveramount
                                                    }</dd>
                                                </>
                                            }
                                            {txContext.loadingTxInfo ?
                                                <Skeleton />
                                                :
                                                <>
                                                    <dt className="col-sm-3">Taxes</dt>
                                                    <dd className="col-sm-9">{
                                                        txContext.loadingTxInfo ?
                                                            <Skeleton />
                                                            :
                                                            txContext.txInfo?.sendertax ? txContext.txInfo?.sendertax : txContext.txInfo?.receivertax
                                                    }</dd>
                                                </>
                                            }
                                            {txContext.loadingTxInfo ?
                                                <Skeleton />
                                                :
                                                (txContext.txInfo?.senderfee && txContext.txInfo?.receiverfee) &&
                                                <>
                                                    <dt className="col-sm-3">Chain Fees</dt>
                                                    <dd className="col-sm-9">{txContext.txInfo?.senderfee + " + " + txContext.txInfo?.receiverfee}</dd>
                                                </>
                                            }
                                            {txContext.loadingTxInfo ?
                                                <Skeleton />
                                                :
                                                <>
                                                    <dt className="col-sm-3">Dates</dt>
                                                    <dd className="col-sm-9">{
                                                        "Create at " + txContext.txInfo?.createat + ", latest udpate at " + txContext.txInfo?.updateat
                                                    }</dd>
                                                </>
                                            }
                                        </dl>
                                        <hr />
                                        <Table striped bordered hover size="sm">
                                            <thead>
                                                <tr>
                                                    <th scope="col">Date</th>
                                                    <th scope="col">Type</th>
                                                    <th scope="col">Message</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                {
                                                    txContext.txLogs ?
                                                        txContext.txLogs.map((log) => {
                                                            return (
                                                                <tr>
                                                                    <td scope="col" style={{ whiteSpace: "nowrap" }}>{log.date}</td>
                                                                    <td scope="col">{
                                                                        (log.intlogtype == 1) &&
                                                                        <span className="badge rounded-pill text-bg-info">Info</span>
                                                                    }
                                                                        {
                                                                            (log.intlogtype == 2) &&
                                                                            <span className="badge rounded-pill text-bg-warning">Warning</span>
                                                                        }
                                                                        {
                                                                            (log.intlogtype == 3) &&
                                                                            <span className="badge rounded-pill text-bg-danger">Error</span>
                                                                        }</td>
                                                                    <td scope="col">{log.message}</td>
                                                                </tr>
                                                            );
                                                        })
                                                        :
                                                        txContext.loadingTxLogs &&
                                                        <tr>
                                                            <td colSpan={3}>
                                                                <div className="d-flex justify-content-center">
                                                                    <div className="spinner-border" role="status">
                                                                        <span className="visually-hidden">Loading...</span>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                }
                                            </tbody>
                                        </Table>
                                    </Card.Body>
                                </Card>
                            </Card.Body>
                        </Card>
                    </Col>
                </Row>
            </Container>
        </>
    );
}