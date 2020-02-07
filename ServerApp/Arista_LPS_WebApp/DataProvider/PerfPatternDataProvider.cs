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
    public class PerfPatternDataProvider : IPerfPatternDataProvider
    {
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        private ILoggerManager _logger;
        public PerfPatternDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To Get the Perforation Pattrens.
        /// </summary>
        /// <param name="isPerfPattern">isPerfPattern</param>
        /// <returns>PerfPattern</returns>
        public async Task<IEnumerable<PerfPatterns>> GetPerfPatterns(bool isPerfPattern)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@param", isPerfPattern ? "PERFPATTERN" : "CUSTOMER");
                    return await sqlConnection.QueryAsync<PerfPatterns>(
                        "spGetPerfPatterns",
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
        /// To Add or Update PerfPatterns.
        /// </summary>
        /// <param name="perfPatterns">perfPatterns</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>ok</returns>
        public async Task<string> AddOrUpdatePerfPatterns(PerfPatterns perfPatterns, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@action", isupdate ? "U" : "I");
                    dynamicParameters.Add("@perfPattern", perfPatterns.PerfPattern);
                    dynamicParameters.Add("@description", perfPatterns.Description);
                    await sqlConnection.ExecuteAsync(
                        "spPerfPatterns_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";
                    _logger.LogDebug(" PerfPattern Data Provider - AddOrUpdatePerfPatterns - SQL Exception:" + ex.Message);
                    DisplayErrorMessage = MessageConstants.BadRequest;
                    return DisplayErrorMessage;
                }
                catch(Exception ex)
                {
                    _logger.LogDebug(" PerfPattern Data Provider - AddOrUpdatePerfPatterns - Exception:" + ex.Message);
                    _logger.LogDebug(" PerfPattern Data Provider - AddOrUpdatePerfPatterns - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Delete the PerfPattern.
        /// </summary>
        /// <param name="perfPattern">perfPattern</param>
        /// <returns>ok</returns>
        public async Task<string> DeletePerfPatterns(int perfPattern)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@perfPattern", perfPattern);
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);

                    await sqlConnection.ExecuteAsync(
                        "spPerfPatterns_Delete",
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
                catch(Exception ex)
                {
                    _logger.LogDebug(" PerfPattern Data Provider - DeletePerfPatterns - Exception:" + ex.Message);
                    _logger.LogDebug(" PerfPattern Data Provider - DeletePerfPatterns - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }
    }
}
