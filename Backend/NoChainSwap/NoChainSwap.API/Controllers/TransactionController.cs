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
using Newtonsoft.Json.Linq;

namespace NoChainSwap.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class TransactionController: Controller
    {
        private IUserService _userService;
        protected readonly IUserDomainFactory _userFactory;
        private ITransactionService _txService;
        protected readonly ICoinTxServiceFactory _coinFactory;

        public TransactionController(
            IUserService userService, 
            IUserDomainFactory userFactory,
            ITransactionService txService,
            ICoinTxServiceFactory coinFactory
        )
        {
            _userService = userService;
            _userFactory = userFactory;
            _txService = txService;
            _coinFactory = coinFactory;
        }

        private string GetUsername(ITransactionModel md)
        {
            var str = string.Empty;
            var user = md.GetUser(_userFactory);
            if (user != null)
            {
                str = user.Name;
                if (string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(user.Email))
                {
                    str = user.Email.Substring(0, user.Email.IndexOf('@'));
                }
            }
            if (string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(md.SenderAddress))
            {
                return md.SenderAddress.Substring(0, 4) + "..." + md.SenderAddress.Substring(-5);
            }
            if (string.IsNullOrEmpty(str))
            {
                str = "Anonymous";
            }
            return str;
        }

        private TxResult ModelToInfo(ITransactionModel md)
        {
            var senderTx = _coinFactory.BuildCoinTxService(md.SenderCoin);
            var receiverTx = _coinFactory.BuildCoinTxService(md.ReceiverCoin);
            return new TxResult
            {
                TxId = md.TxId,
                Hash = md.Hash,
                Username = GetUsername(md),
                SenderCoin = md.GetSenderCoinSymbol(),
                ReceiverCoin = md.GetReceiverCoinSymbol(),
                RecipientAddress = md.RecipientAddress,
                SenderAddress = md.SenderAddress,
                SenderAddressUrl = (md.SenderAddress != null) ? senderTx.GetAddressUrl(md.SenderAddress) : null,
                ReceiverAddress = md.ReceiverAddress,
                ReceiverAddressUrl = (md.ReceiverAddress != null) ? receiverTx.GetAddressUrl(md.ReceiverAddress) : null,
                CreateAt = md.CreateAt.ToString("MM/dd HH:mm:ss"),
                UpdateAt = md.UpdateAt.ToString("MM/dd HH:mm:ss"),
                //Status = TransactionService.GetTransactionEnumToString(md.Status),
                Status = (int)md.Status,
                SenderTxid = md.SenderTxid,
                SenderTxidUrl = !string.IsNullOrEmpty(md.SenderTxid) ? senderTx.GetTransactionUrl(md.SenderTxid) : null,
                ReceiverTxid = md.ReceiverTxid,
                ReceiverTxidUrl = !string.IsNullOrEmpty(md.ReceiverTxid) ? receiverTx.GetTransactionUrl(md.ReceiverTxid) : null,
                SenderFee = md.SenderFee.HasValue ? senderTx.ConvertToString(md.SenderFee.Value) : null,
                ReceiverFee = md.ReceiverFee.HasValue ? receiverTx.ConvertToString(md.ReceiverFee.Value) : null,
                SenderTax = md.SenderTax.HasValue ? senderTx.ConvertToString(md.SenderTax.Value) : null,
                ReceiverTax = md.ReceiverTax.HasValue ? receiverTx.ConvertToString(md.ReceiverTax.Value) : null,
                SenderAmount = senderTx.ConvertToString(md.SenderAmount),
                SenderAmountValue = md.SenderAmount,
                ReceiverAmount = receiverTx.ConvertToString(md.ReceiverAmount),
                ReceiverPayback = md.ReceiverAmount - md.ReceiverTax.GetValueOrDefault()
            };
        }

        [HttpPost("createTx")]
        public async Task<ActionResult<string>> CreateTx([FromBody] TransactionParamInfo param)
        {
            try
            {
                var tx = await _txService.CreateTx(param);
                await _txService.ProcessTransaction(tx);
                tx = _txService.GetById(tx.TxId);

                return new ActionResult<string>(tx.Hash);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("confirmsendpayment")]
        public ActionResult<bool> ConfirmSendPayment([FromBody] TxPaymentParam param)
        {
            try
            {
                _txService.ConfirmSendPayment(param.TxId, param.SenderTxId);
                return new ActionResult<bool>(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("confirmpayment/{txid}")]
        public ActionResult<bool> ConfirmPayment(long txid)
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.Id);
                if (user == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                if (!user.IsAdmin)
                {
                    return StatusCode(401, "Access Denied");
                }
                _txService.ConfirmPayment(txid);

                return new ActionResult<bool>(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("payback")]
        public ActionResult<bool> Payback([FromBody] TxPaybackParam param)
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.Id);
                if (user == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                if (!user.IsAdmin)
                {
                    return StatusCode(401, "Access Denied");
                }
                _txService.Payback(param.TxId, param.ReceiverTxId, param.ReceiverFee);

                return new ActionResult<bool>(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("changestatus")]
        public ActionResult<bool> ChangeStatus([FromBody] TxRevertStatusParam param) {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.Id);
                if (user == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                if (!user.IsAdmin)
                {
                    return StatusCode(401, "Access Denied");
                }

                var tx = _txService.GetById(param.TxId);
                if (tx == null) {
                    throw new Exception("Transaction not found");
                }
                _txService.ChangeStatus(param.TxId, (TransactionStatusEnum)param.Status, param.Message);
                return new ActionResult<bool>(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("listmytransactions")]
        public ActionResult<IList<TxResult>> ListMyTransactions()
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                var ds = _txService.ListByUser(userSession.Id).Select(x => ModelToInfo(x)).ToList();
                return new ActionResult<IList<TxResult>>(ds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("listalltransactions")]
        public ActionResult<IList<TxResult>> ListAllTransactions()
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.Id);
                if (user == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                if (!user.IsAdmin)
                {
                    return StatusCode(401, "Access Denied");
                }

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

        [HttpGet("gettransaction/{hash}")]
        public ActionResult<TxResult> GetTransaction(string hash)
        {
            try
            {
                return ModelToInfo(_txService.GetByHash(hash));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("processtransaction/{txid}")]
        public async Task<ActionResult<bool>> ProcessTransaction(long txid)
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.Id);
                if (user == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                if (!user.IsAdmin)
                {
                    return StatusCode(401, "Access Denied");
                }

                await _txService.DetectAllTransaction();
                var tx = _txService.GetById(txid);
                if (tx == null)
                {
                    return StatusCode(500, $"Dont find transaction with ID {txid}");
                }
                return await _txService.ProcessTransaction(tx);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
