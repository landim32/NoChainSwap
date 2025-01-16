using NBitcoin;
using NBitcoin.RPC;
using Nethereum.HdWallet;
using Newtonsoft.Json;
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
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NoChainSwap.Domain.Impl.Services.Coins
{
    public class BtcTxService : IBtcTxService
    {
        private const string MNEMONIC = "aunt federal magic they culture car primary maple snack misery dumb force three erosion vendor chair just twice blade front unhappy miss inject under";

        protected readonly ICoinMarketCapService _coinMarketCapService;
        protected readonly ITransactionDomainFactory _txFactory;
        protected readonly ITransactionLogDomainFactory _txLogFactory;
        protected readonly IMempoolService _mempoolService;
        //private MemPoolTxInfo _memPoolTxInfo;

        public BtcTxService(
            ICoinMarketCapService coinMarketCapService,
            IMempoolService mempoolService,
            ITransactionDomainFactory txFactory,
            ITransactionLogDomainFactory txLogFactory
        )
        {
            _coinMarketCapService = coinMarketCapService;
            _mempoolService = mempoolService;
            _txFactory = txFactory;
            _txLogFactory = txLogFactory;
        }

        public bool IsPaybackAutomatic()
        {
            return true;
        }

        public CoinEnum GetCoin() {
            return CoinEnum.Bitcoin;
        }

        private BitcoinSecret GetBitcoinPrivatekey()
        {
            Mnemonic mnemo = new Mnemonic(MNEMONIC);
            var extKey = mnemo.DeriveExtKey();
            return extKey.PrivateKey.GetBitcoinSecret(Network.TestNet);
        }

        public Task<string> GetNewAddress(int index)
        {
            Mnemonic mnemo = new Mnemonic(MNEMONIC);
            var extKey = mnemo.DeriveExtKey();
            var addr = extKey
                .Derive(0)
                .Derive(Convert.ToUInt32(index))
                .GetPublicKey()
                .GetAddress(ScriptPubKeyType.Segwit, Network.TestNet);
            return Task.FromResult(addr.ToString());
        }

        public async Task<string> GetPoolAddress()
        {
            var bitcoinSecret = GetBitcoinPrivatekey();
            var address = bitcoinSecret.GetAddress(ScriptPubKeyType.Segwit);
            return await Task.FromResult(address.ToString()); 
        }

        public async Task<long> GetPoolBalance()
        {
            var poolAddress = await GetPoolAddress();
            return await _mempoolService.GetBalance(poolAddress);
        }

        public string GetAddressUrl(string address)
        {
            return $"https://mempool.space/testnet/address/{address}";
        }

        public string GetTransactionUrl(string txId)
        {
            return $"https://mempool.space/testnet/tx/{txId}";
        }

        public string ConvertToString(decimal coin)
        {
            return (coin / 100000000M).ToString("N5") + " BTC";
        }

        public async Task<TxResumeInfo> GetResumeTransaction(string txId)
        {
            var mempoolTx = await _mempoolService.GetTransaction(txId);
            return new TxResumeInfo
            {
                //SenderAmount = mempoolTx.VOut.Where(x => x.ScriptPubKeyAddress == senderAddr).Select(x => x.Value).Sum();
                Amount = mempoolTx.VOut.Select(x => x.Value).Sum(),
                Fee = mempoolTx.Fee,
                TxId = txId,
                SenderAddress = mempoolTx.VOut.OrderByDescending(x => x.Value).Select(x => x.ScriptPubKeyAddress).FirstOrDefault(),
                Success = mempoolTx.Status.Confirmed
            };
        }

        /*
        public async Task<bool> IsTransactionSuccessful(string txid)
        {
            var mempoolTx = await GetCurrentTxMemPool(txid);
            if (mempoolTx == null)
            {
                throw new Exception($"Dont find transaction on mempool ({txid})");
            }
            return await Task.FromResult(mempoolTx.Status.Confirmed);
        }

        public async Task<long> GetSenderAmount(string txid, string senderAddr)
        {
            var mempoolTx = await GetCurrentTxMemPool(txid);
            if (mempoolTx == null)
            {
                throw new Exception($"Dont find transaction on mempool ({txid})");
            }
            var amount = mempoolTx.VOut.Where(x => x.ScriptPubKeyAddress == senderAddr).Select(x => x.Value).Sum();
            return await Task.FromResult(amount);
        }

        public async Task<int> GetFee(string txid)
        {
            var mempoolTx = await GetCurrentTxMemPool(txid);
            if (mempoolTx == null)
            {
                throw new Exception($"Dont find transaction on mempool ({txid})");
            }
            return await Task.FromResult(mempoolTx.Fee);
        }
        */

        public async Task<bool> VerifyTransaction(ITransactionModel tx)
        {
            if (string.IsNullOrEmpty(tx.SenderTxid) && string.IsNullOrEmpty(tx.ReceiverTxid))
            {
                AddLog(tx.TxId, "Transaction tx_id is empty", LogTypeEnum.Error, _txLogFactory);
                tx.Status = TransactionStatusEnum.InvalidInformation;
                tx.Update();
                return await Task.FromResult(false);
            }
            /*
            var mempoolTx = await GetCurrentTxMemPool(tx.SenderTxid);
            if (mempoolTx == null)
            {
                //throw new Exception("Dont find transaction on mempool");
                AddLog(tx.TxId, "Dont find transaction on mempool", LogTypeEnum.Warning, _txLogFactory);
                return await Task.FromResult(false);
            }
            var poolBtcAddr = await GetPoolAddress();

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
            */
            return await Task.FromResult(true);
        }

        public async Task<string> Transfer(string address, long amount)
        {
            var txFee = await _mempoolService.GetRecommendedFee();
            if (txFee == null) {
                throw new Exception("Recommended fee cant be null");
            }

            var bitcoinSecret = GetBitcoinPrivatekey();

            var poolAddress = bitcoinSecret.GetAddress(ScriptPubKeyType.Segwit);

            var poolBalance = await _mempoolService.GetBalance(poolAddress.ToString());

            Money nBalance = Money.Satoshis(poolBalance);
            Money nAmount = Money.Satoshis(amount);
            Money fee = Money.Satoshis(txFee.HourFee);

            BitcoinAddress receiverAddress = BitcoinAddress.Create(address, Network.TestNet);

            var utxos = await _mempoolService.ListUTXO(poolAddress.ToString());
            if (utxos == null || !utxos.Any())
            {
                throw new Exception("No available UTXOs for the pool address.");
            }

            var funding = Transaction.Create(Network.TestNet);
            //funding.Outputs.Add(new TxOut(nBalance, poolAddress));

            //var coins = funding.Outputs.Select((i, v) => new Coin(new OutPoint(funding.GetHash(), v), i)).ToArray();
            var coins = utxos.Select(utxo => 
                new Coin(new OutPoint(uint256.Parse(utxo.Txid), utxo.Vout), new TxOut(Money.Satoshis(utxo.Value), poolAddress))
            ).ToArray();

            var txBuilder = Network.TestNet.CreateTransactionBuilder();
            var tx = txBuilder
                .AddCoins(coins)
                .AddKeys(bitcoinSecret.PrivateKey)
                .Send(receiverAddress, nAmount)
                .SendEstimatedFees(new FeeRate(fee))
                .SetChange(poolAddress)
                .BuildTransaction(true);
            if (!txBuilder.Verify(tx))
            {
                throw new Exception("Cant verify bitcoin transaction");
            }
            return await _mempoolService.BroadcastTransaction(tx.ToHex());
            
        }

        public Task<IList<TxDetectedInfo>> DetectNewTransactions(IList<string> addresses)
        {
            return Task.FromResult<IList<TxDetectedInfo>>(new List<TxDetectedInfo>());
        }

        public void AddLog(long txId, string msg, LogTypeEnum t, ITransactionLogDomainFactory txLogFactory)
        {
            var currentDate = DateTime.Now;
            Console.WriteLine(string.Format("{0} - [{1}] {2}", currentDate.ToString("yyyy-MM-dd h:mm:ss"), t.ToString(), msg));

            var md = txLogFactory.BuildTransactionLogModel();
            md.TxId = txId;
            md.Date = currentDate;
            md.LogType = t;
            md.Message = msg;
            md.Insert();
        }
    }
}
