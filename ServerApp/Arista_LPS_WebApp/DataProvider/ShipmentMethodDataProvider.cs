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
    public class ShipmentMethodDataProvider : IShipmentMethodDataProvider
    {
        #region  Private variables
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        private ILoggerManager _logger;
        #endregion  Private variables

        #region Constuctor
        public ShipmentMethodDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }
        #endregion Constuctor

        /// <summary>
        /// Get ShipmentMethod
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ShipmentMethod>> GetShipmentMethod(bool fromShipment)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {           
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@param", fromShipment ? "SHIPMENT" : "USER");
                    return await sqlConnection.QueryAsync<ShipmentMethod>(
                        "spGetShipmentMethod",
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
        /// Add Or Update ShipmentMethod
        /// </summary>
        /// <param name="shipmentMethod">shipmentMethod</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>ok</returns>
        public async Task<string> AddOrUpdateShipmentMethod(ShipmentMethod shipmentMethod,bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@action", isupdate ? "U" : "I");
                    dynamicParameters.Add("@shipmentMethodID", shipmentMethod.ShipmentMethodID);
                    dynamicParameters.Add("@shipmentMethod", shipmentMethod.shipmentMethod);

                    await sqlConnection.ExecuteAsync(
                        "spShipmentMethod_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch(Exception ex)
                {
                    _logger.LogDebug(" Shipment Method Data Provider - AddOrUpdateShipmentMethod - Exception:" + ex.Message);
                    _logger.LogDebug(" Shipment Method Data Provider - AddOrUpdateShipmentMethod - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete ShipmentMethod
        /// </summary>
        /// <param name="shipmentMethodID">shipmentMethodID</param>
        /// <returns>ok</returns>
        public async Task<string> DeleteShipmentMethod(int shipmentMethodID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@shipmentMethodID", shipmentMethodID);
                    dynamicParameters.Add("@result" , dbType:DbType.String, direction: ParameterDirection.Output, size:30);
                     
                    await sqlConnection.ExecuteAsync(
                        "spShipmentMethod_Delete",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);

                    //Getting result from out parameter.    
                    var response =  dynamicParameters.Get<string>("@result"); 
                    
                    if(response == MessageConstants.RECORDINUSE){
                        return response;
                    }
                    else{
                        return "ok";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" Shipment Method Data Provider - DeleteShipmentMethod - Exception:" + ex.Message);
                    _logger.LogDebug(" Shipment Method Data Provider - DeleteShipmentMethod - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }
    }

}
