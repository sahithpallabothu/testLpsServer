using Arista_LPS_WebApp.DataProvider;
using Arista_LPS_WebApp.Entities;
using Arista_LPS_WebApp.Helpers;
using LPS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Arista_LPS_WebApp.Controllers
{
    [Route("[controller]")]
    public class SoftwareController : Controller
    {
        private ISoftwareDataProvider _softwareDataProvider;
        private ILoggerManager _logger;
        public SoftwareController(ISoftwareDataProvider softwareDataProvider, ILoggerManager logger)
        {
            _softwareDataProvider = softwareDataProvider;
            _logger = logger;
            _logger.LogDebug("SoftwareController::Constructor.");
        }

        [HttpGet("GetAllSoftwares")]
        public async Task<IActionResult> getSoftwares()
        {
            try
            {
                var results = await _softwareDataProvider.GetAllSoftware();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  SoftwareController::getSoftwares ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddSoftware")]
        public async Task<IActionResult> addSoftware([FromBody]Software soft)
        {
            try
            {
                var result = await _softwareDataProvider.AddOrUpdateSoftware(soft);
                if (result == "ok")
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(new { message = result });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  SoftwareController::addSoftware." + ex.Message);
                _logger.LogDebug(" Error --  SoftwareController::addSoftware." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("UpdateSoftware")]
        public async Task<IActionResult> updateSoftware([FromBody]Software soft)
        {
            try
            {
                var result = await _softwareDataProvider.AddOrUpdateSoftware(soft, true);
                if (result == "ok")
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(new { message = result });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  SoftwareController::updateSoftware." + ex.Message);
                _logger.LogDebug(" Error --  SoftwareController::updateSoftware." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpDelete("DeleteSoftware/{SoftwareID}")]
        public async Task<IActionResult> DeleteSoftware(int softwareID)
        {
            try
            {
                var result = await _softwareDataProvider.DeleteSoftware(softwareID);
                if (result == "ok")
                {
                    return Ok(new { message = result });
                }
                else if (result == MessageConstants.RECORDINUSE)
                {
                    return Ok(new { message = result, StatusCode = 2000 });
                }
                else
                {
                    return BadRequest(new { message = result });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  SoftwareController::DeleteSoftware." + ex.Message);
                _logger.LogDebug(" Error --  SoftwareController::DeleteSoftware." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }




    }
}
