using Arista_LPS_WebApp.Entities;
using Arista_LPS_WebApp.Helpers;
using Dapper;
using LPS;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public class DepartmentsDataProvider : IDepartmentsDataProvider
    {
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        private ILoggerManager _logger;
        public DepartmentsDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To Get the Departments.
        /// </summary>
        /// <param name="fromDepartment"></param>
        /// <returns>Departments</returns>
        public async Task<IEnumerable<Departments>> GetDepartments(bool fromDepartment)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@param", fromDepartment ? "DEPARTMENT" : "USER");
                    return await sqlConnection.QueryAsync<Departments>(
                        "spGetDepartment",
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
        /// To Add or Update Departments.
        /// </summary>
        /// <param name="department">department</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>ok</returns>
        public async Task<string> AddOrUpdateDepartments(Departments department, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@action", isupdate ? "U" : "I");
                    dynamicParameters.Add("@recId", department.DeptID);
                    dynamicParameters.Add("@deptName", department.Name);
                    dynamicParameters.Add("@deptDesc", department.Description);
                    dynamicParameters.Add("@active", department.Active);
                    await sqlConnection.ExecuteAsync(
                        "spDepartment_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch(Exception ex)
                {
                    _logger.LogDebug("Exception :: AddOrUpdateDepartments"+ex.Message);
                    _logger.LogDebug("Exception :: AddOrUpdateDepartments" + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// To delete the Departments.
        /// </summary>
        /// <param name="recID">recID</param>
        /// <returns>ok</returns>
        public async Task<string[]> DeleteDepartment(int recID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    string[] resultResponse = new string[2];
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@recID", recID);
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);
                    dynamicParameters.Add("@resuserList", dbType: DbType.String, direction: ParameterDirection.Output, size:5000);

                    await sqlConnection.ExecuteAsync(
                        "spDepartment_Delete",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);

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
                catch(Exception ex)
                {
                    _logger.LogDebug("Exception :: DeleteDepartment."+ex.Message);
                    _logger.LogDebug("Exception :: DeleteDepartment." + ex.StackTrace);
                    throw;
                }
            }
        }
    }
}
