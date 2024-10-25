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

namespace NoChainSwap.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class TransactionController: Controller
    {
        private IUserService _userService;
        private ITransactionService _txService;
        private IBitcoinService _bitcoinService;
        private IStacksService _stacksService;

        public TransactionController(
            IUserService userService, 
            ITransactionService txService,
            IBitcoinService bitcoinService,
            IStacksService stacksService
        )
        {
            _userService = userService;
            _txService = txService;
            _bitcoinService = bitcoinService;
            _stacksService = stacksService;
        }

        private TxResult ModelToInfo(ITransactionModel md)
        {
            return new TxResult
            {
                TxId = md.TxId,
                IntType = (int)md.Type,
                TxType = md.Type == TransactionEnum.BtcToStx ? "BTC To STX" : "STX to BTC",
                BtcAddress = md.BtcAddress,
                BtcAddressUrl = (md.BtcAddress != null) ? _bitcoinService.GetAddressUrl(md.BtcAddress) : null,
                StxAddress = md.StxAddress,
                StxAddressUrl = (md.StxAddress != null) ? _stacksService.GetAddressUrl(md.StxAddress) : null,
                CreateAt = md.CreateAt.ToString("MM/dd/yyyy HH:mm:ss"),
                UpdateAt = md.UpdateAt.ToString("MM/dd/yyyy HH:mm:ss"),
                Status = _txService.GetTransactionEnumToString(md.Status),
                BtcTxid = md.BtcTxid,
                BtcTxidUrl = !string.IsNullOrEmpty(md.BtcTxid) ? _bitcoinService.GetTransactionUrl(md.BtcTxid) : null,
                StxTxid = md.StxTxid,
                StxTxidUrl = !string.IsNullOrEmpty(md.StxTxid) ? _stacksService.GetTransactionUrl(md.StxTxid) : null,
                BtcFee = md.BtcFee.HasValue ? _bitcoinService.ConvertToString(md.BtcFee.Value) : null,
                StxFee = md.StxFee.HasValue ? _stacksService.ConvertToString(md.StxFee.Value) : null,
                BtcAmount = md.BtcAmount.HasValue ? _bitcoinService.ConvertToString(md.BtcAmount.Value) : null,
                StxAmount = md.StxAmount.HasValue ? _stacksService.ConvertToString(md.StxAmount.Value) : null
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
                    Date = x.Date.ToString("MM/dd/yyyy HH:mm:ss"),
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
