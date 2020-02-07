using Arista_LPS_WebApp.DataProvider;
using Arista_LPS_WebApp.Entities;
using Arista_LPS_WebApp.Helpers;
using LPS;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserDataProvider _userDataProvider;
        private readonly ILoggerManager _logger;
        private readonly IRoleDataProvider _roleDataProvider;

        public UserController(IUserDataProvider userDataProvider, ILoggerManager logger, IRoleDataProvider roleDataProvider)
        {
            _roleDataProvider = roleDataProvider;
            _userDataProvider = userDataProvider;
            _logger = logger;
            _logger.LogDebug("UsersController::Constructor.");
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(UserList userParam)
        {  
            try
            {
                var user = await _userDataProvider.AuthenticateUser(userParam.UserName);
                if (user == null)
                    return BadRequest(new { message = "Username is incorrect." });
                if (user.IsUserExists)
                    return BadRequest(new { message = "Access denied. Please contact your administrator." });
                if (user.UserId > 0)
                {
                    var getUserPrivilegedScreens = await _roleDataProvider.GetPrivilegedScreensByUserID(user.UserId);
                    if (getUserPrivilegedScreens != null)
                    {
                        user.RolePrivilegeslist = getUserPrivilegedScreens;
                    }
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  UserController::Authenticate." + ex.Message);
                _logger.LogDebug(" Error --  UserController::Authenticate." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        // Get All Users.  
        [HttpGet("GetAllUsers")]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _userDataProvider.GetUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  UsersController::GetAllUsers." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _userDataProvider.GetUserById(id);
                if (user == null)
                {
                    return Ok(user);
                }           
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  UsersController::GetUserById." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        //Add User.
        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromBody]UserList userList)
        {
            try
            {
                var result = await _userDataProvider.AddOrUpdateUser(userList);
                if (result != "ok")
                    return BadRequest(new { message = result });
                return Ok();               
               
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  UserController::AddUser." + ex.Message);
                _logger.LogDebug(" Error --  UserController::AddUser." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        //Update User.
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody]UserList userList)
        {
            try
            {
                var result = await _userDataProvider.AddOrUpdateUser(userList, true);
                if (result != "ok")
                    return BadRequest(new { message = result });
                return Ok(); 
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  UserController::UpdateUser." + ex.Message);
                _logger.LogDebug(" Error --  UserController::UpdateUser." + ex.StackTrace);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("GetAllLocations")]
        public async Task<IActionResult> GetLocations()
        {
            try
            {
                var locations = await _userDataProvider.GetLocations();
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  UserController::GetLocations." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("ValidateUserInDomain/{userName}")]
        public IActionResult ValidateUserInDomain(string userName)
        {
            try
            {
                var result = _userDataProvider.ValidateUserInDomain(userName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  UserController::ValidateUserInDomain." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("GetDomainName")]
        public IActionResult GetDomainName()
        {
            try
            {
                var result = _userDataProvider.GetDomainName();
                return Ok(new { domainName = result });
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  UserController::GetDomainName." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }

        [HttpGet("CheckAdminExist")]
        public async Task<IActionResult> CheckAdminExist()
        {
            try
            {
                var result = await _userDataProvider.CheckAdminExist();
                if (Convert.ToInt32(result) >= 1)
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }

            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Error --  UserController::CheckAdminExist." + ex.Message);
                return BadRequest(new { message = MessageConstants.BadRequest });
            }
        }
    }
}