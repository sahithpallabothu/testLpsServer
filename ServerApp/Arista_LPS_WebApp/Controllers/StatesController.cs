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
    [Route("[controller]")]
    [ApiController]
    public class StatesController : ControllerBase
    {
        #region Private variables
        private readonly IStateDataProvider _stateDataProvider;
        private readonly ILoggerManager _logger;
        #endregion Private variables

        #region Constuctor
        public StatesController(IStateDataProvider stateTypeDataProvider, ILoggerManager logger)
        {
            _stateDataProvider = stateTypeDataProvider;
            _logger = logger;
            _logger.LogDebug("StatesController::Constructor.");
        }
        #endregion Constuctor

        [HttpGet("GetAllStates/{fromStates}")]
        public async Task<IActionResult> GetStates(bool fromStates)
        {
            try
            {
                var states = await _stateDataProvider.GetStates(fromStates);
                return Ok(states);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  StatesController::GetAllStates." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddState")]
        public async Task<IActionResult> AddState([FromBody]States state)
        {
            try
            {
                var result = await _stateDataProvider.AddOrUpdateState(state);
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
                _logger.LogDebug(" Error --  StatesController::AddState." + ex.Message);
                _logger.LogDebug(" Error --  StatesController::AddState." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("UpdateState")]
        public async Task<IActionResult> UpdateState([FromBody]States state)
        {
            try
            {
                var result = await _stateDataProvider.AddOrUpdateState(state, true);
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
                _logger.LogDebug(" Error --  StatesController::UpdateState." + ex.Message);
                _logger.LogDebug(" Error --  StatesController::UpdateState." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpDelete("DeleteState/{id}")]
        public async Task<IActionResult> DeleteState(int id)
        {
            try
            {
                var result = await _stateDataProvider.DeleteState(id);
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
                _logger.LogDebug(" Error --  StatesController::DeleteState." + ex.Message);
                _logger.LogDebug(" Error --  StatesController::DeleteState." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}
