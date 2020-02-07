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

    public class ApplicationNotificationsController : ControllerBase
    {
        private IApplicationNotificationsDataProvider _applicationNotificaionDataProvider;
        private ILoggerManager _logger;

        public ApplicationNotificationsController(IApplicationNotificationsDataProvider customerDataProvider, ILoggerManager logger)
        {
            _applicationNotificaionDataProvider = customerDataProvider;
            _logger = logger;
            _logger.LogDebug(" ApplicationNotificationsController::Constructor .");
        }

        [HttpGet("GetApplicationContacts/{clientId}/{applicationId}")]
        public async Task<IActionResult> GetApplicationContacts(int clientId, int applicationId)
        {
            try
            {
                var contactlist = await _applicationNotificaionDataProvider.GetApplicationContacts(clientId, applicationId);
                return Ok(contactlist);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ApplicationNotificationsController::GetApplicationContacts ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }


        [HttpPut("AddUpdateApplicationNotifications/")]
        public async Task<IActionResult> AddUpdateApplicationNotifications([FromBody]ApplicationNotificationsData AppNotifications)
        {
            try
            {
                var result = await _applicationNotificaionDataProvider.AddOrUpdateNotifications(AppNotifications, isupdate: true);
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
                _logger.LogDebug(" Error -- ApplicationNotificationsController::AddUpdateApplicationNotifications." + ex.Message);
                _logger.LogDebug(" Error -- ApplicationNotificationsController::AddUpdateApplicationNotifications." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}