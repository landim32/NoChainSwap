import Toast from 'react-bootstrap/Toast';
import ToastContainer from 'react-bootstrap/ToastContainer';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faWarning } from '@fortawesome/free-solid-svg-icons';
import { useState } from 'react';

interface IErrorToastParam {
    showError: boolean;
    messageError: string;
    onClose: () => void;
}

export default function ErrorToast(param: IErrorToastParam) {

    return (
        <ToastContainer
          className="p-3"
          position="bottom-end"
          style={{ zIndex: 9999 }}
        >
          <Toast animation={true} onClose={param.onClose} show={param.showError} delay={10000} autohide>
            <Toast.Header closeButton={true}>
              <FontAwesomeIcon icon={faWarning} style={{color: "red"}} />&nbsp;
              <strong className="me-auto">Error</strong>
            </Toast.Header>
            <Toast.Body>{param.messageError}</Toast.Body>
          </Toast>
        </ToastContainer>
    );
}