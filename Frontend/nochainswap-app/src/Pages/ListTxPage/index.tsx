import Table from 'react-bootstrap/Table';
import Card from 'react-bootstrap/Card';
import Col from 'react-bootstrap/Col';
import Row from 'react-bootstrap/Row';
import Container from 'react-bootstrap/esm/Container';
import { MouseEvent, MouseEventHandler, useContext, useEffect, useState } from 'react';
import TxContext from '../../Contexts/Transaction/TxContext';
import TxInfo from '../../DTO/Domain/TxInfo';
import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/esm/Button';

export default function ListTxPage() {

    const [showModal, setShowModal] = useState<boolean>(false);

    const txContext = useContext(TxContext);
    useEffect(() => {
        txContext.loadListAllTx().then((ret) => {
            if (!ret.sucesso) {
                alert(ret.mensagemErro);
            }
        });
    }, []);

    const txClickHandler = (e: any, item: TxInfo) => {
        e.preventDefault();
        txContext.setTxInfo(item);
        txContext.loadTxLogs(item.txid).then((ret) => {
            if (!ret.sucesso) {
                alert(ret.mensagemErro);
            }
        });
        setShowModal(true);
        //alert("hello");
    };

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
                    <Modal.Title>Transaction {/*txContext.txInfo?.txtype*/} #{txContext.txInfo?.txid}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {
                        txContext.reloadingTx &&
                        <div className="d-flex justify-content-center">
                            <div className="spinner-border" role="status">
                                <span className="visually-hidden">Loading...</span>
                            </div>
                        </div>
                    }
                    <dl className="row">
                        <dt className="col-sm-3">Status</dt>
                        <dd className="col-sm-9">
                            {txContext.txInfo?.status}
                            &nbsp;
                            <a href="#" onClick={async (e) => {
                                e.preventDefault();
                                let ret = await txContext.reloadTx(txContext.txInfo.txid);
                                if (!ret.sucesso) {
                                    alert(ret.mensagemErro);
                                }
                            }}>(Refresh)</a>
                        </dd>
                        <dt className="col-sm-3">{
                            {
                                'btc': 'BTC Address',
                                'stx': 'STX Address'
                            }[txContext.txInfo?.sendercoin]
                        }</dt>
                        <dd className="col-sm-9"><a href={txContext.txInfo?.senderaddressurl} target="_blank">{txContext.txInfo?.senderaddress}</a></dd>
                        <dt className="col-sm-3">{
                            {
                                'btc': 'BTC TxID',
                                'stx': 'STX TxID'
                            }[txContext.txInfo?.sendercoin]
                        }</dt>
                        <dd className="col-sm-9"><a href={txContext.txInfo?.sendertxidurl} target="_blank">{txContext.txInfo?.sendertxid}</a></dd>
                        <dt className="col-sm-3">{
                            {
                                'btc': 'BTC Address',
                                'stx': 'STX Address'
                            }[txContext.txInfo?.receivercoin]
                        }</dt>
                        <dd className="col-sm-9"><a href={txContext.txInfo?.receiveraddressurl} target="_blank">{txContext.txInfo?.receiveraddress}</a></dd>
                        {
                            (txContext.txInfo?.receivertxid) &&
                            <>
                                <dt className="col-sm-3">{
                                    {
                                        'btc': 'BTC TxID',
                                        'stx': 'STX TxID'
                                    }[txContext.txInfo?.receivercoin]
                                }</dt>
                                <dd className="col-sm-9"><a href={txContext.txInfo?.receivertxidurl} target="_blank">{txContext.txInfo?.receivertxid}</a></dd>
                            </>
                        }
                        <dt className="col-sm-3">Amounts</dt>
                        <dd className="col-sm-9">{txContext.txInfo?.senderamount + " -> " + txContext.txInfo?.receiveramount}</dd>
                        <dt className="col-sm-3">Fees</dt>
                        <dd className="col-sm-9">{txContext.txInfo?.senderfee + " + " + txContext.txInfo?.receiverfee}</dd>
                        <dt className="col-sm-3">Dates</dt>
                        <dd className="col-sm-9">{
                            "Create at " + txContext.txInfo?.createat + ", latest udpate at " + txContext.txInfo?.updateat
                        }</dd>
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
                                                <td scope="col" style={{whiteSpace: "nowrap"}}>{log.date}</td>
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
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => { setShowModal(false) }}>
                        Close
                    </Button>
                </Modal.Footer>
            </Modal>
            <Row>
                <Col md="8" className='offset-md-2'>
                    <Card>
                        <Card.Body>
                            <h1 className="text-center">Latest Swaps</h1>
                            <hr />
                            <Table striped bordered hover size="sm">
                                <thead>
                                    <tr>
                                        <th scope="col">Swap</th>
                                        <th scope="col">Wallet</th>
                                        <th scope="col">Latest Update</th>
                                        <th scope="col">Amount</th>
                                        <th scope="col">Status</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {
                                        txContext.loadingAllTxInfo &&
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
                                    {
                                        txContext.allTxInfo &&
                                        txContext.allTxInfo.map((item) => {
                                            let userAddr = item.senderaddress;
                                            let userView = userAddr.substr(0, 6) + '...' + userAddr.substr(-4);
                                            return (

                                                <tr>
                                                    <td scope="col"><a href="#" onClick={(e) => { txClickHandler(e, item) }}>{
                                                        item.sendercoin.toUpperCase() + " to " + item.receivercoin.toUpperCase()
                                                    }</a></td>
                                                    <td scope="col"><a href="#" onClick={(e) => { txClickHandler(e, item) }}>{userView}</a></td>
                                                    <td scope="col"><a href="#" onClick={(e) => { txClickHandler(e, item) }}>{item.updateat}</a></td>
                                                    <td scope="col"><a href="#" onClick={(e) => { txClickHandler(e, item) }}>{
                                                        item.senderamount + " -> " + item.receiveramount
                                                    }</a></td>
                                                    <td scope="col"><a href="#" onClick={(e) => { txClickHandler(e, item) }}>{item.status}</a></td>
                                                </tr>
                                            )
                                        })
                                    }
                                </tbody>
                            </Table>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
}