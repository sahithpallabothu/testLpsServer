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
    public class ApplicationController : ControllerBase
    {
        private IApplicationDataProvider _applicationDataProvider;
        private ILoggerManager _logger;

        public ApplicationController(IApplicationDataProvider applicationDataProvider, ILoggerManager logger)
        {
            _applicationDataProvider = applicationDataProvider;
            _logger = logger;
            _logger.LogDebug(" debug message --  ApplicationController::Constructor .");
        }

        // Add application
        [HttpPost("AddApplication")]
        public async Task<IActionResult> AddApplication([FromBody]Application application)
        {
            try
            {
                var result = await _applicationDataProvider.AddOrUpdateApplication(application);
                if (result != null)
                {
                    return Ok(new { message = result });
                }
                else
                {
                    return BadRequest(new { message = result });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ApplicationController::AddApplication ." + ex.Message);
                _logger.LogDebug(" Error --  ApplicationController::AddApplication ." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        //To Update Application. 
        [HttpPut("UpdateApplication")]
        public async Task<IActionResult> UpdateApplication([FromBody]Application applicationDetails)
        {
            try
            {
                var resultOfUpdate = await _applicationDataProvider.AddOrUpdateApplication(applicationDetails, true);
                if (resultOfUpdate == "Ok")
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(new { message = resultOfUpdate });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ApplicationController::UpdateApplication ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        /* Get All the applications for Particular customer. */
        [HttpGet("GetAllApplications")]
        public async Task<IActionResult> GetAllApplications()
        {
            try
            {
                var apps = await _applicationDataProvider.GetApplications();
                return Ok(apps);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ApplicationController::GetApplications ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        /* Get the list of applications based on Search Criteria */
        [HttpPost("GetApplicationBasedOnSearch")]
        public async Task<IActionResult> GetApplicationBasedOnSearch([FromBody]Application app)
        {
            try
            {
                var apps = await _applicationDataProvider.GetApplicationBasedOnSearch(app);
                return Ok(apps);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ApplicationController::GetApplicationsBasedOnSearch ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("GetLocations")]
        public async Task<IActionResult> GetAllLocations()
        {
            try
            {
                var apps = await _applicationDataProvider.GetLocations();
                return Ok(apps);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ApplicationController::GetAllLocations ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("GetConfigData")]
        public IActionResult GetConfigData()
        {
            try
            {
                var configData = _applicationDataProvider.GetConfiguration();
                return Ok(configData);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ApplicationController::GetConfigData ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("GetApplicationByID/{appID}")]
        public async Task<IActionResult> GetApplicationById(int appID)
        {
            try
            {
                var app = await _applicationDataProvider.GetApplications(appID);
                if (app == null)
                {
                    return NotFound();
                }
                return Ok(app);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ApplicationController::GetApplicationById ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("CheckActiveApplicationExist/{clientID}/{recordID}")]
        public async Task<IActionResult> CheckActiveApplicationExist(int clientID,int recordID=0) {
            try
            {
                var result = await _applicationDataProvider.CheckActiveApplicationExist(clientID,recordID);
               
                    return Ok(new { message=result });
                

            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ApplicationController::CheckActiveApplicationExist." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }

        }

        [HttpDelete("DeleteApplication/{appid}")]
        public async Task<IActionResult> DeleteApplication(int AppID)
        {
            try
            {
                var result = await _applicationDataProvider.DeleteApplication(AppID);

                if (result == "ok")
                {
                    return Ok(new { message = result });
                }
                else
                {
                    return BadRequest(new { message = result });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ApplicationController::DeleteApplication ." + ex.Message);
                _logger.LogDebug(" Error --  ApplicationController::DeleteApplication ." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}
