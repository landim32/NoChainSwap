import { useContext } from "react";
import Col from "react-bootstrap/esm/Col";
import Container from "react-bootstrap/esm/Container";
import Row from "react-bootstrap/esm/Row";
import Form from 'react-bootstrap/Form';
import AuthContext from "../../Contexts/Auth/AuthContext";
import Button from "react-bootstrap/esm/Button";
import Card from 'react-bootstrap/Card';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import InputGroup from 'react-bootstrap/InputGroup';
import { faBitcoinSign, faClose, faLock, faMailBulk, faSave, faTrash, faUser } from '@fortawesome/free-solid-svg-icons';
import Table from "react-bootstrap/esm/Table";
import { Link } from "react-router-dom";

export default function PasswordPage() {

    const authContext = useContext(AuthContext);

    return (
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
                                    Registration is not required to make swaps, but you can do so anyway to access your transaction history.
                                </div>
                                <Form.Group as={Row} className="mb-3">
                                    <Form.Label column sm="3">Email:</Form.Label>
                                    <Col sm="9">
                                        <InputGroup>
                                            <InputGroup.Text><FontAwesomeIcon icon={faUser} fixedWidth /></InputGroup.Text>
                                            <Form.Control type="email" readOnly={true} disabled={true} size="lg" value={authContext.sessionInfo?.email} onChange={(e) => {
                                                //_setName(e.target.value);
                                            }} />
                                        </InputGroup>
                                    </Col>
                                </Form.Group>
                                <Form.Group as={Row} className="mb-3">
                                    <Form.Label column sm="3">Old Password:</Form.Label>
                                    <Col sm="9">
                                        <InputGroup>
                                            <InputGroup.Text><FontAwesomeIcon icon={faLock} fixedWidth /></InputGroup.Text>
                                            <Form.Control type="password" size="lg" placeholder="Your old password" value={authContext.sessionInfo?.email} onChange={(e) => {
                                                //_setName(e.target.value);
                                            }} />
                                        </InputGroup>
                                    </Col>
                                </Form.Group>
                                <Form.Group as={Row} className="mb-3">
                                    <Form.Label column sm="3">New Password:</Form.Label>
                                    <Col sm="9">
                                        <InputGroup>
                                            <InputGroup.Text><FontAwesomeIcon icon={faLock} fixedWidth /></InputGroup.Text>
                                            <Form.Control type="password" size="lg" placeholder="Your new password" value={authContext.sessionInfo?.email} onChange={(e) => {
                                                //_setName(e.target.value);
                                            }} />
                                        </InputGroup>
                                    </Col>
                                </Form.Group>
                                <Form.Group as={Row} className="mb-4">
                                    <Form.Label column sm="3">Confirm Password:</Form.Label>
                                    <Col sm="9">
                                        <InputGroup>
                                            <InputGroup.Text><FontAwesomeIcon icon={faLock} fixedWidth /></InputGroup.Text>
                                            <Form.Control type="password" size="lg" placeholder="Confirm you new password" value={authContext.sessionInfo?.email} onChange={(e) => {
                                                //_setName(e.target.value);
                                            }} />
                                        </InputGroup>
                                    </Col>
                                </Form.Group>
                                <div className="d-grid gap-2 d-md-flex justify-content-md-end">
                                    <Button variant="success" size="lg"><FontAwesomeIcon icon={faSave} fixedWidth /> Change Password</Button>
                                    <Button variant="danger" size="lg"><FontAwesomeIcon icon={faClose} fixedWidth /> Cancel</Button>
                                </div>
                            </Form>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
}