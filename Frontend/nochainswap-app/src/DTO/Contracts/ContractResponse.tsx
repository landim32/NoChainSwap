interface ContractResponse<T> {
    success: boolean;
    message: string;
    errors: any;
    data: T
  }
  
  export default ContractResponse;