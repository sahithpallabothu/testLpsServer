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
    public class DepartmentController : ControllerBase
    {
        private IDepartmentsDataProvider _departmentsDataProvider;
        private ILoggerManager _logger;

        public DepartmentController(IDepartmentsDataProvider departmentsDataProvider, ILoggerManager logger)
        {
            _departmentsDataProvider = departmentsDataProvider;
            _logger = logger;
            _logger.LogDebug(" DepartmentController::Constructor.");
        }

        [HttpGet("GetAllDepartments/{fromDepartment}")]
        public async Task<IActionResult> GetDepartments(bool fromDepartment)
        {
            try
            {
                var departments = await _departmentsDataProvider.GetDepartments(fromDepartment);
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  DepartmentController::GetDepartments." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddDepartment")]
        public async Task<IActionResult> AddDepartment([FromBody]Departments departments)
        {
            try
            {
                var result = await _departmentsDataProvider.AddOrUpdateDepartments(departments);
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
                _logger.LogDebug(" Error --  DepartmentsController::AddDepartments." + ex.Message);
                _logger.LogDebug(" Error --  DepartmentsController::AddDepartments." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("UpdateDepartment")]
        public async Task<IActionResult> UpdateDepartment([FromBody]Departments department)
        {
            try
            {
                var result = await _departmentsDataProvider.AddOrUpdateDepartments(department, true);
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
                _logger.LogDebug(" Error --  DepartmentController::UpdateDepartment." + ex.Message);
                _logger.LogDebug(" Error --  DepartmentController::UpdateDepartment." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpDelete("DeleteDepartment/{recordID}")]
        public async Task<IActionResult> DeleteDepartment(int recordID)
        {
            try
            {
                var result = await _departmentsDataProvider.DeleteDepartment(recordID);
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
                _logger.LogDebug(" Error --  DepartmentController::DeleteDepartment." + ex.Message);
                _logger.LogDebug(" Error --  DepartmentController::DeleteDepartment." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}
