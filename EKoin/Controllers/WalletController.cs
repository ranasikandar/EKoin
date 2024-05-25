using EKoin.Utility;
using Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Models;
using Models.DB;
using Models.Network;
using Models.VModels;
using Models.VModels.Wallet;
using NBitcoin;
using NBitcoin.Crypto;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Models.Wallet;

namespace EKoin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [RestrictToLocalhost]
    public class WalletController : ControllerBase
    {
        #region ctor

        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ILibraryWallet libraryWallet;
        private readonly IMySettings mySettings;
        private readonly IMemoryCache memoryCache;
        private readonly ILedgerRepo ledgerRepo;
        private readonly IConfiguration configuration;
        private readonly IHttpRequest httpRequest;

        public WalletController(ILibraryWallet _libraryWallet, IMySettings _mySettings, IMemoryCache _memoryCache,ILedgerRepo _ledgerRepo
            , IConfiguration _configuration, IHttpRequest _httpRequest)
        {
            libraryWallet = _libraryWallet;
            mySettings = _mySettings;
            memoryCache = _memoryCache;
            ledgerRepo = _ledgerRepo;
            configuration = _configuration;
            httpRequest = _httpRequest;
        }

        #endregion

        [HttpGet("New")]
        public IActionResult New()
        {
            try
            {
                Key_Pair wallet = libraryWallet.GenPubPk();
                return Ok(new { pkx = wallet.PrivateKey_Hex, pubx = wallet.PublicKey_Hex, addr = wallet.Address_String, mnem = wallet.Mnemonic_12_Words });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

        [HttpGet("MyPubK")]
        public IActionResult MyPubK()
        {
            try
            {
                string myPubk = mySettings.GetValue("my_pubx", "myWallet.json");
                return Ok(myPubk);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

        [HttpGet("MyAddr")]
        public IActionResult MyAddr()
        {
            try
            {
                string myAddr = mySettings.GetValue("my_addr", "myWallet.json");
                return Ok(myAddr);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("Genrate")]
        public IActionResult Genrate(Mnemonic12Words model)
        {
            try
            {
                Key_Pair wallet = libraryWallet.GenPubPkFromMnemonic(model.mnemonic12Words);
                return Ok(new { pkx = wallet.PrivateKey_Hex, pubx = wallet.PublicKey_Hex, addr = wallet.Address_String, mnem = wallet.Mnemonic_12_Words });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

        [HttpPost("OverWrite")]
        public IActionResult OverWrite(Mnemonic12Words model)
        {
            try
            {
                Key_Pair wallet = libraryWallet.GenPubPkFromMnemonic(model.mnemonic12Words);

                mySettings.SetValue(new string[] { "my_pkx", "my_pubx", "my_addr", "my_mnem" }
                , new string[] { wallet.PrivateKey_Hex, wallet.PublicKey_Hex, wallet.Address_String, wallet.Mnemonic_12_Words }, "myWallet.json");

                return Ok(new { pubx = wallet.PublicKey_Hex, addr = wallet.Address_String });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

        [HttpPost("SignData")]
        public IActionResult SignData(SignData model)
        {
            try
            {
                Signature_Data_Hash signature_D_Hash = libraryWallet.SignData(model.isBase58OtherwiseHEX, model.privateKey, model.data);

                return Ok(signature_D_Hash);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

        [HttpPost("VerifyData")]
        public IActionResult VerifyData(VerifyData model)
        {
            try
            {
                PubKey pubKey = new PubKey(model.pubx);
                NBitcoin.uint256 uint_256 = new uint256(Convert.FromBase64String(model.dHash));
                ECDSASignature eCDSASignature = new ECDSASignature(Convert.FromBase64String(model.derSign));

                bool verdata = libraryWallet.VerifyData(pubKey, uint_256, eCDSASignature);
                return Ok(verdata);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

        [HttpPost("VerifyDataSignature")]
        public IActionResult VerifyDataSignature(VerifyDataSignature model)
        {
            try
            {
                PubKey pubKey = new PubKey(mySettings.GetValue("my_pubx", "myWallet.json"));
                
                // Convert the message to a byte array
                byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(model.data);
                // Compute the hash of the message
                uint256 uint_256 = Hashes.DoubleSHA256(messageBytes);

                ECDSASignature eCDSASignature = new ECDSASignature(Convert.FromBase64String(model.derSign));

                bool verdata = libraryWallet.VerifyData(pubKey, uint_256, eCDSASignature);
                return Ok(verdata);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

        [HttpPost("GenSHA256Hash")]
        public IActionResult GenSHA256Hash(GenSHA256Hash model)
        {
            try
            {
                return Ok(libraryWallet.GenSHA256Hash(model.data).ToBytes(true));
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

        [HttpPost("SignMyData")]
        public IActionResult SignMyData(SignMyData model)
        {
            try
            {
                Signature_Data_Hash signature_D_Hash=new Signature_Data_Hash();

                if (memoryCache.TryGetValue("myPrivateKey", out Key value))
                {
                    signature_D_Hash = libraryWallet.SignData(value as Key, model.data);
                }
                else
                {
                    signature_D_Hash = libraryWallet.SignData(false, mySettings.GetValue("my_pkx", "myWallet.json"), model.data);
                }
                return Ok(signature_D_Hash);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

        [HttpPost("SendCoin")]//submit transaction, init transaction
        public async Task<IActionResult> SendCoin(SendCoin sendCoin)
        {
            ////test

            ////"http://127.0.0.1:45997/Wallet/Genrate"

            //SendCoin sendCoin1 = new SendCoin();
            //sendCoin1.Reciver = "1DMijhSj7gAxYpe5MSXAbJ6geTZ1owa9jz";
            //sendCoin1.Amount = 0.001M;
            //sendCoin1.Memo = "hello world";
            //await httpRequest.PostData(sendCoin1, "http://127.0.0.1:45997/Wallet/SendCoin");

            ///

            //VerifyDataSignature modal = new VerifyDataSignature() { data = _data, derSign = _derSign };
            //await httpRequest.PostData(modal, "http://127.0.0.1:45997/Wallet/VerifyDataSignature");

            //string _data = "023fe12030b0e899f290b017297a948f9dba6eac1295221b1585da097ea555fac5";
            //string _derSign = "MEQCIHeyLziXTBv60Q5gNBZ5gTJRuenbldJg9hgro0L/UWi7AiBgnXcM52bPlGl3HIbSfFJECE/Uh63WDXK6tTELdEToaA==";
            //var requestBody = $"{{ \"data\": \"{_data}\", \"derSign\": \"{_derSign}\" }}";
            //StringContent content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");
            //await httpRequest.PostData(content, "http://127.0.0.1:45997/Wallet/VerifyDataSignature");

            ////test

            string recAddress;

            try
            {
                BitcoinAddress _address = BitcoinAddress.Create(sendCoin.Reciver, Network.Main);
                recAddress = _address.ToString();
            }
            catch (Exception ex)
            {
                return StatusCode(500,"Invalid Address:"+ex.Message);
            }

            try
            {
                SubmitTransaction submitTransaction = new();
                submitTransaction.Reciver = recAddress;
                submitTransaction.Amount = sendCoin.Amount;
                submitTransaction.Memo = sendCoin.Memo;
                submitTransaction.SenderPubkx = mySettings.GetValue("my_pubx", "myWallet.json");
                submitTransaction.TransactionInitTime = DateTime.UtcNow;

                byte[] dataToBeSigned = Library.Utility.TtoByteArray(new { submitTransaction.Reciver, submitTransaction.Amount, submitTransaction.Memo, submitTransaction.TransactionInitTime });

                Signature_Data_Hash signature_D_Hash = new Signature_Data_Hash();

                if (memoryCache.TryGetValue("myPrivateKey", out Key value))
                {
                    signature_D_Hash = libraryWallet.SignData(value as Key, dataToBeSigned);
                }
                else
                {
                    signature_D_Hash = libraryWallet.SignData(false, mySettings.GetValue("my_pkx", "myWallet.json"), dataToBeSigned);
                }

                submitTransaction.DerSign = Convert.ToBase64String(signature_D_Hash.DerSign);

                ////test
                //bool validateData = libraryWallet.ValidateSubmitedTransaction(submitTransaction, Convert.ToDouble(configuration["TransactionTimePeriodMSec"]));
                //if (validateData)
                //{
                //    return Ok(submitTransaction);
                //}
                //else
                //{
                //    return StatusCode(500,submitTransaction);
                //}
                ////test

                //TODO propogate to TP, wait for response from tp, if tp respond with 200 and tid, send tid
                string response = await httpRequest.SubmitTransaction(submitTransaction, mySettings.GetValue("currentTP", "mySettings.json"));
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

            ////test
            //string address;

            //try
            //{
            //    BitcoinAddress _address= BitcoinAddress.Create(sendCoin.Reciver, Network.Main);
            //    address = _address.ToString();
            //}
            //catch (Exception)
            //{
            //    address = sendCoin.Reciver;
            //    //return StatusCode(500,"Invalid Address:"+ex.Message);
            //}

            //try
            //{
            //    Ledger ledger = new Ledger
            //    {
            //        TID = await ledgerRepo.GetMaxTID() + 1,
            //        LHash = Library.Utility.GetMd5Hash(Guid.NewGuid().ToString()),
            //        Sender = mySettings.GetValue("my_addr", "myWallet.json"),
            //        Reciver = address,
            //        Amount=sendCoin.Amount,
            //        Memo=sendCoin.Memo,
            //        TransactionTime=DateTime.UtcNow,

            //    };

            //    Signature_Data_Hash signature_D_Hash = new Signature_Data_Hash();

            //    if (memoryCache.TryGetValue("myPrivateKey", out Key value))
            //    {
            //        signature_D_Hash = libraryWallet.SignData(value as Key, Library.Utility.TtoByteArray(ledger));
            //    }
            //    else
            //    {
            //        signature_D_Hash = libraryWallet.SignData(false, mySettings.GetValue("my_pkx", "myWallet.json"), Library.Utility.TtoByteArray(ledger));
            //    }

            //    ledger.Signature = signature_D_Hash.DerSign;
            //    ledger.Hash = Library.Utility.GetMd5Hash(ledger);

            //    ledger=await ledgerRepo.AddLedger(ledger);

            //    return Ok(ledger);
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex.Message);
            //    return StatusCode(500);
            //}
            ////test

        }

        [HttpGet("test")]
        public IActionResult test()
        {
            string orgMsg = "Hello World! 7";

            //you
            Key_Pair youKeyPair = libraryWallet.GenPubPk();

            //me
            memoryCache.TryGetValue("myPrivateKey", out Key mykey);
            PubKey mypubKey = new PubKey(mySettings.GetValue("my_pubx", "myWallet.json"));

            byte[] dataBytes = Encoding.UTF8.GetBytes(orgMsg);
            byte[] encryptedData = youKeyPair.Public_Key.Encrypt(dataBytes);
            string encryptedDataBase64 = Convert.ToBase64String(encryptedData);
            string encryptedDataHex = Convert.ToHexString(encryptedData);
            //me

            //u
            byte[] receivedEncryptedData = Convert.FromBase64String(encryptedDataBase64);
            byte[] receivedEncryptedDataHex=Convert.FromHexString(encryptedDataHex);
            byte[] decryptedData = youKeyPair.Private_Key.Decrypt(receivedEncryptedData);
            string decryptedMessage = Encoding.UTF8.GetString(decryptedData);
            //u

            if (decryptedMessage==orgMsg)
            {
                return Ok();
            }
            else
            {
                return StatusCode(400);
            }

        }


    }
}
