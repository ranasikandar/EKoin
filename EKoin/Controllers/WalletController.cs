using Library;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using NBitcoin.Crypto;
using NLog;
using System;
using static Models.Wallet;

namespace EKoin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [RestrictToLocalhost]
    public class WalletController : ControllerBase
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ILibraryWallet libraryWallet;
        private readonly IMySettings mySettings;
        public WalletController(ILibraryWallet _libraryWallet, IMySettings _mySettings)
        {
            libraryWallet = _libraryWallet;
            mySettings = _mySettings;
        }

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
                Signature_Data_Hash signature_D_Hash = libraryWallet.SignData(false, mySettings.GetValue("my_pkx", "myWallet.json"), data);

                return Ok(signature_D_Hash);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

    }
}
