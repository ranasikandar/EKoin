using EKoin.Utility;
using Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Models;
using Models.DB;
using Models.VModels;
using NBitcoin;
using NBitcoin.Crypto;
using NLog;
using System;
using System.Collections.Generic;
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

        public WalletController(ILibraryWallet _libraryWallet, IMySettings _mySettings, IMemoryCache _memoryCache,ILedgerRepo _ledgerRepo)
        {
            libraryWallet = _libraryWallet;
            mySettings = _mySettings;
            memoryCache = _memoryCache;
            ledgerRepo = _ledgerRepo;
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
        public IActionResult Genrate(string Mnemonic12Words)
        {
            try
            {
                Key_Pair wallet = libraryWallet.GenPubPkFromMnemonic(Mnemonic12Words);
                return Ok(new { pkx = wallet.PrivateKey_Hex, pubx = wallet.PublicKey_Hex, addr = wallet.Address_String, mnem = wallet.Mnemonic_12_Words });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

        [HttpPost("OverWrite")]
        public IActionResult OverWrite(string Mnemonic12Words)
        {
            try
            {
                Key_Pair wallet = libraryWallet.GenPubPkFromMnemonic(Mnemonic12Words);

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
        public IActionResult SignData(bool isBase58OtherwiseHEX, string privateKey, string @data)
        {
            try
            {
                Signature_Data_Hash signature_D_Hash = libraryWallet.SignData(isBase58OtherwiseHEX, privateKey, data);

                return Ok(signature_D_Hash);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

        [HttpPost("VerifyData")]
        public IActionResult VerifyData(string pubx, string dHash, string derSign)
        {
            try
            {
                PubKey pubKey = new PubKey(pubx);
                NBitcoin.uint256 uint_256 = new uint256(Convert.FromBase64String(dHash));
                ECDSASignature eCDSASignature = new ECDSASignature(Convert.FromBase64String(derSign));

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
        public IActionResult VerifyDataSignature(string @data, string derSign)
        {
            try
            {
                PubKey pubKey = new PubKey(mySettings.GetValue("my_pubx", "myWallet.json"));
                
                // Convert the message to a byte array
                byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(@data);
                // Compute the hash of the message
                uint256 uint_256 = Hashes.DoubleSHA256(messageBytes);

                ECDSASignature eCDSASignature = new ECDSASignature(Convert.FromBase64String(derSign));

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
        public IActionResult GenSHA256Hash(string @data)
        {
            try
            {
                return Ok(libraryWallet.GenSHA256Hash(@data).ToBytes(true));
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

        [HttpPost("SignMyData")]
        public IActionResult SignMyData(string @data)
        {
            try
            {
                Signature_Data_Hash signature_D_Hash=new Signature_Data_Hash();

                if (memoryCache.TryGetValue("myPrivateKey", out Key value))
                {
                    signature_D_Hash = libraryWallet.SignData(value as Key, data);
                }
                else
                {
                    signature_D_Hash = libraryWallet.SignData(false, mySettings.GetValue("my_pkx", "myWallet.json"), data);
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
            string address;

            try
            {
                BitcoinAddress _address= BitcoinAddress.Create(sendCoin.Reciver, Network.Main);
                address = _address.ToString();
            }
            catch (Exception)
            {
                address = sendCoin.Reciver;
                //return StatusCode(500,"Invalid Address:"+ex.Message);
            }

            try
            {
                Ledger ledger = new Ledger
                {
                    TID = await ledgerRepo.GetMaxTID() + 1,
                    LHash = Library.Utility.GetMd5Hash(Guid.NewGuid().ToString()),
                    Sender = mySettings.GetValue("my_addr", "myWallet.json"),
                    Reciver = address,
                    Amount=sendCoin.Amount,
                    Memo=sendCoin.Memo,
                    TransactionTime=DateTime.UtcNow,
                    
                };

                Signature_Data_Hash signature_D_Hash = new Signature_Data_Hash();

                if (memoryCache.TryGetValue("myPrivateKey", out Key value))
                {
                    signature_D_Hash = libraryWallet.SignData(value as Key, Library.Utility.TtoByteArray(ledger));
                }
                else
                {
                    signature_D_Hash = libraryWallet.SignData(false, mySettings.GetValue("my_pkx", "myWallet.json"), Library.Utility.TtoByteArray(ledger));
                }

                ledger.Signature = signature_D_Hash.DerSign;
                ledger.Hash = Library.Utility.GetMd5Hash(ledger);

                ledger=await ledgerRepo.AddLedger(ledger);

                List<Ledger> x = await ledgerRepo.GetLedger(1);

                return Ok(ledger);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

    }
}
