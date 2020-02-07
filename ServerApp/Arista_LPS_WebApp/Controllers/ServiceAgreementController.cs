using Arista_LPS_WebApp.DataProvider;
using Arista_LPS_WebApp.Entities;
using Arista_LPS_WebApp.Helpers;
using LPS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ServiceAgreementController : ControllerBase
    {
        #region Private variables
        private readonly IServiceAgreementDataProvider _serviceAgreementDataProvider;
        private readonly ILoggerManager _logger;

        #endregion Private variables

        #region Constuctor
        public ServiceAgreementController(IServiceAgreementDataProvider serviceAgreementDataProvider, ILoggerManager logger)
        {
            _serviceAgreementDataProvider = serviceAgreementDataProvider;
            _logger = logger;
            _logger.LogDebug("ServiceAgreementController::Constructor.");
        }

        #endregion Constuctor

        [HttpPost("SendBlobData")]
        public IActionResult SendBlobData([FromForm]string fileName, [FromForm]string customerName, [FromForm]string clientType)
        {
            try
            {
                byte[] file = null;
                file = _serviceAgreementDataProvider.GetFileData(fileName, customerName, clientType);

                if (file == null)
                {
                    return BadRequest(new { message = "File not found." });
                }
                else
                {
                    return Ok(file);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ServiceAgreementController::SendBlobData." + ex.Message);
                _logger.LogDebug(" Error --  ServiceAgreementController::SendBlobData." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("GetServiceAgreements")]
        public async Task<IActionResult> GetServiceAgreements()
        {
            try
            {
                var serviceAgreements = await _serviceAgreementDataProvider.GetClientServiceAgreement();
                return Ok(serviceAgreements);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ServiceAgreementController::GetServiceAgreements." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("GetClientServiceAgreement/{clientID}")]
        public async Task<IActionResult> GetClientServiceAgreement(int clientID)
        {
            try
            {
                var serviceAgreement = await _serviceAgreementDataProvider.GetClientServiceAgreement(clientID);
                return Ok(serviceAgreement);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ServiceAgreementController::GetClientServiceAgreement." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddServiceAgreement")]
        public async Task<IActionResult> AddServiceAgreement([FromBody]ClientServiceAgreement serviceAgreement)
        {
            try
            {
                var result = await _serviceAgreementDataProvider.AddOrUpdateServiceAgreement(serviceAgreement);
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
                _logger.LogDebug(" Error --  ServiceAgreementController::AddServiceAgreement." + ex.Message);
                _logger.LogDebug(" Error --  ServiceAgreementController::AddServiceAgreement." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("UpdateServiceAgreement")]
        public async Task<IActionResult> UpdateServiceAgreement([FromBody]ClientServiceAgreement serviceAgreement)
        {
            try
            {
                var result = await _serviceAgreementDataProvider.AddOrUpdateServiceAgreement(serviceAgreement, true);
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
                _logger.LogDebug(" Error --  ServiceAgreementController::UpdateServiceAgreement." + ex.Message);
                _logger.LogDebug(" Error --  ServiceAgreementController::UpdateServiceAgreement." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        #region Upload File

        [HttpPost("UploadFile"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile([FromForm]IFormFile file, [FromForm]string ClientName, [FromForm]string CustomerType)
        {
            try
            {
                var filetoUpload = Request.Form.Files[0];
                var result = await _serviceAgreementDataProvider.UploadFile(filetoUpload, ClientName, CustomerType);
                if (result == "ok")
                {
                    return Ok(new { message = "File saved." });
                }
                else if (result == "File already exist.")
                {
                    return Ok(new { message = "File already exist." });
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ServiceAgreementController::UploadFile ." + ex.Message);
                _logger.LogDebug(" Error --  ServiceAgreementController::UploadFile ." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        #endregion

        // To Get All Contract Types.
        [HttpGet("GetContractTypes")]
        public async Task<IActionResult> GetContractTypes()
        {
            try
            {
                var contractTypes = await _serviceAgreementDataProvider.GetContractTypes();
                return Ok(contractTypes);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ServiceAgreementController::GetContractTypes." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        // To GetContracts.
        [HttpGet("GetContracts/{clientID}")]
        public async Task<IActionResult> GetContracts(int clientID)
        {
            try
            {
                var contracts = await _serviceAgreementDataProvider.GetContracts(clientID);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ServiceAgreementController::GetContracts." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddContract")]
        public async Task<IActionResult> AddContract([FromBody]Contracts contracts)
        {
            try
            {
                var result = await _serviceAgreementDataProvider.AddOrUpdateContract(contracts);
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
                _logger.LogDebug(" Error --  ServiceAgreementController::AddContract ." + ex.Message);
                _logger.LogDebug(" Error --  ServiceAgreementController::AddContract ." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        // To UpdateContract
        [HttpPut("UpdateContract")]
        public async Task<IActionResult> UpdateContract([FromBody]Contracts contracts)
        {
            try
            {
                var result = await _serviceAgreementDataProvider.AddOrUpdateContract(contracts, true);
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
                _logger.LogDebug(" Error --  ServiceAgreementController::UpdateContract." + ex.Message);
                _logger.LogDebug(" Error --  ServiceAgreementController::UpdateContract." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        // DeleteContract
        [HttpDelete("DeleteContract/{clientContractID}/{customerType}/{clientName}/{selectedRowFileName}")]
        public async Task<IActionResult> DeleteContract(int clientContractID, string customerType, string clientName, string selectedRowFileName)
        {
            try
            {
                var result = await _serviceAgreementDataProvider.DeleteContract(clientContractID, customerType, clientName, selectedRowFileName);
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
                _logger.LogDebug(" Error --  ServiceAgreementDataProvider::DeleteContract." + ex.Message);
                _logger.LogDebug(" Error --  ServiceAgreementDataProvider::DeleteContract." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        #region Billing Rates

        //To Get All billing rates.
        [HttpGet("GetBillingRates/{clientID}")]
        public async Task<IActionResult> GetBillingRates(int clientID)
        {
            try
            {
                var billRates = await _serviceAgreementDataProvider.GetBillingRates(clientID);
                return Ok(billRates);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ServiceAgreementController::GetBillingRates." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddBillingRate")]
        public async Task<IActionResult> AddBillingRate([FromBody]BillingRateInfo billingRateInfo)
        {
            try
            {
                var result = await _serviceAgreementDataProvider.AddOrUpdateBillingRates(billingRateInfo);
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
                _logger.LogDebug(" Error --  ServiceAgreementController::AddBillingRate." + ex.Message);
                _logger.LogDebug(" Error --  ServiceAgreementController::AddBillingRate." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        #endregion
    }
}
