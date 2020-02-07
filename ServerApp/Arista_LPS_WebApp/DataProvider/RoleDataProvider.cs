using Arista_LPS_WebApp.Entities;
using Arista_LPS_WebApp.Helpers;
using Dapper;
using LPS;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public class RoleDataProvider : IRoleDataProvider
    {
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        private ILoggerManager _logger;

        public RoleDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To GetAllRoles.
        /// </summary>
        /// <param name="fromRoles">fromRoles</param>
        /// <returns>roles</returns>
        public async Task<IEnumerable<Roles>> GetAllRoles(bool fromRoles)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@param", fromRoles ? MessageConstants.MODULE_ROLE : MessageConstants.MODULE_USERS);
                    return await sqlConnection.QueryAsync<Roles>(
                        "spGetRoles",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// To Get the Role Privileges.
        /// </summary>
        /// <param name="roleID">roleID</param>
        /// <returns>rolePrivileges</returns>
        public async Task<IEnumerable<RolePrivileges>> GetRolePrivileges(int roleID = 0)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@roleId", roleID);
                    return await sqlConnection.QueryAsync<RolePrivileges>(
                    "spGetFuntions",
                    dynamicParameters,
                    commandType: CommandType.StoredProcedure);
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// To Add or Update Role.
        /// </summary>
        /// <param name="role">role</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>ok</returns>
        public async Task<string> AddOrUpdateRole(Roles role, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {

                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();

                    dynamicParameters.Add("@action", isupdate ? "U" : "I");
                    dynamicParameters.Add("@currentRoleId", role.RoleId);
                    dynamicParameters.Add("@roleName", role.RoleName.Trim());
                    dynamicParameters.Add("@roleComments", role.RoleDescriprion);
                    dynamicParameters.Add("@isActive", role.RoleActive);
                    dynamicParameters.Add("@role_Privileges_Xml", GetScreenPrivileges(role.Privileges));
                    await sqlConnection.ExecuteAsync(
                        "spRole_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch
                {
                    _logger.LogDebug("Exception :: AddOrUpdateRole ");
                    throw;
                }
            }
        }

        /// <summary>
        /// To Delete the Role.
        /// </summary>
        /// <param name="roleid">roleid</param>
        /// <returns>ok</returns>
        public async Task<string[]> DeleteRole(int roleid)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    string[] resultResponse = new string[2];
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);
                    dynamicParameters.Add("@roleID", roleid);
                    dynamicParameters.Add("@resuserList", dbType: DbType.String, direction: ParameterDirection.Output, size: 5000);

                    await sqlConnection.ExecuteAsync(
                        "spRole_Delete",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);

                    //Getting result from out parameter.    
                    var response = dynamicParameters.Get<string>("@result");
                    var result = dynamicParameters.Get<string>("@resuserList");

                    if (response == MessageConstants.RECORDINUSE)
                    {
                        resultResponse[0] = response;
                        resultResponse[1] = result;
                    }
                    else
                    {
                        resultResponse[0] = "ok";
                    }
                    return resultResponse;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug("Exception :: DeleteRole "+ ex.Message);
                    _logger.LogDebug("Exception :: DeleteRole " + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Get the Screen Privileges.
        /// </summary>
        /// <param name="rolePrivileges">rolePrivileges</param>
        /// <returns>xmlstring</returns>
        private string GetScreenPrivileges(List<RolePrivileges> rolePrivileges)
        {
            StringBuilder childXml = new StringBuilder();
            childXml.Append("<root>");

            rolePrivileges.ForEach(screen =>
            {
                childXml.AppendFormat("<row ScreenName='{0}' ReadPrivilage='{1}' InsertPrivilage='{2}' UpdatePrivilage='{3}' DeletePrivilage='{4}' />",
                    screen.ScreenName, screen.read, screen.insert, screen.update, screen.delete);
            });

            childXml.Append("</root>");
            return childXml.ToString();
        }

        /// <summary>
        /// To Get the Prviledged screens by userID.
        /// </summary>
        /// <param name="userID">userID</param>
        /// <returns></returns>
        public async Task<IEnumerable<RolePrivileges>> GetPrivilegedScreensByUserID(int userID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@userID", userID);

                    return await sqlConnection.QueryAsync<RolePrivileges>(
                           "spGetPrivilegeByUserID",
                           dynamicParameters,
                           commandType: CommandType.StoredProcedure);
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
