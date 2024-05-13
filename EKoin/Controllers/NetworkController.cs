using Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Models;
using Models.DB;
using Models.Network;
using Models.VModels;
using Models.VModels.Network;
using NBitcoin;
using NBitcoin.Crypto;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using static Models.Wallet;

namespace EKoin.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class NetworkController : ControllerBase
    {
        #region ctor

        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly INodeRepo nodeRepo;
        private readonly ILibraryWallet libraryWallet;
        private readonly IMySettings mySettings;
        private readonly ILedgerRepo ledgerRepo;
        private readonly IMemoryCache memoryCache;
        private readonly IBalanceRepo balanceRepo;
        private readonly IConfiguration configuration;
        
        public NetworkController(INodeRepo _nodeRepo, ILibraryWallet _libraryWallet, IMySettings _mySettings, ILedgerRepo _ledgerRepo
            , IMemoryCache _memoryCache, IBalanceRepo _balanceRepo, IConfiguration _configuration)
        {
            nodeRepo = _nodeRepo;
            libraryWallet = _libraryWallet;
            mySettings = _mySettings;
            ledgerRepo = _ledgerRepo;
            memoryCache = _memoryCache;
            balanceRepo = _balanceRepo;
            configuration = _configuration;
        }

        #endregion

        [HttpPost("RememberMe")]
        public async Task<IActionResult> RememberMe(RememberMe model)
        {
            try
            {
                //verify remote node pubk with dersign
                PubKey pubKeyr = new PubKey(model.pubkx);
                ECDSASignature eCDSASignatureR = new ECDSASignature(Convert.FromBase64String(model.derSign));

                // Convert the message to a byte array
                byte[] messageBytesR = System.Text.Encoding.UTF8.GetBytes(model.pubkx);
                // Compute the hash of the message
                uint256 messageHashR = Hashes.DoubleSHA256(messageBytesR);

                bool verdata = libraryWallet.VerifyData(pubKeyr, messageHashR, eCDSASignatureR);
                //verify remote node pubk with dersign

                if (verdata)
                {
                    //save node in db
                    //get ip addr
                    IPAddress remoteIpAddress = new IPAddress(0);
                    int remotePort = 0;
                    remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
                    remotePort = Request.HttpContext.Connection.RemotePort;

                    //compare in db
                    List<NetworkNode> networkNodes = await nodeRepo.GetNetworkNodes(model.pubkx, remoteIpAddress.ToString());

                    //del prior nodes with pubkx or ip, for decenterlization 1 pubk must be on a ip and 1 ip with 1 pubk
                    await nodeRepo.DeleteNetworkNode(networkNodes);

                    //db save node
                    NetworkNode newNetworkNode = new NetworkNode
                    {
                        IP = remoteIpAddress.ToString(),
                        Pubkx = model.pubkx,
                        Port = remotePort,
                        IsTP = model.isTP
                    };
                    await nodeRepo.AddUpdateNetworkNode(newNetworkNode);


                    //sign my pubkx and send with dersign
                    string mypubkx = mySettings.GetValue("my_pubx", "myWallet.json");
                    Signature_Data_Hash signature_D_Hash = libraryWallet.SignData(false, mySettings.GetValue("my_pkx", "myWallet.json"), mypubkx);
                    return Ok(new { pubkx = mypubkx, derSign = signature_D_Hash.DerSign });
                }
                else
                {
                    return StatusCode(400, "verify signature fail");
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return StatusCode(500);
            }
        }

        [HttpPost("ForgetMe")]
        public async Task<IActionResult> ForgetMe(ForgetMe model)
        {
            try
            {
                PubKey pubKeyr = new PubKey(model.pubkx);
                ECDSASignature eCDSASignatureR = new ECDSASignature(Convert.FromBase64String(model.derSign));

                // Convert the message to a byte array
                byte[] messageBytesR = System.Text.Encoding.UTF8.GetBytes(model.pubkx);
                // Compute the hash of the message
                uint256 messageHashR = Hashes.DoubleSHA256(messageBytesR);

                bool verifydata = libraryWallet.VerifyData(pubKeyr, messageHashR, eCDSASignatureR);

                if (verifydata)
                {
                    //db remove node
                    IPAddress remoteIpAddress = new IPAddress(0);
                    remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
                    //compare in db
                    List<NetworkNode> networkNodes = await nodeRepo.GetNetworkNodes(model.pubkx, remoteIpAddress.ToString());
                    //del any nodes with pubkx or ip, for decenterlization 1 pubk must be on a ip and 1 ip with 1 pubk
                    bool deleted=await nodeRepo.DeleteNetworkNode(networkNodes);

                    return Ok(deleted);
                }
                else
                {
                    return StatusCode(400, "verify signature fail");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return StatusCode(500);
            }
        }

        [HttpGet("GetNodes")]
        public async Task<IActionResult> GetNodes(GetNodes model)
        {
            try
            {
                List<NetworkNode> networkNodesDb = await nodeRepo.GetNetworkNodes(0, model.tp);
                
                NetworkNodeM networkNodeVM = new();
                foreach (NetworkNode node in networkNodesDb)
                {
                    NetNodes nodeVM = new();
                    nodeVM.pubkx = node.Pubkx;
                    nodeVM.ip = node.IP;
                    nodeVM.port = node.Port;
                    nodeVM.isTP = node.IsTP;

                    networkNodeVM.Nodes.Add(nodeVM);
                }

                //List<NetworkNodeVM> netNodes = (List<NetworkNodeVM>)networkNodes.Select(x => new { x.Pubkx, x.IP, x.Port, x.IsTP });
                //string networknodedata = JsonSerializer.Serialize(networkNodes.Select(x => new { x.Pubkx, x.IP, x.Port, x.IsTP }));

                string netNodeData = JsonSerializer.Serialize(networkNodeVM.Nodes);

                Signature_Data_Hash signature_D_Hash = libraryWallet.SignData(false, mySettings.GetValue("my_pkx", "myWallet.json"), netNodeData);

                //return Ok(new { nodes= networkNodeVM, derSign =signature_D_Hash.DerSign});

                networkNodeVM.DerSign = signature_D_Hash.DerSign;
                return Ok(networkNodeVM);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return StatusCode(500);
            }
        }

        [HttpGet("MaxTID")]
        public async Task<IActionResult> MaxTID()
        {
            try
            {
                if (!memoryCache.TryGetValue("max_tid", out ulong maxTid))
                {
                    maxTid = await ledgerRepo.GetMaxTID();
                    maxTid =mySettings.GetSetCache("max_tid", maxTid);
                }

                return Ok(maxTid);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return StatusCode(500);
            }
        }

        [HttpGet("GetBalance")]
        public async Task<IActionResult> GetBalance(AddressBalance address)
        {
            try
            {
                return Ok(await balanceRepo.GetBalance(address.Address));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return StatusCode(500);
            }
            
        }

        [HttpGet("Sync")]
        public async Task<IActionResult> Sync(Sync model)
        {
            try
            {
                List<Ledger> ledgerlist = await ledgerRepo.GetLedger(model.fromLedgerId);
                //todo gen csv file zip it and send
                return Ok(ledgerlist);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return StatusCode(500);
            }
            
        }
        
        [HttpPost("SubmitTransaction")]
        public async Task<IActionResult> SubmitTransaction(SubmitTransaction submitTransaction)
        {
            string outPutMsg = string.Empty;

            try
            {
                //check if valid transaction
                //check sign time
                //if ((DateTime.UtcNow - submitTransaction.TransactionInitTime).TotalMilliseconds < Convert.ToDouble(configuration["TransactionTimePeriodMSec"]))
                if (isTransactionTimePeriodValid(submitTransaction.TransactionInitTime))
                {
                    //check signature
                    byte[] signData = Library.Utility.TtoByteArray(new { submitTransaction.Reciver, submitTransaction.Amount,submitTransaction.Memo, submitTransaction.TransactionInitTime });
                    PubKey senderPubK = new PubKey(submitTransaction.SenderPubkx);
                    uint256 dataHash = Hashes.DoubleSHA256(signData);
                    ECDSASignature eCDSASignature = new ECDSASignature(Convert.FromBase64String(submitTransaction.DerSign));

                    bool verifyData = libraryWallet.VerifyData(senderPubK, dataHash, eCDSASignature);

                    if (verifyData)
                    {
                        //check balance 1st step
                        decimal senderBalance = await balanceRepo.GetBalance(senderPubK.GetAddress(ScriptPubKeyType.Legacy, Network.Main).ToString());
                        if (senderBalance >= submitTransaction.Amount)
                        {
                            //check balance 2nd step OR let TP take care for detail balance check OR datail balance is also checek in nodes at ledger add time every 
                            //TODO propogate to TP, wait for response from tp, if tp respond with 200 and tid, send tid
                            
                            return Ok();
                        }
                        else
                        {
                            outPutMsg += "check 1, Low Balance";
                        }
                    }
                    else
                    {
                        outPutMsg += "Signature Verification Fail";
                    }
                }
                else
                {
                    outPutMsg += "Sign Time Epired";
                }

                return StatusCode(500,outPutMsg);


            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return StatusCode(500);
            }

            
        }

        private bool isTransactionTimePeriodValid(DateTime dateTime)
        {
            double diffMSec = (DateTime.UtcNow - dateTime).TotalMilliseconds;

            if (diffMSec > 0)
            {
                if (diffMSec < Convert.ToDouble(configuration["TransactionTimePeriodMSec"]))
                {
                    return true;
                }
            }

            return false;
        }


    }
}
