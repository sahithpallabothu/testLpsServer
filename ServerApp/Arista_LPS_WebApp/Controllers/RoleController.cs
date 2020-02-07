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
    public class RoleController : ControllerBase
    {
        private IRoleDataProvider _roleDataProvider;
        private ILoggerManager _logger;
        public RoleController(IRoleDataProvider roleDataProvider, ILoggerManager logger)
        {
            _roleDataProvider = roleDataProvider;
            _logger = logger;
            _logger.LogDebug("RoleController::Constructor.");
        }

        [HttpGet("GetAllRoles/{fromRoles}")]
        public async Task<IActionResult> GetAllRoles(bool fromRoles)
        {
            try
            {
                var allRoles = await _roleDataProvider.GetAllRoles(fromRoles);
                return Ok(allRoles);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  RoleController::GetAllRoles." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("GetAllScreensPrivileges")]
        public async Task<IActionResult> GetAllScreensPrivileges()
        {
            try
            {
                var allScreenprevileges = await _roleDataProvider.GetRolePrivileges();
                return Ok(allScreenprevileges);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  RoleController::GetAllScreensPrevileges." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("GetAllScreensPrivilegesByRoleId/{id}")]
        public async Task<IActionResult> GetAllScreensPrivilegesByRoleId(int id)
        {

            try
            {
                var flagtype = await _roleDataProvider.GetRolePrivileges(id);
                return Ok(flagtype);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  RoleController::GetScreenPrivilegesByRoleId." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole([FromBody]Roles roles)
        {
            try
            {
                var result = await _roleDataProvider.AddOrUpdateRole(roles);
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
                _logger.LogDebug(" Error -- RoleController::AddRole." + ex.Message);
                _logger.LogDebug(" Error -- RoleController::AddRole." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateRole([FromBody]Roles role)
        {
            try
            {
                var result = await _roleDataProvider.AddOrUpdateRole(role, isupdate: true);
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
                _logger.LogDebug(" Error -- RoleController::UpdateRole." + ex.Message);
                _logger.LogDebug(" Error -- RoleController::UpdateRole." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpDelete("DeleteRole/{roleId}")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            try
            {
                var result = await _roleDataProvider.DeleteRole(roleId);
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
                _logger.LogDebug(" Error --  RoleController::Deleterole ." + ex.Message);
                _logger.LogDebug(" Error --  RoleController::Deleterole ." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("GetPrivilegedScreensByUserID/{userId}")]
        public async Task<IActionResult> GetPrivilegedScreensByUserID(int userId)
        {
            try
            {
                var getPrivilegedScreensByUserID = await _roleDataProvider.GetPrivilegedScreensByUserID(userId);
                return Ok(getPrivilegedScreensByUserID);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  RoleController::GetPrivilegedScreensByUserID ." + ex.Message);
                _logger.LogDebug(" Error --  RoleController::GetPrivilegedScreensByUserID ." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}