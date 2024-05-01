using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DB;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EKoin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [RestrictToLocalhost]
    public class SettingsController : ControllerBase
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly INodeRepo nodeRepo;
        public SettingsController(INodeRepo _nodeRepo)
        {
            nodeRepo = _nodeRepo;
        }

        [HttpGet("GetNodes")]
        public async Task<IActionResult> GetNodes(int Id_Local, bool? isTP)
        {
            try
            {
                return Ok(await nodeRepo.GetNetworkNodes(Id_Local, isTP));
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("AddUpdateNodes")]
        public async Task<IActionResult> AddUpdateNodes(NetworkNode node)
        {
            try
            {
                NetworkNode networkNode = await nodeRepo.AddUpdateNetworkNode(node);
                if (networkNode == null)
                {
                    return StatusCode(400);
                }
                else
                {
                    return Ok(new { networkNode.Id_Local });
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

        [HttpPost("DeleteNode")]
        public async Task<IActionResult> DeleteNode(int Id_Local)
        {
            try
            {
                bool success = await nodeRepo.DeleteNetworkNode(Id_Local);
                if (success)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(400);

                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return StatusCode(500);
            }

        }

    }
}
