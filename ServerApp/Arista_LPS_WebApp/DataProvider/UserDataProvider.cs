using Arista_LPS_WebApp.Entities;
using Arista_LPS_WebApp.Helpers;
using Dapper;
using LPS;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public class UserDataProvider : IUserDataProvider
    {
        private readonly string connectionString;
        private readonly AppSettings _appSettings;
        private ILoggerManager _logger;
        public UserDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To Add or Update User.
        /// </summary>
        /// <param name="userList">userList</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>ok</returns>
        public async Task<string> AddOrUpdateUser(UserList userList, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@action", isupdate ? "U" : "I");
                    dynamicParameters.Add("@userID", userList.UserId);
                    dynamicParameters.Add("@firstName", userList.FirstName);
                    dynamicParameters.Add("@lastName", userList.LastName);
                    dynamicParameters.Add("@userName", userList.UserName);
                    dynamicParameters.Add("@department", userList.DepartmentId);
                    dynamicParameters.Add("@location", userList.LocationId);
                    dynamicParameters.Add("@initials", userList.Initials);
                    dynamicParameters.Add("@is_Active", userList.IsActive);
                    dynamicParameters.Add("@user_Roles_Xml", BuildUserRoles(userList.UserRoles));
                    dynamicParameters.Add("@email", userList.EmailAddress);
                    dynamicParameters.Add("@extension", userList.Extension);
                    dynamicParameters.Add("@cellPhone", userList.CellPhone);
                    await sqlConnection.ExecuteAsync(
                        "spUserList_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch (Exception ex)
                {
                    _logger.LogDebug("Exception :: AddOrUpdateUser " + ex.Message);
                    _logger.LogDebug("Exception :: AddOrUpdateUser " + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Build the User Roles.
        /// </summary>
        /// <param name="UserRoles"></param>
        /// <returns>xmlstring</returns>
        private string BuildUserRoles(List<UserRoles> UserRoles)
        {
            try
            {
                StringBuilder roleXml = new StringBuilder();
                roleXml.Append("<root>");
                UserRoles.ForEach(role =>
                {
                    roleXml.AppendFormat("<row role_Id ='{0}' />", role.RoleId);
                });
                roleXml.Append("</root>");

                return roleXml.ToString();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// To Authenticate User.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<UserList> AuthenticateUser(string userName)
        {
            try
            {
                string username = string.Empty;
                // Get the Current Logged In User
                username = GetCurrentUser();

                using (var sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@uname", userName);
                    dynamic userdata = await sqlConnection.QueryAsync<dynamic>(
                      "spAuthenticate",
                      dynamicParameters,
                      commandType: CommandType.StoredProcedure);

                    UserList ul = new UserList();

                    // return null if user not found
                    if (userdata.Count == 0)
                    {
                        List<UserList> users = new List<UserList>();
                        users = GetADUsers();

                        users.ForEach(us =>
                        {
                            if (us.UserName.ToLower() == userName.ToLower())
                            {
                                ul.IsUserExists = true;
                            }
                        });

                        return ul.IsUserExists ? ul : null;
                    }

                    Slapper.AutoMapper.Cache.ClearInstanceCache();
                    Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(UserList), new List<string> { "UserId" });
                    Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(Roles), new List<string> { "RoleId" });

                    var user = (Slapper.AutoMapper.MapDynamic<UserList>(userdata) as IEnumerable<UserList>).ToList().ElementAt(0);

                    if (user.IsPrivilegeExists == "N")
                    {
                        ul.IsUserExists = true;
                        return ul;
                    }

                    string AllRoles = string.Join(",", user.UserRoles.Select(item => item.RoleName.ToString()));

                    // authentication successful so generate jwt token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                     new Claim(ClaimTypes.Name, user.UserId.ToString()),
                     new Claim(ClaimTypes.Role, AllRoles)
                        }),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    user.Token = tokenHandler.WriteToken(token);

                    // remove password before returning
                    user.Password = null;

                    return user;
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Exception :: AuthenticateUser" + ex.StackTrace);
                _logger.LogDebug("Exception :: AuthenticateUser" + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// To Get Currently LoggedIn User.
        /// </summary>
        /// <returns>currentUser</returns>
        private string GetCurrentUser()
        {
            try
            {
                string currentUser = string.Empty;
                if (WindowsIdentity.GetCurrent().Name.Contains('\\'))
                {
                    currentUser = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                }
                return currentUser;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// To GetUsers.
        /// </summary>
        /// <param name="userID">userID</param>
        /// <returns>users</returns>
        public IEnumerable<UserList> GetUsers(int userID = 0)
        {
            try
            {
                List<UserList> users = new List<UserList>();
                List<UserList> localusers = new List<UserList>();

                localusers = GetLocalUsers();

                if (localusers == null)
                    return null;

                return localusers.AsEnumerable();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// To Get ADUsers.
        /// </summary>
        /// <returns>users</returns>
        private List<UserList> GetADUsers()
        {
            try
            {
                _logger.LogDebug(" User --  UserDataProvider::GetADUsers Start.");

                List<UserList> users = new List<UserList>();

                using (var searcher = new PrincipalSearcher(new UserPrincipal(new PrincipalContext(ContextType.Domain, _appSettings.DOMAIN))))
                {
                    _logger.LogDebug(" User --  UserDataProvider:: PrincipalSearcher Start.");

                    List<UserPrincipal> usersPrin = searcher.FindAll().Select(u => (UserPrincipal)u).ToList();

                    _logger.LogDebug(" User --  UserDataProvider:: UserPrincipal Start. : " + usersPrin.Count);

                    foreach (var u in usersPrin)
                    {
                        if (u.Enabled != null && u.Enabled == true)
                        {
                            UserList ul = new UserList();

                            ul.FirstName = !string.IsNullOrEmpty(u.GivenName) ? u.GivenName : string.Empty;
                            ul.LastName = !string.IsNullOrEmpty(u.Surname) ? u.Surname : string.Empty;
                            ul.UserName = !string.IsNullOrEmpty(u.SamAccountName) ? u.SamAccountName : string.Empty;
                            ul.EmailAddress = !string.IsNullOrEmpty(u.EmailAddress) ? u.EmailAddress : string.Empty;
                            ul.CellPhone = !string.IsNullOrEmpty(u.VoiceTelephoneNumber) ? u.VoiceTelephoneNumber : string.Empty;
                            ul.IsActive = true;

                            users.Add(ul);
                        }
                    }
                }
                _logger.LogDebug(" User --  UserDataProvider::GetADUsers End.");

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Exception :: GetADUsers " + ex.Message);
                _logger.LogDebug("Exception :: GetADUsers " + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// To Get User.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>users</returns>
        public async Task<IEnumerable<UserList>> GetUserById(string userId)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@userName", userId);
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);
                    dynamic userdata = await sqlConnection.QueryAsync<dynamic>(
                        "spGetUserList",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);

                    var response = dynamicParameters.Get<string>("@result");

                    if (response == MessageConstants.NOTINDB)
                    {
                        return null;
                    }

                    Slapper.AutoMapper.Cache.ClearInstanceCache();
                    Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(UserList), new List<string> { "UserId" });
                    Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(Roles), new List<string> { "RoleId" });

                    List<UserList> users = null;
                    users = (Slapper.AutoMapper.MapDynamic<UserList>(userdata) as IEnumerable<UserList>).ToList();

                    // return null if user not found
                    if (users == null)
                        return null;

                    return users;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug("Exception :: GetADUsers " + ex.Message);
                    _logger.LogDebug("Exception :: GetADUsers " + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Get LocalUsers.
        /// </summary>
        /// <returns>localUsers</returns>
        private List<UserList> GetLocalUsers()
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);
                    dynamic userdata = sqlConnection.Query<dynamic>(
                        "spGetUserList",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);

                    var response = dynamicParameters.Get<string>("@result");

                    if (response == MessageConstants.NOTINDB)
                    {
                        return null;
                    }

                    Slapper.AutoMapper.Cache.ClearInstanceCache();
                    Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(UserList), new List<string> { "UserId" });
                    Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(Roles), new List<string> { "RoleId" });

                    List<UserList> users = null;
                    users = (Slapper.AutoMapper.MapDynamic<UserList>(userdata) as IEnumerable<UserList>).ToList();

                    // return null if user not found
                    if (users == null)
                        return null;

                    return users;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug("Exception :: GetLocalUsers " + ex.Message);
                    _logger.LogDebug("Exception :: GetLocalUsers " + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Get Locations.
        /// </summary>
        /// <returns>locations</returns>
        public async Task<IEnumerable<LocationClass>> GetLocations()
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    return await sqlConnection.QueryAsync<LocationClass>(
                        "spGetLocation",
                        null,
                        commandType: CommandType.StoredProcedure);
                }
                catch(Exception ex)
                {
                    _logger.LogDebug("Exception :: GetLocations " + ex.Message);
                    _logger.LogDebug("Exception :: GetLocations " + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Validate the Users in Domain.
        /// </summary>
        /// <param name="userName">userName</param>
        /// <returns></returns>
        public UserList ValidateUserInDomain(string userName)
        {
            UserList ul = new UserList();
            var searcher = new PrincipalSearcher(new UserPrincipal(new PrincipalContext(ContextType.Domain, _appSettings.DOMAIN)));
            List<UserPrincipal> usersPrin = searcher.FindAll().Select(u => (UserPrincipal)u).ToList();
            foreach (var u in usersPrin)
            {
                if (u.Enabled != null && u.Enabled == true)
                {
                    if (u.SamAccountName.ToLower() == userName.ToLower())
                    {
                        ul.FirstName = !string.IsNullOrEmpty(u.GivenName) ? u.GivenName : string.Empty;
                        ul.LastName = !string.IsNullOrEmpty(u.Surname) ? u.Surname : string.Empty;
                        ul.UserName = !string.IsNullOrEmpty(u.SamAccountName) ? u.SamAccountName : string.Empty;
                        ul.EmailAddress = !string.IsNullOrEmpty(u.EmailAddress) ? u.EmailAddress : string.Empty;
                        ul.CellPhone = !string.IsNullOrEmpty(u.VoiceTelephoneNumber) ? u.VoiceTelephoneNumber : string.Empty;
                        ul.IsActive = true;
                        ul.IsUserExists = true;
                        break;
                    }
                    else
                    {
                        ul.IsUserExists = false;
                    }
                }
            }
            return ul;
        }

        /// <summary>
        /// To Get DomainName.
        /// </summary>
        /// <returns></returns>
        public string GetDomainName()
        {
            return _appSettings.DOMAIN;
        }

        /// <summary>
        /// To Check AdminExists.
        /// </summary>
        /// <returns></returns>
        public async Task<int> CheckAdminExist()
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@count", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 4);

                    await sqlConnection.ExecuteAsync(
                        "spCheckAdminExist",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);

                    var response = dynamicParameters.Get<int>("@count");
                    return Convert.ToInt32(response);
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}