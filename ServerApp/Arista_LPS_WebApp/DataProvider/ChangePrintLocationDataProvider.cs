using Arista_LPS_WebApp.Entities;
using Arista_LPS_WebApp.Helpers;
using Dapper;
using LPS;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public class ChangePrintLocationDataProvider : IChangePrintLocationDataProvider
    {
        private readonly string connectionString;
        private readonly AppSettings _appSettings;
        private ILoggerManager _logger;

        public ChangePrintLocationDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To get the Application Data.
        /// </summary>
        /// <param name="isPrintWinSalem">isPrintWinSalem</param>
        /// <returns>applications</returns>
        public async Task<IEnumerable<ChangePrintLocation>> GetAllApplicationData(int isPrintWinSalem)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@isPrintWinSalem", isPrintWinSalem);
                    var result = await sqlConnection.QueryAsync<ChangePrintLocation>(
                         "spGetChangePrintLocation",
                         dynamicParameters,
                         commandType: CommandType.StoredProcedure);
                    return result;
                }
                catch
                {
                    _logger.LogDebug("Error :: ChangePrintLocationDataProvider:: GetAllApplicationData");
                    throw;
                }
            }
        }

        /// <summary>
        /// To Update the Application's Print Location.
        /// </summary>
        /// <param name="applicationLocationData">applicationLocationData</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>ok</returns>
        public async Task<string> UpdateApplicationPrintLocation(List<UpdateApplicationLocation> applicationLocationData, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@printLocation_Xml", GetApplicationLocationXml(applicationLocationData));
                    await sqlConnection.ExecuteAsync(
                        "spApplPrintLocation_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch
                {
                    _logger.LogDebug("Error :: ChangePrintLocationDataProvider:: UpdateApplicationPrintLocation");
                    throw;
                }
            }
        }

        /// <summary>
        /// To Get the Application Location xml.
        /// </summary>
        /// <param name="applicationLocationData">applicationLocationData</param>
        /// <returns>xmlstring</returns>
        private string GetApplicationLocationXml(List<UpdateApplicationLocation> applicationLocationData)
        {
            StringBuilder childXml = new StringBuilder();

            childXml.Append("<root>");
            applicationLocationData.ForEach(n =>
                   {
                       childXml.AppendFormat
                       ("<row  ApplicationId='{0}' isPrintWinSalem='{1}'/>", n.ApplicationId, n.isPrintWinSalem);
                   });
            childXml.Append("</root>");

            return childXml.ToString();
        }
    }
}