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
    public class FlagDataProvider : IFlagDataProvider
    {
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        private ILoggerManager _logger;

        public FlagDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To Get the Flags.
        /// </summary>
        /// <param name="fromFlag">fromFlag</param>
        /// <returns>flags</returns>
        public async Task<IEnumerable<Flagtype>> GetFlags(bool fromFlag)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@param", fromFlag ? "FLAG" : "CUSTOMER");
                    return await sqlConnection.QueryAsync<Flagtype>(
                    "spGetFlags",
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
        /// To Add or Update FlagType.
        /// </summary>
        /// <param name="flagtype">flagtype</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>ok</returns>
        public async Task<string> AddOrUpdateFlagType(Flagtype flagtype, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@action", isupdate ? "U" : "I");
                    dynamicParameters.Add("@flagId", flagtype.FlagId);
                    dynamicParameters.Add("@flagDesc", flagtype.Description);
                    await sqlConnection.ExecuteAsync(
                        "spFlagType_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";
                    _logger.LogDebug(" FlagType Data Provider - AddOrUpdateFlagType - SQL Exception:" + ex.Message);
                    DisplayErrorMessage = MessageConstants.BadRequest;
                    return DisplayErrorMessage;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" FlagType Data Provider - AddOrUpdateFlagType Exception:" + ex.Message);
                    _logger.LogDebug(" FlagType Data Provider - AddOrUpdateFlagType Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Delete Flag.
        /// </summary>
        /// <param name="flagTypeID">flagTypeID</param>
        /// <returns>ok</returns>
        public async Task<string> DeleteFlag(int flagTypeID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@flagId", flagTypeID);
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);
                    await sqlConnection.ExecuteAsync(
                        "spFlagType_Delete",
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
                catch(Exception ex)
                {
                    _logger.LogDebug(" FlagType Data Provider - DeleteFlag Exception:" + ex.Message);
                    _logger.LogDebug(" FlagType Data Provider - DeleteFlag Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }
    }
}
