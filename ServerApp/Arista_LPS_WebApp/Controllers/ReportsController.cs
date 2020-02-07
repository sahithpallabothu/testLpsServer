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
    public class ReportsController : ControllerBase
    {
        private IReportsDataProvider _reportsDataProvider;
        private ILoggerManager _logger;

        public ReportsController(IReportsDataProvider reportsDataProvider, ILoggerManager logger)
        {
            _reportsDataProvider = reportsDataProvider;
            _logger = logger;
            _logger.LogDebug(" debug message --  ReportsController::Constructor .");
        }
        /* Get the list of applications based on Search Criteria */
        [HttpPost("GetSearchDetails")]
        public async Task<IActionResult> GetApplicationBasedOnSearch([FromBody]Reports app)
        {
            try
            {
                var apps = await _reportsDataProvider.GetSearchDetails(app);
                return Ok(apps);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ReportsController::GetSearchDetails ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

    }
}