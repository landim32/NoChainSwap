using NBitcoin;
using NoChainSwap.Domain.Impl.Models;
using NoChainSwap.Domain.Interfaces.Factory;
using NoChainSwap.Domain.Interfaces.Models;
using NoChainSwap.Domain.Interfaces.Services;
using NoChainSwap.Domain.Interfaces.Services.Coins;
using NoChainSwap.DTO.Mempool;
using NoChainSwap.DTO.Stacks;
using NoChainSwap.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Impl.Services.Coins
{
    public class BtcTxService : CoinTxService, IBtcTxService
    {
        private const string MNEMONIC = "aunt federal magic they culture car primary maple snack misery dumb force three erosion vendor chair just twice blade front unhappy miss inject under";

        public BtcTxService(
            ICoinMarketCapService coinMarketCapService,
            IMempoolService mempoolService,
            IStacksService stxService,
            ITransactionDomainFactory txFactory,
            ITransactionLogDomainFactory txLogFactory
        )
        {
            _coinMarketCapService = coinMarketCapService;
            _mempoolService = mempoolService;
            _stxService = stxService;
            _txFactory = txFactory;
            _txLogFactory = txLogFactory;
        }

        public override string GetSlug()
        {
            return "bitcoin";
        }

        public override string GetPoolAddress()
        {
            Mnemonic mnemo = new Mnemonic(MNEMONIC);
            var extKey = mnemo.DeriveExtKey();
            var bitcoinSecret = extKey.PrivateKey.GetBitcoinSecret(Network.TestNet);
            var address = bitcoinSecret.GetAddress(ScriptPubKeyType.Segwit);
            return address.ToString();
        }

        public override string GetAddressUrl(string address)
        {
            return $"https://mempool.space/testnet/address/{address}";
        }

        public override string GetTransactionUrl(string txId)
        {
            return $"https://mempool.space/testnet/tx/{txId}";
        }

        public override string ConvertToString(long coin)
        {
            return ((decimal)coin / 100000000M).ToString("N5") + " BTC";
        }

        public override long GetSenderAmount()
        {
            return 0;
        }
        public override decimal GetSenderProportion(ICoinTxService receiverService)
        {
            return 0M;
        }

        public override async Task<bool> VerifyTransaction(ITransactionModel tx)
        {
            var mempoolTx = await _mempoolService.GetTransaction(tx.SenderTxid);
            if (mempoolTx == null)
            {
                //throw new Exception("Dont find transaction on mempool");
                AddLog(tx.TxId, "Dont find transaction on mempool", LogTypeEnum.Warning, _txLogFactory);
                return await Task.FromResult(false);
            }
            var poolBtcAddr = GetPoolAddress();

            //var addrs = new List<string>();
            var addrsSender = mempoolTx.VIn.Select(x => x.Prevout.ScriptPubKeyAddress).ToList();
            var addrsReceiver = mempoolTx.VOut.Select(x => x.ScriptPubKeyAddress).ToList();

            if (!addrsSender.Contains(tx.SenderAddress))
            {
                AddLog(tx.TxId, "Sender address not in transaction", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(false);
            }

            if (!addrsReceiver.Contains(poolBtcAddr))
            {
                AddLog(tx.TxId, "Pool address not in transaction", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(false);
            }

            var poolBtcAmount = mempoolTx.VOut.Where(x => x.ScriptPubKeyAddress == poolBtcAddr).Select(x => x.Value).Sum();
            if (poolBtcAmount <= 0)
            {
                AddLog(tx.TxId, "Pool did not receive any value", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(false);
            }
            return await Task.FromResult(false);
        }
    }
}
