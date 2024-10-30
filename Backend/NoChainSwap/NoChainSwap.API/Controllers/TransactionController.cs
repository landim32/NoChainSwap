using NoChainSwap.API.DTO;
using NoChainSwap.Domain.Impl.Models;
using NoChainSwap.Domain.Impl.Services;
using NoChainSwap.Domain.Interfaces.Models;
using NoChainSwap.Domain.Interfaces.Services;
using NoChainSwap.DTO;
using NoChainSwap.DTO.CoinMarketCap;
using NoChainSwap.DTO.Transaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NoChainSwap.Domain.Interfaces.Factory;
using System.Net;

namespace NoChainSwap.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class TransactionController: Controller
    {
        private IUserService _userService;
        private ITransactionService _txService;
        private IStacksService _stacksService;
        protected readonly ICoinTxServiceFactory _coinFactory;

        public TransactionController(
            IUserService userService, 
            ITransactionService txService,
            IStacksService stacksService,
            ICoinTxServiceFactory coinFactory
        )
        {
            _userService = userService;
            _txService = txService;
            _stacksService = stacksService;
            _coinFactory = coinFactory;
        }

        private TxResult ModelToInfo(ITransactionModel md)
        {
            var senderTx = _coinFactory.BuildCoinTxService(md.SenderCoin);
            var receiverTx = _coinFactory.BuildCoinTxService(md.ReceiverCoin);
            return new TxResult
            {
                TxId = md.TxId,
                SenderCoin = md.GetSenderCoinSymbol(),
                ReceiverCoin = md.GetReceiverCoinSymbol(),
                SenderAddress = md.SenderAddress,
                SenderAddressUrl = (md.SenderAddress != null) ? senderTx.GetAddressUrl(md.SenderAddress) : null,
                ReceiverAddress = md.ReceiverAddress,
                ReceiverAddressUrl = (md.ReceiverAddress != null) ? receiverTx.GetAddressUrl(md.ReceiverAddress) : null,
                CreateAt = md.CreateAt.ToString("MM/dd HH:mm:ss"),
                UpdateAt = md.UpdateAt.ToString("MM/dd HH:mm:ss"),
                Status = TransactionService.GetTransactionEnumToString(md.Status),
                SenderTxid = md.SenderTxid,
                SenderTxidUrl = !string.IsNullOrEmpty(md.SenderTxid) ? senderTx.GetTransactionUrl(md.SenderTxid) : null,
                ReceiverTxid = md.ReceiverTxid,
                ReceiverTxidUrl = !string.IsNullOrEmpty(md.ReceiverTxid) ? receiverTx.GetTransactionUrl(md.ReceiverTxid) : null,
                SenderFee = md.SenderFee.HasValue ? senderTx.ConvertToString(md.SenderFee.Value) : null,
                ReceiverFee = md.ReceiverFee.HasValue ? _stacksService.ConvertToString(md.ReceiverFee.Value) : null,
                SenderAmount = md.SenderAmount.HasValue ? senderTx.ConvertToString(md.SenderAmount.Value) : null,
                ReceiverAmount = md.ReceiverAmount.HasValue ? _stacksService.ConvertToString(md.ReceiverAmount.Value) : null
            };
        }

        [HttpPost("createTx")]
        public ActionResult<bool> CreateTx([FromBody] TransactionParamInfo param)
        {
            try
            {
                /*
                var user = _userService.GetUserInSession(HttpContext);
                if (user == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                */
                var tx = _txService.CreateTx(param);
                
                return new ActionResult<bool>(tx.TxId > 0);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("listmytransactions/{address}")]
        public ActionResult<IList<TxResult>> ListMyTransactions(string address)
        {
            try
            {
                /*
                var user = _userService.GetUserInSession(HttpContext);
                if (user == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                */
                var ds = _txService.ListByAddress(address).Select(x => ModelToInfo(x)).ToList();
                return new ActionResult<IList<TxResult>>(ds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("listalltransactions")]
        public ActionResult<IList<TxResult>> ListAllTransactions()
        {
            try
            {
                /*
                var user = _userService.GetUserInSession(HttpContext);
                if (user == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                */
                var ds = _txService.ListAll().Select(x => ModelToInfo(x)).ToList();
                return new ActionResult<IList<TxResult>>(ds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private string GetLogTypeToStr(LogTypeEnum logType)
        {
            string str = string.Empty;
            switch (logType) {
                case LogTypeEnum.Information:
                    str = "info";
                    break;
                case LogTypeEnum.Warning:
                    str = "warning";
                    break;
                case LogTypeEnum.Error:
                    str = "danger";
                    break;
            }
            return str;
        }

        [HttpGet("listtransactionlog/{txid}")]
        public ActionResult<IList<TxLogResult>> ListTransactionLog(long txid)
        {
            try
            {
                /*
                var user = _userService.GetUserInSession(HttpContext);
                if (user == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                */
                var ds = _txService.ListLogById(txid).Select(x => new TxLogResult
                {
                    LogType = GetLogTypeToStr(x.LogType),
                    IntLogType = (int)x.LogType,
                    Date = x.Date.ToString("MM/dd HH:mm:ss"),
                    Message = x.Message
                }).ToList();
                return new ActionResult<IList<TxLogResult>>(ds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("gettransaction/{txid}")]
        public ActionResult<TxResult> GetTransaction(long txid)
        {
            try
            {
                /*
                var user = _userService.GetUserInSession(HttpContext);
                if (user == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                */
                return ModelToInfo(_txService.GetTx(txid));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("processtransaction/{txid}")]
        public async Task<ActionResult<bool>> ProcessTransaction(long txid)
        {
            try
            {
                /*
                var user = _userService.GetUserInSession(HttpContext);
                if (user == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                */
                var tx = _txService.GetTx(txid);
                if (tx == null)
                {
                    return StatusCode(500, $"Dont find transaction with ID {txid}");
                }
                return await _txService.ProcessTransaction(tx);
                //return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
