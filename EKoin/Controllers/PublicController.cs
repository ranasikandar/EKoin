using Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NBitcoin;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Models.Wallet;

namespace EKoin.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class PublicController : ControllerBase
    {
        #region ctor

        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ILibraryWallet libraryWallet;
        private readonly IMySettings mySettings;
        private readonly IMemoryCache memoryCache;

        public PublicController(ILibraryWallet _libraryWallet, IMySettings _mySettings, IMemoryCache _memoryCache)
        {
            libraryWallet = _libraryWallet;
            mySettings = _mySettings;
            memoryCache = _memoryCache;
        }

        #endregion

        [HttpGet("Hi")]
        public IActionResult Hi()
        {
            return Ok(1);
        }

        //is this node transaction pool
        [HttpGet("Role")]
        public IActionResult Role()
        {
            return Ok(0);
        }

        [HttpGet("WhatIsMyIp")]
        public IActionResult WhatIsMyIp()
        {
            IPAddress remoteIpAddress = new IPAddress(0);
            int remotePort = 0;

            try
            {
                remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
                remotePort = Request.HttpContext.Connection.RemotePort;

                //if (remoteIpAddress != null)
                //{
                //    // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
                //    // This usually only happens when the browser is on the same machine as the server.
                //    if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                //    {
                //        remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList.First(x => x.AddressFamily ==
                //        System.Net.Sockets.AddressFamily.InterNetwork);
                //    }
                //}

                //IPAddress ip;
                //var headers = Request.Headers.ToList();
                //if (headers.Exists((kvp) => kvp.Key == "X-Forwarded-For")) //X_FORWARDED_FOR //REMOTE_ADDR
                //{
                //    // when running behind a load balancer you can expect this header
                //    var header = headers.First((kvp) => kvp.Key == "X-Forwarded-For").Value.ToString();
                //    // in case the IP contains a port, remove ':' and everything after
                //    ip = IPAddress.Parse(header.Remove(header.IndexOf(':')));
                //}
                //else
                //{
                //    // this will always have a value (running locally in development won't have the header)
                //    ip = Request.HttpContext.Connection.RemoteIpAddress;
                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex, "get remote ip fail");
            }

            return Ok(new { ip = remoteIpAddress.ToString(), port = remotePort });
        }

        [HttpGet("PubKx")]
        public IActionResult PubKx()
        {
            try
            {
                string mypubkx = mySettings.GetValue("my_pubx", "myWallet.json");

                Signature_Data_Hash signature_D_Hash = libraryWallet.SignData(false, mySettings.GetValue("my_pkx", "myWallet.json"), mypubkx);

                return Ok(new { pubkx=mypubkx, DerSign=signature_D_Hash.DerSign});
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

    }
}
