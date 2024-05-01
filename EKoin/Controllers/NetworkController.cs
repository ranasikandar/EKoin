using Library;
using Microsoft.AspNetCore.Mvc;
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
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly INodeRepo nodeRepo;

        public NetworkController(INodeRepo _nodeRepo)
        {
            nodeRepo = _nodeRepo;
        }


        [HttpPost("AnnonceNode")]//tp data is not include in signed data it can be manupulated by MITM
        public async Task<IActionResult> AnnonceNode(string pubkx, string derSign, bool isTP = false)
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

                Library.Wallet walletH = new Library.Wallet();
                bool verdata = walletH.VerifyData(pubKeyr, messageHashR, eCDSASignatureR);
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
                    string mypubkx = MySettings.GetValue("my_pubx", "myWallet.json");
                    Signature_Data_Hash signature_D_Hash = walletH.SignData(false, MySettings.GetValue("my_pkx", "myWallet.json"), mypubkx);
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

        [HttpGet("GetNodes")]
        public async Task<IActionResult> GetNodes(bool tp = false)
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

                Library.Wallet walletH = new Library.Wallet();
                Signature_Data_Hash signature_D_Hash = walletH.SignData(false, MySettings.GetValue("my_pkx", "myWallet.json"), netNodeData);

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

    }
}
