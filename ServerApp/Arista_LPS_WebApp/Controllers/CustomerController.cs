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
    public class CustomerController : ControllerBase
    {
        private ICustomerDataProvider _customerDataProvider;
        private ILoggerManager _logger;

        public CustomerController(ICustomerDataProvider customerDataProvider, ILoggerManager logger)
        {
            _customerDataProvider = customerDataProvider;
            _logger = logger;
            _logger.LogDebug("CustomerController :: Constructor.");
        }

        [HttpGet("GetCustomers/{isCustomer}")]
        public async Task<IActionResult> GetCustomers(bool isCustomer = false)
        {
            try
            {
                var customers = await _customerDataProvider.GetCustomers(isCustomer: isCustomer);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  CustomerController::GetCustomers." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("GetCustomer/{customerid}")]
        public async Task<IActionResult> GetCustomer(int customerid)
        {
            try
            {
                var customers = await _customerDataProvider.GetCustomers(customerid, true);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  CustomerController::GetCustomer." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("GetViewCustomers")]
        public async Task<IActionResult> GetViewCustomer(Customer obj)
        {
            try
            {
                var customers = await _customerDataProvider.GetViewCustomer(obj);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  CustomerController::GetViewCustomer ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddCustomer")]
        public async Task<IActionResult> AddCustomer([FromBody]Customer customer)
        {
            try
            {
                var result = await _customerDataProvider.AddOrUpdateCustomer(customer);
                if (result == "ok")
                {
                    return Ok(new { message = result });
                }
                if (result.Contains("CUSTOMEREXIST"))
                {
                    return Ok(new { message = "CUSTOMEREXIST" });
                }
                if (result.Contains("MAILERIDORCRIDEXIST"))
                {
                    return Ok(new { message = "MAILERIDORCRIDEXIST" });
                }
                else
                {
                    return BadRequest(new { message = result });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  CustomerController::AddCustomer." + ex.Message);
                _logger.LogDebug(" Error --  CustomerController::AddCustomer." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer([FromBody]Customer customer)
        {
            try
            {
                var result = await _customerDataProvider.AddOrUpdateCustomer(customer, true);
                if (result == "ok")
                {
                    return Ok(new { message = result });
                }
                if (result.Contains("CUSTOMEREXIST"))
                {
                    return Ok(new { message = "CUSTOMEREXIST" });
                }
                if (result.Contains("MAILERIDORCRIDEXIST"))
                {
                    return Ok(new { message = "MAILERIDORCRIDEXIST" });
                }
                else
                {
                    return BadRequest(new { message = result });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  CustomerController::UpdateCustomer." + ex.Message);
                _logger.LogDebug(" Error --  CustomerController::UpdateCustomer." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("GetHeldTypes")]
        public async Task<IActionResult> GetHeldTypes()
        {
            try
            {
                var heldType = await _customerDataProvider.GetHeldType();
                return Ok(heldType);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  CustomerController::GetHeldTypes." + ex.Message);
                _logger.LogDebug(" Error --  CustomerController::GetHeldTypes." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}
