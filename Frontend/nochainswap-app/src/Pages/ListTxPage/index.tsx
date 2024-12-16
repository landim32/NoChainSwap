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
import { useNavigate, useParams } from 'react-router-dom';
import ProviderResult from '../../DTO/Contexts/ProviderResult';
import MessageToast from '../../Components/MessageToast';
import { MessageToastEnum } from '../../DTO/Enum/MessageToastEnum';

interface IListTxParam {
    OnlyMyTx: boolean
};

export default function ListTxPage(param: IListTxParam) {

    let navigate = useNavigate();

    const [dialog, setDialog] = useState<MessageToastEnum>(MessageToastEnum.Error);
    const [showMessage, setShowMessage] = useState<boolean>(false);
    const [messageText, setMessageText] = useState<string>("");

    const [onlyMyTx, setOnlyMyTx] = useState<boolean>(param.OnlyMyTx);

    const txContext = useContext(TxContext);

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

    useEffect(() => {
        if (onlyMyTx) {
            txContext.loadListMyTx().then((ret) => {
                if (!ret.sucesso) {
                    throwError(ret.mensagemErro);
                }
            });
        }
        else {
            txContext.loadListAllTx().then((ret) => {
                if (!ret.sucesso) {
                    throwError(ret.mensagemErro);
                }
            });
        }
    }, []);

    const txClickHandler = (e: any, item: TxInfo) => {
        e.preventDefault();
        navigate("/tx/" + item.hash);
        /*
        txContext.setTxInfo(item);
        txContext.loadTxLogs(item.txid).then((ret) => {
            if (!ret.sucesso) {
                alert(ret.mensagemErro);
            }
        });
        setShowModal(true);
        */
        //alert("hello");
    };

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
                    <Col md="12">
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
                                            txContext.loadingTxInfoList &&
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
                                            txContext.txInfoList &&
                                            txContext.txInfoList.map((item) => {
                                                //let userAddr = item.senderaddress;
                                                //let userView = userAddr.substr(0, 6) + '...' + userAddr.substr(-4);
                                                return (

                                                    <tr>
                                                        <td scope="col" style={{ whiteSpace: "nowrap" }}><a href="#" onClick={(e) => { txClickHandler(e, item) }}>{
                                                            item.sendercoin.toUpperCase() + " to " + item.receivercoin.toUpperCase()
                                                        }</a></td>
                                                        <td scope="col" style={{ whiteSpace: "nowrap" }}><a href="#" onClick={(e) => { txClickHandler(e, item) }}>{item.username}</a></td>
                                                        <td scope="col" style={{ whiteSpace: "nowrap" }}><a href="#" onClick={(e) => { txClickHandler(e, item) }}>{item.updateat}</a></td>
                                                        <td scope="col" style={{ whiteSpace: "nowrap" }}><a href="#" onClick={(e) => { txClickHandler(e, item) }}>{
                                                            item.senderamount + " -> " + item.receiveramount
                                                        }</a></td>
                                                        <td scope="col"><a href="#" onClick={(e) => { txClickHandler(e, item) }}>{txContext.getStatus(item.status)}</a></td>
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
        </>
    );
}