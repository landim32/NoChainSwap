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
    public class BtcTxService : CoinTxService, IBtcTxService
    {
        private const string MNEMONIC = "aunt federal magic they culture car primary maple snack misery dumb force three erosion vendor chair just twice blade front unhappy miss inject under";

        protected IMempoolService _mempoolService;
        private MemPoolTxInfo _memPoolTxInfo;

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

        private async Task<MemPoolTxInfo> GetCurrentTxMemPool(string txid) { 
            if (_memPoolTxInfo == null || string.Compare(_memPoolTxInfo.TxId, txid, true) != 0)
            {
                _memPoolTxInfo = await _mempoolService.GetTransaction(txid);
            }
            return _memPoolTxInfo;
        }

        public override CoinEnum GetCoin() {
            return CoinEnum.Bitcoin;
        }

        private BitcoinSecret GetBitcoinPrivatekey()
        {
            Mnemonic mnemo = new Mnemonic(MNEMONIC);
            var extKey = mnemo.DeriveExtKey();
            return extKey.PrivateKey.GetBitcoinSecret(Network.TestNet);
        }

        public override Task<string> GetNewAddress(int index)
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

        public override async Task<string> GetPoolAddress()
        {
            var bitcoinSecret = GetBitcoinPrivatekey();
            var address = bitcoinSecret.GetAddress(ScriptPubKeyType.Segwit);
            return await Task.FromResult(address.ToString()); 
        }

        public override async Task<long> GetPoolBalance()
        {
            var poolAddress = await GetPoolAddress();
            return await _mempoolService.GetBalance(poolAddress);
        }

        public override string GetAddressUrl(string address)
        {
            return $"https://mempool.space/testnet/address/{address}";
        }

        public override string GetTransactionUrl(string txId)
        {
            return $"https://mempool.space/testnet/tx/{txId}";
        }

        public override string ConvertToString(decimal coin)
        {
            return (coin / 100000000M).ToString("N5") + " BTC";
        }

        public override async Task<long> GetSenderAmount(string txid, string senderAddr)
        {
            var mempoolTx = await GetCurrentTxMemPool(txid);
            if (mempoolTx == null)
            {
                throw new Exception($"Dont find transaction on mempool ({txid})");
            }
            var amount = mempoolTx.VOut.Where(x => x.ScriptPubKeyAddress == senderAddr).Select(x => x.Value).Sum();
            return await Task.FromResult(amount);
        }

        public override async Task<int> GetFee(string txid)
        {
            var mempoolTx = await GetCurrentTxMemPool(txid);
            if (mempoolTx == null)
            {
                throw new Exception($"Dont find transaction on mempool ({txid})");
            }
            return await Task.FromResult(mempoolTx.Fee);
        }

        public override async Task<bool> IsTransactionSuccessful(string txid)
        {
            var mempoolTx = await GetCurrentTxMemPool(txid);
            if (mempoolTx == null)
            {
                throw new Exception($"Dont find transaction on mempool ({txid})");
            }
            return await Task.FromResult(mempoolTx.Status.Confirmed);
        }

        public override string GetSwapDescription(decimal proportion)
        {
            return "";
        }

        public override async Task<bool> VerifyTransaction(ITransactionModel tx)
        {
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
            return await Task.FromResult(true);
        }

        public async override Task<string> Transfer(string address, long amount)
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
    }
}
