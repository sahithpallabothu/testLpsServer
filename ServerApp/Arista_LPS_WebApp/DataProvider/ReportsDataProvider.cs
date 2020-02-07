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
    public class ReportsDataProvider: IReportsDataProvider
    {
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        private ILoggerManager _logger;
        public ReportsDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To get all the Applications on the Seacrh criteria.
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns>applications</returns>
        public async Task<IEnumerable<Reports>> GetSearchDetails(Reports obj)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@Hold", obj.Hold);
                    dynamicParameters.Add("@Active", obj.Active);
                    dynamicParameters.Add("@CustomerFlag", obj.CustomerFlag);
                    dynamicParameters.Add("@PDF", obj.PDF);
                    dynamicParameters.Add("@EBPP", obj.Abpp);
                    dynamicParameters.Add("@ClientActive", obj.ClientActive);

                    return await sqlConnection.QueryAsync<Reports>(
                            "spGetReports",
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
