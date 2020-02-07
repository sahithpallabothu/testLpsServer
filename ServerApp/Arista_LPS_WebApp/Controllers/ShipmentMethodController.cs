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
    public class ShipmentMethodController : ControllerBase
    {

        #region Private variables
        private readonly IShipmentMethodDataProvider _shipmentMethodDataProvider;
        private readonly ILoggerManager _logger;
        #endregion Private variables

        #region Constuctor
        public ShipmentMethodController(IShipmentMethodDataProvider shipmentMethodDataProvider, ILoggerManager logger)
        {
            _shipmentMethodDataProvider = shipmentMethodDataProvider;
            _logger = logger;
            _logger.LogDebug("ShipmentMethodController::Constructor.");
        }
        #endregion Constuctor

        [HttpGet("GetShipmentMethod/{fromShipment}")]
        public async Task<IActionResult> GetShipmentMethod(bool fromShipment)
        {
            try
            {
                var result = await _shipmentMethodDataProvider.GetShipmentMethod(fromShipment);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ShipmentMethodController::GetShipmentMethod." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddShipmentMethod")]
        public async Task<IActionResult> AddShipmentMethod([FromBody]ShipmentMethod shipmentmethod)
        {
            try
            {
                var result = await _shipmentMethodDataProvider.AddOrUpdateShipmentMethod(shipmentmethod);
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
                _logger.LogDebug(" Error --  ShipmentMethodController::AddShipmentMethod." + ex.Message);
                _logger.LogDebug(" Error --  ShipmentMethodController::AddShipmentMethod." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("UpdateShipmentMethod")]
        public async Task<IActionResult> UpdateShipmentMethod([FromBody]ShipmentMethod shipmentmethod)
        {
            try
            {
                var result = await _shipmentMethodDataProvider.AddOrUpdateShipmentMethod(shipmentmethod, true);
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
                _logger.LogDebug(" Error --  ShipmentMethodController::UpdateShipmentMethod." + ex.Message);
                _logger.LogDebug(" Error --  ShipmentMethodController::UpdateShipmentMethod." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpDelete("DeleteShipmentMethod/{shipmentmethodID?}")]
        public async Task<IActionResult> DeleteShipmentMethod(int shipmentMethodID)
        {
            try
            {
                var result = await _shipmentMethodDataProvider.DeleteShipmentMethod(shipmentMethodID);
                if (result == "ok")
                {
                    return Ok(new { message = result });
                }
                else if (result == MessageConstants.RECORDINUSE)
                {
                    return Ok(new { message = result, StatusCode = 2000 });
                }
                else
                {
                    return BadRequest(new { message = result });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ShipmentMethodController::DeleteShipmentMethod." + ex.Message);
                _logger.LogDebug(" Error --  ShipmentMethodController::DeleteShipmentMethod." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}
