using Arista_LPS_WebApp.DataProvider;
using Arista_LPS_WebApp.Entities;
using Arista_LPS_WebApp.Helpers;
using LPS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AristaLPS.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RateDescriptionController : ControllerBase
    {
        private IRateDescriptionDataProvider _rateDescriptionDataProvider;
        private ILoggerManager _logger;
        public RateDescriptionController(IRateDescriptionDataProvider rateDescriptionDataProvider, ILoggerManager logger)
        {
            _rateDescriptionDataProvider = rateDescriptionDataProvider;
            _logger = logger;
            _logger.LogDebug("RateDescriptionController::Constructor.");
        }

        [HttpGet("GetAllRateDescriptions/{fromRateType}")]
        public async Task<IActionResult> getRateDescriptions(bool fromRateType)
        {
            try
            {
                var results = await _rateDescriptionDataProvider.GetRateDescriptions(fromRateType);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  RateDescriptionController::getRateDescriptions ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddRateDescription")]
        public async Task<IActionResult> addRateDescription([FromBody]RateDescription rateDescription)
        {
            try
            {
                var result = await _rateDescriptionDataProvider.AddOrUpdateRateDescription(rateDescription);
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
                _logger.LogDebug(" Error --  RateDescriptionController::addRateDescription." + ex.Message);
                _logger.LogDebug(" Error --  RateDescriptionController::addRateDescription." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("UpdateRateDescription")]
        public async Task<IActionResult> updateRateDescription([FromBody]RateDescription rateDescription)
        {
            try
            {
                var result = await _rateDescriptionDataProvider.AddOrUpdateRateDescription(rateDescription, true);
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
                _logger.LogDebug(" Error --  RateDescriptionController::updateRateDescription." + ex.Message);
                _logger.LogDebug(" Error --  RateDescriptionController::updateRateDescription." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpDelete("DeleteRateDescription/{rateDescriptionID}")]
        public async Task<IActionResult> DeleteRateDescription(int rateDescriptionID)
        {
            try
            {
                var result = await _rateDescriptionDataProvider.DeleteRateDescription(rateDescriptionID);
                if (result == "ok")
                {
                   return Ok(new { message = result });
                } 
                else if(result == MessageConstants.RECORDINUSE)
                {
                   return  Ok(new { message = result, StatusCode = 2000 });
                }
                else
                {
                   return BadRequest(new { message = result });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  RateDescriptionController::DeleteRateDescription." + ex.Message);
                _logger.LogDebug(" Error --  RateDescriptionController::DeleteRateDescription." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}
