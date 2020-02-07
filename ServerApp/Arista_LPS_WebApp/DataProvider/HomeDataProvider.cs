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

    public class HomeDataProvider : IHomeDataProvider
    {
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        private ILoggerManager _logger;
        private readonly int maxRecordCount;

        public HomeDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            maxRecordCount = Convert.ToInt32(_appSettings.RecordCount);
            _logger = logger;
        }

        /// <summary>
        /// To get the Inserts.
        /// </summary>
        /// <param name="search">search</param>
        /// <returns>inserts</returns>
        public async Task<IEnumerable<Inserts>> GetInserts(SearchDataForHome search)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    return await sqlConnection.QueryAsync<Inserts>(
                        "spGetInserts",
                        GetSearchParameters(search),
                    commandType: CommandType.StoredProcedure);
                }
                catch
                {
                    _logger.LogDebug("Exception :: GetInserts ");
                    throw;
                }
            }
        }

        /// <summary>
        /// To get the Jobs.
        /// </summary>
        /// <param name="search">search</param>
        /// <param name="isHome">isHome</param>
        /// <returns>Jobs</returns>
        public async Task<IEnumerable<JobDetail>> GetJobs(SearchDataForHome search)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();

                    _logger.LogDebug("Information :: GetJobs :: Info " + "QueryAsync Start");

                    return await sqlConnection.QueryAsync<JobDetail>(
                        "spGetJobs",
                         GetSearchParameters(search),
                    commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug("Exception :: GetJobs ERROR");
                    _logger.LogDebug("Exception :: GetJobs :: Stacktrace " + ex.StackTrace);
                    _logger.LogDebug("Exception :: GetJobs " + ex.Message);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// To Validate the Max Record Count in Home Search.
        /// </summary>
        /// <param name="search">search</param>
        /// <returns>bool</returns>
        public async Task<bool> ValidateJobAndInsertCount(SearchDataForHome search)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters = GetSearchParameters(search);
                    dynamicParameters.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    await sqlConnection.QueryAsync<dynamic>(                
                        "spGetJobsAndInsertsCount",
                        dynamicParameters,
                   commandType: CommandType.StoredProcedure);

                    var response = dynamicParameters.Get<int>("@result");
                    return response > maxRecordCount;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug("Exception :: ValidateJobAndInsertCount ERROR");
                    _logger.LogDebug("Exception :: ValidateJobAndInsertCount :: Stacktrace " + ex.StackTrace);
                    _logger.LogDebug("Exception :: ValidateJobAndInsertCount " + ex.Message);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// To build the Dynamic Parameters for search.
        /// </summary>
        /// <param name="search">search</param>
        /// <returns>DynamicParameters</returns>        
        private DynamicParameters GetSearchParameters(SearchDataForHome search)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@clientID", search.SelectedClientId);
            dynamicParameters.Add("@startDate", search.SelectedStartDate);
            dynamicParameters.Add("@endDate", search.selectedEndDate);
            dynamicParameters.Add("@custName", search.selectedCustName);
            dynamicParameters.Add("@custCode", search.selectedCustCode);
            dynamicParameters.Add("@appName", search.selectedAppName);
            dynamicParameters.Add("@appCode", search.selectedAppCode);
            return dynamicParameters;
        }
    }
}