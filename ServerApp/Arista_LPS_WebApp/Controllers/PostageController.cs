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
    public class PostageController : ControllerBase
    {
        private IPostageDataProvider _postageDataProvider;
        private ILoggerManager _logger;

        public PostageController(IPostageDataProvider postageDataProvider, ILoggerManager logger)
        {
            _postageDataProvider = postageDataProvider;
            _logger = logger;
            logger.LogDebug("PostageController::Constructor.");
        }

        [HttpPost("getPostageByJob")]
        public async Task<IActionResult> getPostageByJob([FromForm]string jobNumber, [FromForm]string runDate, [FromForm]string recordId)
        {
            try
            {
                var postageData = await _postageDataProvider.getPostageByJob(jobNumber, runDate, recordId);
                return Ok(postageData);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  PostageController::getPostageByJob." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("updateJobDetail")]
        public async Task<IActionResult> updateJobDetail([FromBody]JobDetail jobDetailObject)
        {
            try
            {
                var result = await _postageDataProvider.updateJobDetail(jobDetailObject);
                if (result == "ok")
                {
                    return Ok();
                }
                else
                {
                    _logger.LogDebug(" Error --  PostageController::updateJobDetail." + result);
                    return BadRequest(new { message = result });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  PostageController::updateJobDetail." + ex.Message);
                _logger.LogDebug(" Error --  PostageController::updateJobDetail." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}
