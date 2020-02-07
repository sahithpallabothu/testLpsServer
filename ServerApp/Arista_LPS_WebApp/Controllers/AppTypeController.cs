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
    public class AppTypeController : ControllerBase
    {
        private IAppTypeDataProvider _appDataProvider;
        private ILoggerManager _logger;
        public AppTypeController(IAppTypeDataProvider appTypeDataProvider, ILoggerManager logger)
        {
            _appDataProvider = appTypeDataProvider;
            _logger = logger;
            _logger.LogDebug(" AppTypeController::Constructor .");
        }

        [HttpGet("GetAllApptypes/{fromAppType}")]
        public async Task<IActionResult> GetAppTypes(bool fromAppType)
        {
            try
            {
                var appTypes = await _appDataProvider.GetAppTypes(fromAppType);
                return Ok(appTypes);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  AppTypeController::GetAllApptypes ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddApptype")]
        public async Task<IActionResult> AddAppType([FromBody]AppType apptype)
        {
            try
            {
                var result = await _appDataProvider.AddOrUpdateAppType(apptype);
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
                _logger.LogDebug(" Error --  AppTypeController::AddApptype ." + ex.Message);
                _logger.LogDebug(" Error --  AppTypeController::AddApptype ." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("UpdateApptype")]
        public async Task<IActionResult> UpdateAppType([FromBody]AppType apptype)
        {
            try
            {
                var result = await _appDataProvider.AddOrUpdateAppType(apptype, true);
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
                _logger.LogDebug(" Error --  AppTypeController::UpdateAppType ." + ex.Message);
                _logger.LogDebug(" Error --  AppTypeController::UpdateAppType ." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpDelete("DeleteApptype/{appid}")]
        public async Task<IActionResult> DeleteAppType(string AppID)
        {
            try
            {
                var result = await _appDataProvider.DeleteAppType(AppID);

                if (result[0] == "ok")
                {
                    return Ok(new { message = result[0] });
                }
                else if (result[0] == MessageConstants.RECORDINUSE)
                {
                    return Ok(new { message = result, StatusCode = 2000 });
                }
                else
                {
                    return BadRequest(new { message = result[0] });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  AppTypeController::DeleteApptype ." + ex.Message);
                _logger.LogDebug(" Error --  AppTypeController::DeleteApptype ." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}
