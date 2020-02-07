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
    public class ContactsController : ControllerBase
    {
        private IContactsDataProvider _contactDataProvider;
        private ILoggerManager _logger;

        public ContactsController(IContactsDataProvider customerDataProvider, ILoggerManager logger)
        {
            _contactDataProvider = customerDataProvider;
            _logger = logger;
            _logger.LogDebug(" ContactsController :: Constructor.");
        }

        [HttpGet("GetContacts/{clientId}")]
        public async Task<IActionResult> GetContacts(int clientId)
        {
            try
            {
                var contactlist = await _contactDataProvider.GetContacts(clientId);
                return Ok(contactlist);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ContactController :: GetContacts." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("GetApplications/{clientId}/{contactID}")]
        public async Task<IActionResult> GetApplications(int clientId, int contactId)
        {
            try
            {
                var applist = await _contactDataProvider.GetApplications(clientId, contactId);
                return Ok(applist);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  ContactController::GetApplications." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPost("AddContact")]
        public async Task<IActionResult> AddContact([FromBody]Contacts Contact)
        {
            try
            {
                var result = await _contactDataProvider.AddOrUpdateContact(Contact);
                if (Convert.ToInt32(result) > 0)
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
                _logger.LogDebug(" Error --  ContactsController::AddContact." + ex.Message);
                _logger.LogDebug(" Error --  ContactsController::AddContact." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpPut("UpdateContact")]
        public async Task<IActionResult> UpdateContact([FromBody]Contacts Contact)
        {
            try
            {
                var result = await _contactDataProvider.AddOrUpdateContact(Contact, isupdate: true);
                if (Convert.ToInt32(result) > 0)
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
                _logger.LogDebug(" Error -- ContactsController::UpdateContact." + ex.Message);
                _logger.LogDebug(" Error -- ContactsController::UpdateContact." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpDelete("DeleteContact/{contactId}/{customerID}/{lastDeleteFlag}")]
        public async Task<IActionResult> DeleteContact(int contactId, int customerID, int lastDeleteFlag)
        {
            try
            {
                var result = await _contactDataProvider.DeleteContact(contactId, customerID, lastDeleteFlag);
                if (result == "ok")
                {
                    return Ok(new { message = result });
                }
                else if (result == MessageConstants.RECORDINUSE)
                {
                    return Ok(new { message = result, StatusCode = 2000 });
                }
                else if (result == "LAST_CONTACT")
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
                _logger.LogDebug(" Error --  ContactsController::DeleteContact." + ex.Message);
                _logger.LogDebug(" Error --  ContactsController::DeleteContact." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}