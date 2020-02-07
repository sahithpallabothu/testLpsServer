using Arista_LPS_WebApp.DataProvider;
using Arista_LPS_WebApp.Entities;
using Arista_LPS_WebApp.Helpers;
using LPS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SizeController : ControllerBase
    {
        private ISizeDataProvider _sizeDataProvider;
        private ILoggerManager _logger;
        public SizeController(ISizeDataProvider sizeDataProvider, ILoggerManager logger)
        {
            _sizeDataProvider = sizeDataProvider;
            _logger = logger;
            _logger.LogDebug("SizeController::Constructor.");
        }

        [HttpGet("getAllSizes/{fromSize}")]
        public async Task<IActionResult> GetSize(bool fromSize)
        {
            try
            {
                var result = await _sizeDataProvider.GetSize(fromSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  SizeController::GetSize ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddSize")]
        public async Task<IActionResult> AddSize([FromBody]Size sizeObj)
        {
            try
            {
                var result = await _sizeDataProvider.AddOrUpdateSize(sizeObj);
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
                _logger.LogDebug(" Error --  SizeController::AddSize ." + ex.Message);
                _logger.LogDebug(" Error --  SizeController::AddSize ." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("UpdateSize")]
        public async Task<IActionResult> UpdateSize([FromBody]Size sizeObj)
        {
            try
            {
                var result = await _sizeDataProvider.AddOrUpdateSize(sizeObj, true);
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
                _logger.LogDebug(" Error --  SizeController::UpdateSize." + ex.Message);
                _logger.LogDebug(" Error --  SizeController::UpdateSize." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpDelete("DeleteSize/{sizeID}")]
        public async Task<IActionResult> DeleteSize(int sizeID)
        {
            try
            {
                var result = await _sizeDataProvider.DeleteSize(sizeID);
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
                _logger.LogDebug(" Error --  SizeController::DeleteSize." + ex.Message);
                _logger.LogDebug(" Error --  SizeController::DeleteSize." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}
