export default interface TxInfo {
    txid: number;
    sendercoin: string;
    receivercoin: string;
    senderaddress: string;
    senderaddressurl: string;
    receiveraddress: string;
    receiveraddressurl: string;
    createat: string;
    updateat: string;
    status: string;
    sendertxid?: string;
    sendertxidurl?: string;
    receivertxid?: string;
    receivertxidurl?: string;
    senderfee?: string,
    receiverfee?: string,
    senderamount?: string,
    receiveramount?: string
  }