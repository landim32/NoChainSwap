export default interface TxInfo {
    txid: number;
    hash: string;
    username: string;
    sendercoin: string;
    receivercoin: string;
    recipientaddress: string;
    senderaddress: string;
    senderaddressurl: string;
    receiveraddress: string;
    receiveraddressurl: string;
    createat: string;
    updateat: string;
    status: number;
    sendertxid?: string;
    sendertxidurl?: string;
    receivertxid?: string;
    receivertxidurl?: string;
    sendertax?: number;
    receivertax?: number;
    senderfee?: number,
    receiverfee?: number,
    senderamount: string,
    receiveramount: string,
    receiverpayback: number
  }