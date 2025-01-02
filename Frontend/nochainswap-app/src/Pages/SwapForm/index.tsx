import Container from 'react-bootstrap/Container';
import Button from 'react-bootstrap/Button';
import Card from 'react-bootstrap/Card';
import Col from 'react-bootstrap/Col';
import Form from 'react-bootstrap/Form';
import Row from 'react-bootstrap/Row';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faRetweet } from '@fortawesome/free-solid-svg-icons/faRetweet'
import { useContext, useEffect, useState } from 'react';
import SwapContext from '../../Contexts/Swap/SwapContext';
import { CoinEnum } from '../../DTO/Enum/CoinEnum';
import Modal from 'react-bootstrap/Modal';
import ProviderResult from '../../DTO/Contexts/ProviderResult';
import CurrencyInput from 'react-currency-input-field';
import { redirect, useNavigate } from 'react-router-dom';
import MessageToast from '../../Components/MessageToast';
import { MessageToastEnum } from '../../DTO/Enum/MessageToastEnum';
import { faEnvelope, faLock, faRightLeft } from '@fortawesome/free-solid-svg-icons';
import InputGroup from 'react-bootstrap/InputGroup';
import AuthContext from '../../Contexts/Auth/AuthContext';
import UserContext from '../../Contexts/User/UserContext';
import env from 'react-dotenv';

