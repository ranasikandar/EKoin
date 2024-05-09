using Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Models;
using Models.DB;
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
        public NetworkController(INodeRepo _nodeRepo, ILibraryWallet _libraryWallet, IMySettings _mySettings,ILedgerRepo _ledgerRepo, IMemoryCache _memoryCache)
        {
            nodeRepo = _nodeRepo;
            libraryWallet = _libraryWallet;
            mySettings = _mySettings;
            ledgerRepo = _ledgerRepo;
            memoryCache = _memoryCache;
        }

        #endregion

        [HttpPost("RememberMe")]
        public async Task<IActionResult> RememberMe(string pubkx, string derSign, bool isTP = true)
        {
            try
            {
                //verify remote node pubk with dersign
                PubKey pubKeyr = new PubKey(pubkx);
                ECDSASignature eCDSASignatureR = new ECDSASignature(Convert.FromBase64String(derSign));

                // Convert the message to a byte array
                byte[] messageBytesR = System.Text.Encoding.UTF8.GetBytes(pubkx);
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
                    List<NetworkNode> networkNodes = await nodeRepo.GetNetworkNodes(pubkx, remoteIpAddress.ToString());

                    //del prior nodes with pubkx or ip, for decenterlization 1 pubk must be on a ip and 1 ip with 1 pubk
                    await nodeRepo.DeleteNetworkNode(networkNodes);

                    //db save node
                    NetworkNode newNetworkNode = new NetworkNode
                    {
                        IP = remoteIpAddress.ToString(),
                        Pubkx = pubkx,
                        Port = remotePort,
                        IsTP = isTP
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
        public async Task<IActionResult> ForgetMe(string pubkx, string derSign)
        {
            try
            {
                PubKey pubKeyr = new PubKey(pubkx);
                ECDSASignature eCDSASignatureR = new ECDSASignature(Convert.FromBase64String(derSign));

                // Convert the message to a byte array
                byte[] messageBytesR = System.Text.Encoding.UTF8.GetBytes(pubkx);
                // Compute the hash of the message
                uint256 messageHashR = Hashes.DoubleSHA256(messageBytesR);

                bool verifydata = libraryWallet.VerifyData(pubKeyr, messageHashR, eCDSASignatureR);

                if (verifydata)
                {
                    //db remove node
                    IPAddress remoteIpAddress = new IPAddress(0);
                    remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
                    //compare in db
                    List<NetworkNode> networkNodes = await nodeRepo.GetNetworkNodes(pubkx, remoteIpAddress.ToString());
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
        public async Task<IActionResult> GetNodes(bool tp = true)
        {
            try
            {
                List<NetworkNode> networkNodesDb = await nodeRepo.GetNetworkNodes(0, tp);
                
                NetworkNodeVM networkNodeVM = new();
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

        [HttpGet("Sync")]
        public async Task<IActionResult> Sync(ulong fromLedgerId)
        {
            List<Ledger> ledgerlist = await ledgerRepo.GetLedger(fromLedgerId);
            return Ok();
        }

    }
}
