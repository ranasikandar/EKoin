using Models.Network;
using NBitcoin;
using NBitcoin.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Wallet;

namespace Library
{
    public interface ILibraryWallet
    {
        Key_Pair GenPubPk();
        Key_Pair GenPubPkFromMnemonic(string mnemonic);
        Signature_Data_Hash SignData(bool isBase58OtherwiseHEX, string PK, string @data);
        Signature_Data_Hash SignData(bool isBase58OtherwiseHEX, string key, byte[] data);
        Signature_Data_Hash SignData(Key key, string @data);
        Signature_Data_Hash SignData(Key key, byte[] data);
        bool VerifyData(PubKey pubKey, uint256 messageHash, ECDSASignature signature);
        uint256 GenSHA256Hash(string @data);
        bool ValidateSubmitedTransaction(SubmitTransaction submitTransaction, double TransactionTimePeriodMSec);
        bool isTransactionTimePeriodValid(DateTime dateTime, double transactionTimePeriodMSec);
    }
}
