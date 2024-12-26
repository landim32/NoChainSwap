export enum TransactionStatusEnum {
    Initialized = 1,
    Calculated = 2,
    WaitingSenderPayment = 3,
    DetectedSenderPayment = 4,
    SenderNotConfirmed = 5,
    SenderConfirmed = 6,
    SenderConfirmedReiceiverPaymentWaiting = 7,
    SenderConfirmedReiceiverNotConfirmed = 8,
    Finished = 9,
    InvalidInformation = 10,
    CriticalError = 11,
    Canceled = 12
}