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
    public class AppTypeDataProvider : IAppTypeDataProvider
    {
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        private ILoggerManager _logger;
        public AppTypeDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To Get the AppTypes.
        /// </summary>
        /// <param name="fromDepartment">fromDepartment</param>
        /// <returns>apptypes</returns>
        public async Task<IEnumerable<AppType>> GetAppTypes(bool fromDepartment)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@param", fromDepartment ? "APPTYPE" : "CUSTOMER");
                    return await sqlConnection.QueryAsync<AppType>(
                        "spGetAppTypes",
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
        /// To Add or Update the AppType.
        /// </summary>
        /// <param name="apptype">apptype</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>ok</returns>
        public async Task<string> AddOrUpdateAppType(AppType apptype, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@action", isupdate ? "U" : "I");
                    dynamicParameters.Add("@appID", apptype.AppID);
                    dynamicParameters.Add("@description", apptype.Description);
                    dynamicParameters.Add("@active", apptype.Active);
                    dynamicParameters.Add("@sedcRateNumber", apptype.SEDCRateNumber);

                    await sqlConnection.ExecuteAsync(
                        "spAppType_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";
                    _logger.LogDebug(" AppType Data Provider - AddAppType - SQL Exception:" + ex.Message);
                    _logger.LogDebug(" AppType Data Provider - AddAppType - SQL Exception:" + ex.StackTrace);
                    DisplayErrorMessage = MessageConstants.BadRequest;
                    return DisplayErrorMessage;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" AppType Data Provider - AddAppType - Exception:" + ex.Message);
                    _logger.LogDebug(" AppType Data Provider - AddAppType - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Delete the AppTypes
        /// </summary>
        /// <param name="appID">appID</param>
        /// <returns>ok</returns>
        public async Task<string[]> DeleteAppType(string appID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    string[] resultResponse = new string[2];
                    await sqlConnection.OpenAsync();

                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@appID", appID);
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);
                    dynamicParameters.Add("@rescustList", dbType: DbType.String, direction: ParameterDirection.Output, size: 5000);

                    await sqlConnection.ExecuteAsync(
                        "spAppType_Delete",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    var response = dynamicParameters.Get<string>("@result");
                    var result = dynamicParameters.Get<string>("@rescustList");

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
                    _logger.LogDebug(" AppTypeDataProvider - DeleteAppType - Exception:" + ex.Message);
                    _logger.LogDebug(" AppTypeDataProvider - DeleteAppType - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }
    }
}
