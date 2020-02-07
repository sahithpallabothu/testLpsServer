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
    public class RateDescriptionDataProvider : IRateDescriptionDataProvider
    {
        private readonly string connectionString;
        private readonly AppSettings _appSettings;
        private ILoggerManager _logger;
        public RateDescriptionDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To Get the RateDescription.
        /// </summary>
        /// <param name="fromRateType">fromRateType</param>
        /// <returns>RateDescriptions</returns>
        public async Task<IEnumerable<RateDescription>> GetRateDescriptions(bool fromRateType)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@param", fromRateType ? "RATETYPE" : "SERVICEAGREEMENT");
                    return await sqlConnection.QueryAsync<RateDescription>(
                        "spGetRateType",
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
        /// To Add or Update Rate Descriptions.
        /// </summary>
        /// <param name="rateDescription">rateDescription</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>ok</returns>
        public async Task<string> AddOrUpdateRateDescription(RateDescription rateDescription, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@action", isupdate ? "U" : "I");
                    dynamicParameters.Add("@rateTypeID", rateDescription.RateTypeID);
                    dynamicParameters.Add("@description", rateDescription.Description);
                    dynamicParameters.Add("@active", rateDescription.Active);
                    await sqlConnection.ExecuteAsync(
                        "spRateType_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";
                    _logger.LogDebug(" RateDescription Data Provider - AddOrUpdateRateDescription - SQL Exception:" + ex.Message);
                    DisplayErrorMessage = MessageConstants.BadRequest;
                    return DisplayErrorMessage;
                }
                catch(Exception ex)
                {
                    _logger.LogDebug(" RateDescription Data Provider - AddOrUpdateRateDescription - Exception:" + ex.Message);
                    _logger.LogDebug(" RateDescription Data Provider - AddOrUpdateRateDescription - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Delete the RateDescription.
        /// </summary>
        /// <param name="rateTypeID">rateTypeID</param>
        /// <returns>ok</returns>
        public async Task<string> DeleteRateDescription(int rateTypeID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@rateTypeID", rateTypeID);
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);
                    await sqlConnection.ExecuteAsync(
                        "spRateType_Delete",
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
                    _logger.LogDebug(" RateDescription Data Provider - DeleteRateDescription - Exception:" + ex.Message);
                    _logger.LogDebug(" RateDescription Data Provider - DeleteRateDescription - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }
    }
}

