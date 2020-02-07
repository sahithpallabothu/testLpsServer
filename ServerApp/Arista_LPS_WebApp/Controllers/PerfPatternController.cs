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
    public class PerfPatternController : ControllerBase
    {
        private IPerfPatternDataProvider _perfPatternDataProvider;
        private ILoggerManager _logger;
        public PerfPatternController(IPerfPatternDataProvider perfPatternDataProvider, ILoggerManager logger)
        {
            _perfPatternDataProvider = perfPatternDataProvider;
            _logger = logger;
            _logger.LogDebug("PerfPatternController::Constructor.");
        }

        [HttpGet("GetAllPerfPatterns/{fromPerfPattern}")]
        public async Task<IActionResult> GetPerfPatterns(bool fromPerfPattern)
        {
            try
            {
                var perfPat = await _perfPatternDataProvider.GetPerfPatterns(fromPerfPattern);
                return Ok(perfPat);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  PerfPatternController::GetAllPrefPatterns." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddPerfPattern")]
        public async Task<IActionResult> AddPerfPattern([FromBody]PerfPatterns perfpattern)
        {
            try
            {
                var result = await _perfPatternDataProvider.AddOrUpdatePerfPatterns(perfpattern);
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
                _logger.LogDebug(" Error --  PerfPatternController::AddPerfPattern." + ex.Message);
                _logger.LogDebug(" Error --  PerfPatternController::AddPerfPattern." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("UpdatePerfPattern")]
        public async Task<IActionResult> UpdatePerfPatterns([FromBody]PerfPatterns perfpattern)
        {
            try
            {
                var result = await _perfPatternDataProvider.AddOrUpdatePerfPatterns(perfpattern, true);
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
                _logger.LogDebug(" Error --  PerfPatternController::UpdatePerfPatterns." + ex.Message);
                _logger.LogDebug(" Error --  PerfPatternController::UpdatePerfPatterns." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpDelete("DeletePerfPattern/{perfPattern}")]
        public async Task<IActionResult> DeletePerfPatterns(int perfPattern)
        {
            try
            {
                var result = await _perfPatternDataProvider.DeletePerfPatterns(perfPattern);            
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
                _logger.LogDebug(" Error --  PerfPatternController::DeletePerfPatterns." + ex.Message);
                _logger.LogDebug(" Error --  PerfPatternController::DeletePerfPatterns." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}
