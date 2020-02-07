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
    public class InsertController : ControllerBase
    {
        #region private variables
        private IInsertDataProvider _insertDataProvider;
        private ILoggerManager _logger;
        #endregion

        public InsertController(IInsertDataProvider insertDataProvider, ILoggerManager logger)
        {
            _insertDataProvider = insertDataProvider;
            _logger = logger;
            _logger.LogDebug("InsertController::Constructor.");
        }

        //Add Insert
        [HttpPost("AddInsert")]
        public async Task<IActionResult> AddInsert([FromBody]Inserts[] inserts)
        {
            try
            {
                var result = await _insertDataProvider.AddOrUpdateInsert(inserts);
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
                _logger.LogDebug(" Error --  InsertController::AddInsert." + ex.Message);
                _logger.LogDebug(" Error --  InsertController::AddInsert." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        //To Update Insert. 
        [HttpPut("UpdateInsert")]
        public async Task<IActionResult> UpdateInsert([FromBody]Inserts[] inserts)
        {
            try
            {
                var resultOfUpdate = await _insertDataProvider.AddOrUpdateInsert(inserts, true);
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
                _logger.LogDebug(" Error --  InsertController::UpdateInsert." + ex.Message);
                _logger.LogDebug(" Error --  InsertController::UpdateInsert." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        /*Check if Insert Type And Insert Number Already Exist For Given Application Code*/
        [HttpGet("checkInsertType/{applicationCode}/{insertType}/{insertNumber}/{active}/{startDate}/{endDate}")]
        public async Task<bool> CheckInsertType(string applicationCode, int insertType, int insertNumber, int active, string startDate, string endDate)
        {
            try
            {
                int inserts = await _insertDataProvider.CheckInsertType(applicationCode, insertType, insertNumber, active, FormatDate(startDate), FormatDate(endDate));

                return inserts == 0 ? false : true;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error -- InsertsController::GetInserts." + ex.Message);
                return false;
            }
        }

        /*Get All the inserts */
        [HttpGet("GetAllInserts/{custName}/{custCode}/{startDate}/{endDate}/{active}")]
        public async Task<IActionResult> GetAllInserts(string custName, string custCode, string startDate, string endDate,bool active)
        {
            try
            {
                var inserts = await _insertDataProvider.GetAllInsertsOrByID(0,
                                                                            custName == "empty" ? null : custName,
                                                                            custCode == "empty" ? null : custCode,
                                                                            FormatDate(startDate),
                                                                            FormatDate(endDate),null, active);
                return Ok(inserts);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error -- InsertsController::GetInserts." + ex.Message);
                _logger.LogDebug(" Error -- InsertsController::GetInserts." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        /*Get All the applications*/
        [HttpGet("GetApplicationsForInserts")]
        public async Task<IActionResult> GetAllApplications()
        {
            try
            {
                var apps = await _insertDataProvider.GetApplicationsForInserts();
                return Ok(apps);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error -- InsertController::GetApplications ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("GetAllInsertTypes")]
        public async Task<IActionResult> GetAllInsertTypes()
        {
            try
            {
                var inserts = await _insertDataProvider.GetAllInsertTypes();
                return Ok(inserts);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error -- InsertsController::GetInsertTypes ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        /*get inserts based on application id*/
        [HttpGet("GetAllInsertsByApplicationID/{appID}/{startDate}/{endDate}/{screenCPLName}")]
        public async Task<IActionResult> GetAllInsertsByApplicationID(int appID, string startDate, string endDate, string screenCPLName)
        {
            try
            {
                var app = await _insertDataProvider.GetAllInsertsOrByID(appID, "","", FormatDate(startDate), FormatDate(endDate), screenCPLName);
                if (app == null)
                {
                    return NotFound();
                }
                return Ok(app);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  InsertsController::GetInsertsByApplicationId ." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpDelete("DeleteInsertByID/{RecID}")]
        public async Task<IActionResult> DeleteInsertByID(int recId)
        {
            try
            {
                var result = await _insertDataProvider.DeleteInsertByID(recId);
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
                _logger.LogDebug(" Error --  InsertsController::DeleteInsertByID." + ex.Message);
                _logger.LogDebug(" Error --  InsertsController::DeleteInsertByID." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        private string FormatDate(string dateToFormat)
        {
            dateToFormat = string.Join('/', dateToFormat.Split('-'));
            return dateToFormat == "00/00/00" ? null : dateToFormat;
        }
    }
}
