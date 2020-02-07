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
    public class SizeDataProvider : ISizeDataProvider
    {
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        private ILoggerManager _logger;

        public SizeDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To get the Size
        /// </summary>
        /// <param name="fromSize">fromSize</param>
        /// <returns>size</returns>
        public async Task<IEnumerable<Size>> GetSize(bool fromSize)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@param", fromSize ? "SIZE" : "CUSTOMER");
                    await sqlConnection.OpenAsync();
                    return await sqlConnection.QueryAsync<Size>(
                        "spGetSize",
                        null,
                        commandType: CommandType.StoredProcedure);
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// To Add or Update the Size.
        /// </summary>
        /// <param name="sizeobj">sizeobj</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>ok</returns>
        public async Task<string> AddOrUpdateSize(Size sizeobj, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@action", isupdate ? "U" : "I");
                    dynamicParameters.Add("@sizeID", sizeobj.SizeID);
                    dynamicParameters.Add("@size", sizeobj.size);

                    await sqlConnection.ExecuteAsync(
                        "spSize_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";
                    _logger.LogDebug(" Size Data Provider - AddOrUpdateSize - SQL Exception:" + ex.Message);
                    DisplayErrorMessage = MessageConstants.BadRequest;
                    return DisplayErrorMessage;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" Size Data Provider - AddOrUpdateSize - Exception:" + ex.Message);
                    _logger.LogDebug(" Size Data Provider - AddOrUpdateSize - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Delete Size.
        /// </summary>
        /// <param name="sizeID">sizeID</param>
        /// <returns>ok</returns>
        public async Task<string> DeleteSize(int sizeID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@sizeID", sizeID);
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);

                    await sqlConnection.ExecuteAsync(
                        "spSize_Delete",
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
                    _logger.LogDebug(" Size Data Provider - DeleteSize - Exception:" + ex.Message);
                    _logger.LogDebug(" Size Data Provider - DeleteSize - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }
    }
}
