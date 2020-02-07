using Arista_LPS_WebApp.DataProvider;
using Arista_LPS_WebApp.Entities;
using Arista_LPS_WebApp.Helpers;
using LPS;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FeeDescriptionController : ControllerBase
    {
        private IFeeDescriptionDataProvider _feeDescDataProvider;
        private ILoggerManager _logger;
        public FeeDescriptionController(IFeeDescriptionDataProvider feeDescDataProvider, ILoggerManager logger)
        {
            _feeDescDataProvider = feeDescDataProvider;
            _logger = logger;
            _logger.LogDebug(" FeeDescriptionController::Constructor.");
        }

        [HttpGet("getAllFeeDescriptions/{fromFeeDesc}")]
        public async Task<IActionResult> GetFeeDescriptions(bool fromFeeDesc)
        {
            try
            {
                var feeDescriptions = await _feeDescDataProvider.GetFeeDescriptions(fromFeeDesc);
                return Ok(feeDescriptions);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  FeeDescriptionController::GetFeeDescriptions." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddFeeDescription")]
        public async Task<IActionResult> AddFeeDescription([FromBody]FeeDescription feeDescription)
        {
            try
            {
                var feeDesc = await _feeDescDataProvider.AddOrUpdateFeeDescription(feeDescription);
                if (feeDesc == "ok")
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(new { message = feeDesc });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error -- FeeDescriptionController::AddFeeDescription." + ex.Message);
                _logger.LogDebug(" Error -- FeeDescriptionController::AddFeeDescription." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("UpdateFeeDescription")]
        public async Task<IActionResult> UpdateFeeDescription([FromBody]FeeDescription feeDescription)
        {
            try
            {
                var feeDesc = await _feeDescDataProvider.AddOrUpdateFeeDescription(feeDescription, true);
                if (feeDesc == "ok")
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(new { message = feeDesc });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error -- FeeDescriptionController::UpdateFeeDescription." + ex.Message);
                _logger.LogDebug(" Error -- FeeDescriptionController::UpdateFeeDescription." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpDelete("DeleteFeeDescription/{RecID}")]
        public async Task<IActionResult> DeleteFeeDescription(int recId)
        {
            try
            {
                var result = await _feeDescDataProvider.DeleteFeeDescription(recId);
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
                _logger.LogDebug(" Error --  FeeDescriptionController::DeleteFeeDescription ." + ex.Message);
                _logger.LogDebug(" Error --  FeeDescriptionController::DeleteFeeDescription ." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}