export default function SwapForm() {

    const [dialog, setDialog] = useState<MessageToastEnum>(MessageToastEnum.Error);
    const [showMessage, setShowMessage] = useState<boolean>(false);
    const [messageText, setMessageText] = useState<string>("");

    const [address, setAddress] = useState<string>("");
    const [email, setEmail] = useState<string>("");

    const authContext = useContext(AuthContext);
    const userContext = useContext(UserContext);
    const swapContext = useContext(SwapContext);
    //const txContext = useContext(TxContext);

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

    const addressPlaceHolderHandle = () => {
        switch (swapContext.receiverCoin) {
            case CoinEnum.Bitcoin:
                return "Your Bitcoin Address";
                break;
            case CoinEnum.Stacks:
                return "Your Stacks Address";
                break;
            case CoinEnum.USDT:
                return "Your BNB Address";
                break;
            case CoinEnum.BRL:
                return "Sua chave PIX";
                break;
        }
    };

    const btnSwapTextHandle = () => {
        if (swapContext.loadingPrice) {
            return "Loading Price...";
        }
        else if (swapContext.loadingExecute) {
            return "Loading...";
        }
        else {
            return "SWAP";
        }
    };

    let navigate = useNavigate();

    useEffect(() => {
        swapContext.loadCurrentPrice(swapContext.senderCoin, swapContext.receiverCoin);
        if (authContext.sessionInfo && authContext.sessionInfo?.id > 0) {
            userContext.getMe().then((ret) => {
                if (ret.sucesso) {
                    userContext.getAddressByChain(authContext.chain).then((retAddr) => {
                        if (retAddr.sucesso) {
                            setAddress(retAddr.UserAddress);
                        } else {
                            throwError(retAddr.mensagemErro);
                        }
                    });
                } else {
                    throwError(ret.mensagemErro);
                }
            });
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
                        <Card className='shadow'>
                            <Card.Body style={{ position: "relative" }}>
                                <h4 className="text-center">{env.PROJECT_NAME}</h4>
                                <Card className='mb-3'>
                                    <Card.Body>
                                        <Row>
                                            <Col md="6">
                                                <Form.Group as={Col}>
                                                    <Form.Label>From</Form.Label>
                                                    <Form.Select size="lg" value={swapContext.senderCoin} onChange={async (e) => {
                                                        let senderCoin: CoinEnum = parseInt(e.target.value);
                                                        let ret = await swapContext.setSenderCoin(senderCoin);
                                                        //console.log("loadCurrentPrice: ", JSON.stringify(ret));
                                                        if (!ret.sucesso) {
                                                            throwError(ret.mensagemErro);
                                                        }
                                                    }}>
                                                        {env.USE_BITCOIN_SWAP == true &&
                                                            <>
                                                                <option value={CoinEnum.Bitcoin}>Bitcoin</option>
                                                                <option value={CoinEnum.Stacks}>Stacks</option>
                                                            </>
                                                        }
                                                        {env.USER_BRL_SWAP == true &&
                                                            <option value={CoinEnum.BRL}>Real (Pix)</option>
                                                        }
                                                        <option value={CoinEnum.USDT}>USDT (BNB)</option>
                                                    </Form.Select>
                                                    <Form.Text className='text-right' muted>Price: {swapContext.getFormatedSenderPrice()}</Form.Text>
                                                </Form.Group>
                                            </Col>
                                            <Col md="6">
                                                <Form.Label htmlFor="origAmount">Amount
                                                    {Number(swapContext.senderPoolBalance) > 0 &&
                                                        <>(Pool Balance: {swapContext.getFormatedSenderBalance()})</>
                                                    }
                                                    :</Form.Label>
                                                <Form.Group as={Col} style={{ textAlign: "right" }}>
                                                    <Form.Control
                                                        type="number" size="lg"
                                                        style={{ textAlign: 'right' }}
                                                        value={swapContext.senderAmount}
                                                        onChange={(e) => {
                                                            swapContext.setSenderAmount(parseFloat(e.target.value));
                                                        }}></Form.Control>
                                                    {/*
                                                <CurrencyInput
                                                    className='form-control form-control-lg'
                                                    decimalSeparator="."
                                                    groupSeparator=","
                                                    defaultValue={0.00000}
                                                    //defaultValue={swapContext.origAmount}
                                                    style={{ textAlign: 'right' }}
                                                    decimalScale={5}
                                                    fixedDecimalLength={5}
                                                    allowNegativeValue={false}
                                                    disableGroupSeparators={true}
                                                    disableAbbreviations={true}
                                                    value={swapContext.origAmount}
                                                    //onChange={(e) => {
                                                        //swapContext.setOrigAmount(parseFloat(e.target.value));
                                                    //}}
                                                    onValueChange={(value, name, values) => {
                                                        swapContext.setOrigAmount(values.float);
                                                    }}
                                                ></CurrencyInput>
                                                */}
                                                    <Form.Text className='text-end' muted>
                                                        {swapContext.senderFee > 0 &&
                                                            <>
                                                                Fee (3%): {swapContext.getFormatedSenderFee()}
                                                            </>
                                                        }
                                                    </Form.Text>
                                                </Form.Group>
                                            </Col>
                                        </Row>
                                    </Card.Body>
                                </Card>
                                <div style={{ position: "absolute", zIndex: 9999, marginTop: -32, marginLeft: -26, left: "50%" }}>
                                    <Button className='btn-circle btn-lg' size="lg" variant="warning"

                                        onClick={() => {
                                            swapContext.reverseCoin();
                                        }}>
                                        <FontAwesomeIcon icon={faRightLeft} style={{ transform: "rotate(90deg)" }} />
                                    </Button>
                                </div>
                                <Card className="mb-3">
                                    <Card.Body>
                                        <Row>
                                            <Col md="6">
                                                <Form.Group as={Col}>
                                                    <Form.Label>To</Form.Label>
                                                    <Form.Select size="lg" value={swapContext.receiverCoin} onChange={async (e) => {
                                                        let receiverCoin: CoinEnum = parseInt(e.target.value);
                                                        let ret = await swapContext.setReceiverCoin(receiverCoin);
                                                        if (!ret.sucesso) {
                                                            throwError(ret.mensagemErro);
                                                        }
                                                    }}>
                                                        {env.USE_BITCOIN_SWAP == true &&
                                                            <>
                                                                <option value={CoinEnum.Bitcoin}>Bitcoin</option>
                                                                <option value={CoinEnum.Stacks}>Stacks</option>
                                                            </>
                                                        }
                                                        {env.USER_BRL_SWAP == true &&
                                                            <option value={CoinEnum.BRL}>Real (Pix)</option>
                                                        }
                                                        <option value={CoinEnum.USDT}>USDT (BNB)</option>
                                                    </Form.Select>
                                                    <Form.Text className='text-right' muted>Price: {swapContext.getFormatedReceiverPrice()}</Form.Text>
                                                </Form.Group>
                                            </Col>
                                            <Col md="6">
                                                <Form.Label htmlFor="destAmount">Amount
                                                    {Number(swapContext.receiverPoolBalance) > 0 &&
                                                        <>(Pool Balance: {swapContext.getFormatedReceiverBalance()})</>
                                                    }
                                                    :</Form.Label>
                                                <Form.Group as={Col} style={{ textAlign: "right" }}>
                                                    <Form.Control
                                                        type="number" size="lg"
                                                        style={{ textAlign: 'right' }}
                                                        value={swapContext.receiverAmount}>
                                                    </Form.Control>
                                                    {/*
                                                <CurrencyInput
                                                    className='form-control form-control-lg'
                                                    decimalSeparator="."
                                                    groupSeparator=","
                                                    defaultValue={0.00000}
                                                    style={{ textAlign: 'right' }}
                                                    decimalScale={5}
                                                    fixedDecimalLength={5}
                                                    allowNegativeValue={false}
                                                    disableGroupSeparators={true}
                                                    value={swapContext.destAmount}
                                                ></CurrencyInput>
                                                */}
                                                    <Form.Text muted>
                                                        {swapContext.receiverFee > 0 &&
                                                            <>
                                                                Fee (3%): {swapContext.getFormatedReceiverFee()}
                                                            </>
                                                        }
                                                    </Form.Text>
                                                </Form.Group>
                                            </Col>
                                        </Row>
                                        <hr />
                                        <Row>
                                            <Form.Group as={Col} className="mb-3">
                                                <InputGroup>
                                                    <InputGroup.Text><FontAwesomeIcon icon={faLock} fixedWidth /></InputGroup.Text>
                                                    <Form.Control
                                                        type="text" size="sm"
                                                        value={address}
                                                        placeholder={addressPlaceHolderHandle()}
                                                        onChange={(e) => {
                                                            setAddress(e.target.value);
                                                        }} />
                                                </InputGroup>
                                            </Form.Group>
                                        </Row>
                                        {!userContext.user?.email &&
                                            <Row>
                                                <Form.Group as={Col}>
                                                    <InputGroup>
                                                        <InputGroup.Text><FontAwesomeIcon icon={faEnvelope} fixedWidth /></InputGroup.Text>
                                                        <Form.Control
                                                            type="text" size="sm"
                                                            value={email}
                                                            placeholder="Email to receive updates (optional)"
                                                            onChange={(e) => {
                                                                setEmail(e.target.value);
                                                            }} />
                                                    </InputGroup>
                                                </Form.Group>
                                            </Row>
                                        }
                                    </Card.Body>
                                </Card>
                                <p className="mb-3" style={{ textAlign: 'center' }}>{swapContext.getCoinText()}</p>
                                <Row>
                                    <Col md="4" className='offset-md-4'>
                                        <view className='d-grid gap-2'>
                                            <Button
                                                size="lg"
                                                variant="danger"
                                                disabled={swapContext.loadingPrice || swapContext.loadingExecute || address == ""}
                                                onClick={async () => {
                                                    if (swapContext.senderAmount > 0) {
                                                        let ret = await swapContext.createTx(authContext.chain, email, address);
                                                        if (ret.sucesso) {
                                                            navigate("/tx/" + ret.hash);
                                                        }
                                                        else {
                                                            throwError(ret.mensagemErro);
                                                        }
                                                    }
                                                    else {
                                                        throwError("Amount is empty!");
                                                    }
                                                }}>
                                                {btnSwapTextHandle()}
                                            </Button>
                                        </view>
                                    </Col>
                                </Row>
                            </Card.Body>
                        </Card>
                    </Col>
                </Row>
            </Container>
        </>
    );
}