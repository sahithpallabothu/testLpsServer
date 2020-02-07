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
    public class AdditionalChargesController : ControllerBase
    {
        private IAdditionalChargesDataProvider _additionalChargesDataProvider;
        private ILoggerManager _logger;

        public AdditionalChargesController(IAdditionalChargesDataProvider additionalChargesDataProvider, ILoggerManager logger)
        {
             _additionalChargesDataProvider = additionalChargesDataProvider;
            _logger = logger;
            _logger.LogDebug(" debug message --  ApplicationController::Constructor .");
        }

        [HttpPost("AddAdditionalCharges")]
        public async Task<IActionResult> AddAdditionalCharges([FromBody]AdditionalChargesInfo additionalCharges)
        {
            try
            {
                var result = await _additionalChargesDataProvider.AddOrUpdateAdditionalCharges(additionalCharges);
                if (result=="ok")
                {
                    return Ok(new { message = result });
                }
                else
                {
                    return BadRequest(new { message = result });
                }
            }
            catch(Exception ex)
            {
                _logger.LogDebug(" Error --  AdditionalChargesController::AddAdditionalCharges ." + ex.Message);
                _logger.LogDebug(" Error --  AdditionalChargesController::AddAdditionalCharges ." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("validateJobNumber/{jobNumber}/{clientCode}")]
        public async Task<IActionResult> validateJobNumber(string jobNumber , string clientCode)
        {
            try{
                var jobDetails = await _additionalChargesDataProvider.ValidateJobNumber(jobNumber, clientCode);
                return Ok(jobDetails);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  AdditionalChargesController::Validate job Number ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
            
        }

        [HttpPost("checkDuplicateData")]
        public async Task<IActionResult> checkDuplicateData([FromBody]AdditionalCharges additionalCharges)
        {
            try
            {
                var result = await _additionalChargesDataProvider.CheckDuplicateData(additionalCharges);
                if (result=="ok")
                {
                    return Ok(new { message = result });
                }
                if (result == MessageConstants.RECORDINUSE)
                {
                    return Ok(new { message = result});
                }
                else
                {
                    return BadRequest(new { message = result });
                }
            }
            catch(Exception ex)
            {
                _logger.LogDebug(" Error --  AdditionalChargesController::AddAdditionalCharges ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        /*Get All the Additional charges based on search criteria.*/
        [HttpGet("GetAllAdditionalCharges/{customerCode}/{clientName}")]
        public async Task<IActionResult> GetAllAdditionalCharges(string customerCode, string ClientName)
        {

            try
            {
                var addlCharges = await _additionalChargesDataProvider.GetAdditionalChargesBasedOnSearch(customerCode, ClientName);
                return Ok(addlCharges);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  AdditionalChargesController::GetAllAdditionalCharges ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("UpdateAdditionalChargeByID")]
        public async Task<IActionResult> UpdateAdditionalChargeByID([FromBody]PostageAdditionalCharges acDetails)
        {
            try
            {
                var acDetail = await _additionalChargesDataProvider.UpdateAdditionalChargeByID(acDetails);
                if (acDetail == "ok")
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(new { message = acDetail });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  AdditionalChargesController::UpdateAdditionalChargeByID ." + ex.Message);
                _logger.LogDebug(" Error --  AdditionalChargesController::UpdateAdditionalChargeByID ." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        /*Get All the Additional charges Count.*/
        [HttpGet("GetAllAdditionalChargesCount/{customerCode}/{clientName}")]
        public async Task<IActionResult> GetAllAdditionalChargesCount(string customerCode, string ClientName)
        {

            try
            {
                bool isMaxCount = await _additionalChargesDataProvider.GetAdditionalChargesCount(customerCode, ClientName);
                return Ok(new { message = isMaxCount ? MessageConstants.MAXLIMITEXCEEED : MessageConstants.MAXLIMITNOTEXCEEED });
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  AdditionalChargesController::GetAllAdditionalChargesCount ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

    }
}
