using Arista_LPS_WebApp.DataProvider;
using Arista_LPS_WebApp.Entities;
using Arista_LPS_WebApp.Helpers;
using LPS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RunningSummaryController : ControllerBase
    {
        private IRunningSummaryDataProvider _runningSummaryDataProvider;
        private ILoggerManager _logger;

        public RunningSummaryController(IRunningSummaryDataProvider runningSummaryDataProvider, ILoggerManager logger)
        {
            _runningSummaryDataProvider = runningSummaryDataProvider;
            _logger = logger;
            _logger.LogDebug("RunningSummaryController::Constructor.");
        }

        [HttpGet("GetAllJobDetails/{runDate}/{isPrintWinSalem}/{isAutoRun}/{postFlag}/{trip}")]
        public async Task<IActionResult> GetAllJobDetails(string runDate, int isPrintWinSalem, int isAutoRun, int postFlag, string trip)
        {
            try
            {
                var JobDetailsList = await _runningSummaryDataProvider.GetAllJobDetails(runDate, isPrintWinSalem, isAutoRun, postFlag, trip);
                return Ok(JobDetailsList);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  RunningSummaryController::GetAllJobDetails." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("UpdateJobPostFlag/")]
        public async Task<IActionResult> UpdateJobPostFlag([FromBody]List<UpdateJobPostFlag> jobData)
        {
            try
            {
                var result = await _runningSummaryDataProvider.UpdateJobPostFlag(jobData, isupdate: true);
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
                _logger.LogDebug(" Error --RunningSummaryController::UpdateJobPostFlag." + ex.Message);
                _logger.LogDebug(" Error --RunningSummaryController::UpdateJobPostFlag." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}