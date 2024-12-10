export default interface TxInfo {
    txid: number;
    sendercoin: string;
    receivercoin: string;
    recipientaddress: string;
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
    senderfee?: number,
    receiverfee?: number,
    senderamount?: number,
    receiveramount?: number
  }