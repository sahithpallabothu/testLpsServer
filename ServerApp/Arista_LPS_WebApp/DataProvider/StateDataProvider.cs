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
    public class StateDataProvider : IStateDataProvider
    {
        #region  Private variables
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        private ILoggerManager _logger;
        #endregion  Private variables

        #region Constuctor
        public StateDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }
        #endregion Constuctor

        /// <summary>
        /// To Get the States.
        /// </summary>
        /// <param name="fromStates">fromStates</param>
        /// <returns>states</returns>
        public async Task<IEnumerable<States>> GetStates(bool fromStates)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@param", fromStates ? "STATES" : "USER");
                    return await sqlConnection.QueryAsync<States>(
                        "spGetStates",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure
                    );
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// To Add or Update States.
        /// </summary>
        /// <param name="state">state</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>ok</returns>
        public async Task<string> AddOrUpdateState(States state, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@action", isupdate ? "U" : "I");
                    dynamicParameters.Add("@stateID", state.StateID);
                    dynamicParameters.Add("@stateName", state.StateName);
                    dynamicParameters.Add("@stateCode", state.StateCode);
                    await sqlConnection.ExecuteAsync(
                        "spState_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";
                    _logger.LogDebug(" State Data Provider - AddOrUpdateState - SQL Exception:" + ex.Message);
                    DisplayErrorMessage = MessageConstants.BadRequest;
                    return DisplayErrorMessage;
                }
                catch(Exception ex)
                {
                    _logger.LogDebug(" State Data Provider - AddOrUpdateState - Exception:" + ex.Message);
                    _logger.LogDebug(" State Data Provider - AddOrUpdateState - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }
        /// <summary>
        /// Delete State
        /// </summary>
        /// <param name="stateID">stateID</param>
        /// <returns>ok</returns>
        public async Task<string> DeleteState(int stateID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@stateID", stateID);
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);

                    await sqlConnection.ExecuteAsync(
                        "spState_Delete",
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
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" State Data Provider - DeleteState - Exception:" + ex.Message);
                    _logger.LogDebug(" State Data Provider - DeleteState - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }
    }
}
