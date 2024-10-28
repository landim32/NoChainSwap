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

export default function SwapForm() {

    const [showModal, setShowModal] = useState<boolean>(false);

    const swapContext = useContext(SwapContext);
    useEffect(() => {
        swapContext.loadCurrentPrice();
    }, []);

    return (
        <Container>
            <Modal
                show={showModal}
                size="lg"
                onHide={() => { setShowModal(false) }}
                backdrop="static"
                keyboard={false}
            >
                <Modal.Header closeButton>
                    <Modal.Title>Confirm Swap</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <p>You are about to make a conversion of <strong>{swapContext.getFormatedSenderAmount()}</strong> into <strong>{swapContext.getFormatedReceiverAmount()}</strong>.</p>
                    <p><strong>{swapContext.getFormatedSenderAmount()}</strong> -&gt; <span>{swapContext.senderPoolAddress}</span> (Pool Address)</p>
                    <p>As soon as the transaction reaches <strong>confirmation</strong> the transfer of
                        <strong>{swapContext.getFormatedReceiverAmount()}</strong> will be initiated to the address provided by your Leather Wallet:
                    </p>
                    <p><strong>{swapContext.getFormatedReceiverAmount()}</strong> -&gt; <span>{swapContext.receiverPoolAddress}</span> (Your Wallet Address)</p>
                    <p>A fee of <strong>230 satoshis</strong> will be charged, please confirm if you agree.</p>
                    <p>Do you confirm this transaction?</p>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => { setShowModal(false) }}>
                        Close
                    </Button>
                    <Button variant="primary" onClick={() => {
                        let callback = (ret: ProviderResult) => {
                            if (ret.sucesso) {
                                console.log("txId: ", swapContext.currentTxId);
                                alert(ret.mensagemSucesso);
                                setShowModal(false);
                            }
                            else {
                                alert(ret.mensagemErro);
                                setShowModal(false);
                            }
                        };
                        swapContext.execute(callback);
                    }}>Confirm Swap</Button>
                </Modal.Footer>
            </Modal>
            <Row>
                <Col md="6" className='offset-md-3'>
                    <Card>
                        <Card.Body>
                            <h1 className="text-center">BTC to STX</h1>
                            <Card className="mb-3">
                                <Card.Body>
                                    <Row>
                                        <Col md="6">
                                            <Form.Group as={Col}>
                                                <Form.Label>From</Form.Label>
                                                <Form.Select size="lg" value={swapContext.senderCoin} onChange={(e) => {
                                                    if (e.target.value == '1') {
                                                        swapContext.setSenderCoin(CoinEnum.Bitcoin);
                                                    }
                                                    else {
                                                        swapContext.setSenderCoin(CoinEnum.Stacks);
                                                    }
                                                }}>
                                                    <option value={CoinEnum.Bitcoin}>Bitcoin</option>
                                                    <option value={CoinEnum.Stacks}>Stacks</option>
                                                </Form.Select>
                                                <Form.Text className='text-right' muted>Price: {swapContext.getFormatedSenderPrice()}</Form.Text>
                                            </Form.Group>
                                        </Col>
                                        <Col md="6">
                                            <Form.Label htmlFor="origAmount">Amount:</Form.Label>
                                            <Form.Group as={Col}>
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
                                                <Form.Text className='text-right' muted>Pool Balance {swapContext.getFormatedSenderBalance()}</Form.Text>
                                            </Form.Group>
                                        </Col>
                                    </Row>
                                </Card.Body>
                            </Card>
                            <Row className="mb-3">
                                <Col md="2" className='offset-md-5'>
                                    <view className='d-grid gap-2'>
                                        <Button size="lg" variant="warning" onClick={() => {
                                            swapContext.reverseCoin();
                                        }}>
                                            <FontAwesomeIcon icon={faRetweet} />
                                        </Button>
                                    </view>
                                </Col>
                            </Row>
                            <Card className="mb-3">
                                <Card.Body>
                                    <Row>
                                        <Col md="6">
                                            <Form.Group as={Col}>
                                                <Form.Label>To</Form.Label>
                                                <Form.Select size="lg" value={swapContext.receiverCoin} onChange={(e) => {
                                                    if (e.target.value == '1') {
                                                        swapContext.setReceiverCoin(CoinEnum.Bitcoin);
                                                    }
                                                    else {
                                                        swapContext.setReceiverCoin(CoinEnum.Stacks);
                                                    }
                                                }}>
                                                    <option value={CoinEnum.Bitcoin}>Bitcoin</option>
                                                    <option value={CoinEnum.Stacks}>Stacks</option>
                                                </Form.Select>
                                                <Form.Text className='text-right' muted>Price: {swapContext.getFormatedReceiverPrice()}</Form.Text>
                                            </Form.Group>
                                        </Col>
                                        <Col md="6">
                                            <Form.Label htmlFor="destAmount">Amount:</Form.Label>
                                            <Form.Group as={Col}>
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
                                                <Form.Text className='text-right' muted>Pool Balance {swapContext.getFormatedReceiverBalance()}</Form.Text>
                                            </Form.Group>
                                        </Col>
                                    </Row>
                                </Card.Body>
                            </Card>
                            <p className="mb-3" style={{ textAlign: 'center' }}>{swapContext.getCoinText()}</p>
                            <Row>
                                <Col md="4" className='offset-md-4'>
                                    <view className='d-grid gap-2'>
                                        <Button size="lg" variant="primary" onClick={() => {
                                            if (swapContext.senderAmount > 0) {
                                                setShowModal(true);
                                            }
                                            else {
                                                alert("Amount is empty!");
                                            }
                                        }}>
                                            Swap
                                        </Button>
                                    </view>
                                </Col>
                            </Row>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
}