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
    public class FlagsController : ControllerBase
    {
        private IFlagDataProvider _flagDataProvider;
        private ILoggerManager _logger;

        public FlagsController(IFlagDataProvider flagDataProvider, ILoggerManager logger)
        {
            _flagDataProvider = flagDataProvider;
            _logger = logger;
            _logger.LogDebug("FlagsController::Constructor.");
        }

        [HttpGet("GetAllFlags/{fromFlag}")]
        public async Task<IActionResult> GetAllFlags(bool fromFlag)
        {
            try
            {
                var flags = await _flagDataProvider.GetFlags(fromFlag);
                return Ok(flags);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  FlagsController::GetAllFlags." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddFlag")]
        public async Task<IActionResult> AddFlag([FromBody]Flagtype flagtype)
        {
            try
            {
                var flag = await _flagDataProvider.AddOrUpdateFlagType(flagtype);
                if (flag == "ok")
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(new { message = flag });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  FlagsController::AddFlag." + ex.Message);
                _logger.LogDebug(" Error --  FlagsController::AddFlag." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("UpdateFlag")]
        public async Task<IActionResult> UpdateFlag([FromBody]Flagtype flagtype)
        {
            try
            {
                var flag = await _flagDataProvider.AddOrUpdateFlagType(flagtype, true);
                if (flag == "ok")
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(new { message = flag });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  FlagsController::UpdateFlag." + ex.Message);
                _logger.LogDebug(" Error --  FlagsController::UpdateFlag." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpDelete("DeleteFlag/{flagid}")]
        public async Task<IActionResult> DeleteFlag(int flagid)
        {
            try
            {
                var result = await _flagDataProvider.DeleteFlag(flagid);
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
                _logger.LogDebug(" Error --  FlagsController::DeleteFlag." + ex.Message);
                _logger.LogDebug(" Error --  FlagsController::DeleteFlag." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}
