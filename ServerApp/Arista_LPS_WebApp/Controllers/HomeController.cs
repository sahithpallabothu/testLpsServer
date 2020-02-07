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
    public class HomeController : ControllerBase
    {
        private IHomeDataProvider _homeDataProvider;
        private ILoggerManager _logger;

        public HomeController(IHomeDataProvider homeDataProvider, ILoggerManager logger)
        {
            _homeDataProvider = homeDataProvider;
            _logger = logger;
            _logger.LogDebug(" HomeController::Constructor.");
        }

        [HttpPost("getInserts")]
        public async Task<IActionResult> getInserts(SearchDataForHome insert)
        {
            try
            {
                var inserts = await _homeDataProvider.GetInserts(insert);
                return Ok(inserts);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  HomeController::GetInserts." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("getJobs")]
        public async Task<IActionResult> getJobs(SearchDataForHome jobObj)
        {
            try
            {
                var jobs = await _homeDataProvider.GetJobs(jobObj);
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  HomeController::getJobs." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("getJobsAndInsertsCount")]
        public async Task<IActionResult> getJobsAndInsertsCount(SearchDataForHome search)
        {
            try
            {
                bool isMaxCount = await _homeDataProvider.ValidateJobAndInsertCount(search);

                return Ok(new { message = isMaxCount ? MessageConstants.MAXLIMITEXCEEED : MessageConstants.MAXLIMITNOTEXCEEED });
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  HomeController::getJobs." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}
