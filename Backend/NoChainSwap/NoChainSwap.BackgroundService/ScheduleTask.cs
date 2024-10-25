using System;
using System.Linq;
using System.Threading.Tasks;
using NoChainSwap.Domain.Interfaces.Services;

namespace NoChainSwap.BackgroundService
{
    public class ScheduleTask
    {
        private ITransactionService _transactionService;

        public ScheduleTask(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<bool> ProccessAllTransactions()
        {
            bool ret = false;
            try
            {
                Console.WriteLine("Start processing all transactions");
                ret = await _transactionService.ProcessAllTransaction();
                Console.WriteLine("All transaction proccess terminated");
            } catch(Exception e)
            {
                Console.WriteLine("Error on proccessing transactions:\n" + e.Message);
            }
            return ret;
        }
    }
}
