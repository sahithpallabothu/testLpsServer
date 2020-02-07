using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Arista_LPS_WebApp.Entities;
using LPS;
using Arista_LPS_WebApp.DataProvider;
using System.Threading.Tasks;
using System;
using Arista_LPS_WebApp.Helpers;

namespace Arista_LPS_WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PRatesController : ControllerBase
    {
        private IPRateDataProvider _pRateDataProvider;
        private ILoggerManager _logger;
        public PRatesController(IPRateDataProvider pRateDataProvider, ILoggerManager logger)
        {
            _pRateDataProvider = pRateDataProvider;
            _logger = logger;
            _logger.LogDebug(" debug message --  PRatesController::Constructor .");
        }

        [HttpGet("GetPRates")]
        public async Task<IActionResult> GetPRates()
        {
            try
            {
                _logger.LogDebug(" debug message --  PRatesController::GetPRates : ");
                var pRates = await _pRateDataProvider.GetPRates();
                return Ok(pRates);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  PRatesController::GetPRates ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddPRate")]
        public async Task<IActionResult> AddPRate([FromBody]PRates pRate)
        {
            try
            {
                var result = await _pRateDataProvider.AddOrUpdatePRates(pRate);
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
                _logger.LogDebug(" Error --  PRatesController::AddPRate ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }

        }

        [HttpPut("UpdatePRates")]
        public async Task<IActionResult> UpdatePRates([FromBody]PRates pRate)
        {
            try
            {
                var result = await _pRateDataProvider.AddOrUpdatePRates(pRate, true);
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
                _logger.LogDebug(" Error --  PRatesController::UpdatePRates ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }


        [HttpDelete("DeletePRate/{Prateid}")]
        public async Task<IActionResult> DeletePRate(int pRateID)
        {
            try
            {
                var result = await _pRateDataProvider.DeletePRate(pRateID);
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
                _logger.LogDebug(" Error --  PRatesController::DeletePRate ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}
