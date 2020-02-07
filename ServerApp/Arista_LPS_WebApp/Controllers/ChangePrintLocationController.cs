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

    public class ChangePrintLocationController : ControllerBase
    {
        private IChangePrintLocationDataProvider _changePrintLocationDataProvider;
        private ILoggerManager _logger;

        public ChangePrintLocationController(IChangePrintLocationDataProvider printLocationDataProvider, ILoggerManager logger)
        {
            _changePrintLocationDataProvider = printLocationDataProvider;
            _logger = logger;
            _logger.LogDebug(" ChangePrintLocationController::Constructor .");
        }

        [HttpGet("GetAllApplicationData/{isPrintWinSalem}")]
        public async Task<IActionResult> GetAllApplicationData(int isPrintWinSalem)
        {
            try
            {
                var Applicationlist = await _changePrintLocationDataProvider.GetAllApplicationData(isPrintWinSalem);
                return Ok(Applicationlist);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ChangePrintLocationController::GetApplicationsData ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }


        [HttpPut("UpdateApplicationPrintLocation/")]
        public async Task<IActionResult> UpdateApplicationPrintLocation([FromBody]List<UpdateApplicationLocation> ApplicationLocationData)
        {
            try
            {
                var result = await _changePrintLocationDataProvider.UpdateApplicationPrintLocation(ApplicationLocationData, isupdate: true);
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
                _logger.LogDebug(" Error -- ChangePrintLocationController::UpdateApplicationLocation." + ex.Message);
                _logger.LogDebug(" Error -- ChangePrintLocationController::UpdateApplicationLocation." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}