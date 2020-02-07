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
    public class FeeDescriptionDataProvider : IFeeDescriptionDataProvider
    {
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        private ILoggerManager _logger;
        public FeeDescriptionDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To Get the FeeDescription.
        /// </summary>
        /// <param name="fromFeeDesc">fromFeeDesc</param>
        /// <returns>Feedescriptions</returns>
        public async Task<IEnumerable<FeeDescription>> GetFeeDescriptions(bool fromFeeDesc)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@param", fromFeeDesc ? "FEEDESC" : "USER");
                    return await sqlConnection.QueryAsync<FeeDescription>(
                        "spGetFeeDescriptions",
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
        /// To Add or Update Fee Description.
        /// </summary>
        /// <param name="feeDescription">feeDescription</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>ok</returns>
        public async Task<string> AddOrUpdateFeeDescription(FeeDescription feeDescription, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@action", isupdate ? "U" : "I");
                    dynamicParameters.Add("@recId", feeDescription.RecId);
                    dynamicParameters.Add("@feeDesc", feeDescription.Description);
                    dynamicParameters.Add("@active", feeDescription.Active);

                    await sqlConnection.ExecuteAsync(
                        "spFeesDesc_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";

                    _logger.LogDebug(" FeeDescription Data Provider - AddOrUpdateFeeDescription - SQL Exception:" + ex.Message);
                    DisplayErrorMessage = MessageConstants.BadRequest;

                    return DisplayErrorMessage;
                }
                catch(Exception ex)
                {
                    _logger.LogDebug(" FeeDescription Data Provider - AddOrUpdateFeeDescription - Exception:" + ex.Message);
                    _logger.LogDebug(" FeeDescription Data Provider - AddOrUpdateFeeDescription - Exception:" + ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// To delete FeeDescription.
        /// </summary>
        /// <param name="recID">recID</param>
        /// <returns>ok</returns>
        public async Task<string> DeleteFeeDescription(int recID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@recId", recID);
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);
                    await sqlConnection.ExecuteAsync(
                        "spFeesDesc_Delete",
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
                    _logger.LogDebug("FeeDescription Data Provider - DeleteFeeDescription - Exception:" + ex.Message);
                    _logger.LogDebug("FeeDescription Data Provider - DeleteFeeDescription - Exception:" + ex.Message);
                    throw;
                }
            }
        }
    }
}
