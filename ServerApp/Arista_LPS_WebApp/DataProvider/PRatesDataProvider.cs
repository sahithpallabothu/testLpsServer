using Arista_LPS_WebApp.Helpers;
using Arista_LPS_WebApp.Entities;

using Dapper;
using LPS;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;


namespace Arista_LPS_WebApp.DataProvider
{
    public class PRateDataProvider : IPRateDataProvider
    {
        private readonly string connectionString;
        private readonly AppSettings _appSettings;
        private ILoggerManager _logger;
        public PRateDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        public async Task<IEnumerable<PRates>> GetPRates()
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {            
                try
                {
                    await sqlConnection.OpenAsync();
                    return await sqlConnection.QueryAsync<PRates>(
                        "spGetPostageRates",
                        null,
                        commandType: CommandType.StoredProcedure);
                }
                catch
                {
                  throw;
                }
            }
        }

        public async Task<string> AddOrUpdatePRates(PRates Prate,bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@action", isupdate ? "U" : "I");
                    dynamicParameters.Add("@postalTypeId", Prate.PostalTypeID);
                    dynamicParameters.Add("@typeCode", Prate.TypeCode);
                    dynamicParameters.Add("@rate",Prate.Rate);
                    dynamicParameters.Add("@date", Prate.StartDate);
                    dynamicParameters.Add("@description", Prate.Description);
                    dynamicParameters.Add("@displayOrder", Prate.DisplayOrder);
                    dynamicParameters.Add("@active", Prate.Active);
                    await sqlConnection.ExecuteAsync(
                        "spPostageRates_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";
                   _logger.LogDebug(" PRate Data Provider - AddOrUpdatePRates - SQL Exception:" + ex.Message);
                    DisplayErrorMessage = MessageConstants.BadRequest;
                    return DisplayErrorMessage;
                }
                catch
                {
                  throw;
                }
            }
        }

        public async Task<string> DeletePRate(int pRateID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@postalTypeID", pRateID);
                    await sqlConnection.ExecuteAsync(
                        "spPostageRates_Delete",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch
                {
                  throw ;
                }
            }
        }

    }
}

