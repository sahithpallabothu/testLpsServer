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
    public class SoftwareDataProvider:ISoftwareDataProvider
    {
        private readonly string connectionString;
        private readonly AppSettings _appSettings;
        private ILoggerManager _logger;
        public SoftwareDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To Get the Software.
        /// </summary>
        /// <returns>Software</returns>
        public async Task<IEnumerable<Software>> GetAllSoftware()
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    return await sqlConnection.QueryAsync<Software>(
                        "spGetSoftware",
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
        /// To Add or Update Software.
        /// </summary>
        /// <param name="soft">Software</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>ok</returns>
        public async Task<string> AddOrUpdateSoftware(Software soft, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@action", isupdate ? "U" : "I");
                    dynamicParameters.Add("@ID", soft.Id);
                    dynamicParameters.Add("@software", soft.software);
                    dynamicParameters.Add("@active", soft.Active);
                    await sqlConnection.ExecuteAsync(
                        "spsoftware_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";
                    _logger.LogDebug(" Software Data Provider - AddOrUpdateSoftware - SQL Exception:" + ex.Message);
                    DisplayErrorMessage = MessageConstants.BadRequest;
                    return DisplayErrorMessage;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" Software Data Provider - AddOrUpdateSoftware - Exception:" + ex.Message);
                    _logger.LogDebug(" Software Data Provider - AddOrUpdateSoftware - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Delete the Software.
        /// </summary>
        /// <param name="ID">ID</param>
        /// <returns>ok</returns>
        public async Task<string> DeleteSoftware(int ID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@ID", ID);
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);
                    await sqlConnection.ExecuteAsync(
                        "spSoftware_Delete",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    //Getting result from out parameter.    
                    var response = dynamicParameters.Get<string>("@result");
                    if (response == MessageConstants.RECORDINUSE)
                    {
                        return response;
                    }
                    else
                    {
                        return "ok";
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" Software Data Provider - DeleteSoftware - Exception:" + ex.Message);
                    _logger.LogDebug(" Software Data Provider - DeleteSoftware - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }


    }
}
