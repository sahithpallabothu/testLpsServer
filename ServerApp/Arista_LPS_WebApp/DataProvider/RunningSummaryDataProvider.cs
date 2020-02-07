using Arista_LPS_WebApp.Entities;
using Arista_LPS_WebApp.Helpers;
using Dapper;
using LPS;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public class RunningSummaryDataProvider : IRunningSummaryDataProvider
    {
        private readonly string connectionString;
        private readonly AppSettings _appSettings;
        private ILoggerManager _logger;
        public RunningSummaryDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To get all the JobDetails.
        /// </summary>
        /// <param name="runDate">runDate</param>
        /// <param name="isPrintWinSalem">isPrintWinSalem</param>
        /// <param name="isAutoRun">isAutoRun</param>
        /// <param name="postFlag">postFlag</param>
        /// <param name="trip">trip</param>
        /// <returns>jobDetails</returns>
        public async Task<IEnumerable<RunningSummary>> GetAllJobDetails(string runDate, int isPrintWinSalem, int isAutoRun, int postFlag, string trip)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@isPrintWinSalem", isPrintWinSalem);
                    dynamicParameters.Add("@isAutoRun", isAutoRun);
                    dynamicParameters.Add("@runDate", runDate);
                    dynamicParameters.Add("@postFlag", postFlag);
                    dynamicParameters.Add("@trip", trip);
                    var result = await sqlConnection.QueryAsync<RunningSummary>(
                         "spGetRunningSummary",
                         dynamicParameters,
                         commandType: CommandType.StoredProcedure);

                    return result;
                }
                catch
                {
                    _logger.LogDebug(" Error ::  RunningSummaryDataProvider::GetAllJobDetails .");
                    throw;
                }
            }
        }

        /// <summary>
        /// To Update the Post Flag.
        /// </summary>
        /// <param name="jobData">jobData</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>ok</returns>
        public async Task<string> UpdateJobPostFlag(List<UpdateJobPostFlag> jobData, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@jobdetails_Xml", GetJobDataXml(jobData));
                    await sqlConnection.ExecuteAsync(
                        "spRunningSummary_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" Error ::  RunningSummaryDataProvider::UpdateJobPostFlag ." + ex.Message);
                    _logger.LogDebug(" Error ::  RunningSummaryDataProvider::UpdateJobPostFlag ." + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// Build Job Details xml.
        /// </summary>
        /// <param name="jobData"></param>
        /// <returns></returns>
        private string GetJobDataXml(List<UpdateJobPostFlag> jobData)
        {
            StringBuilder childXml = new StringBuilder();

            childXml.Append("<root>");
            jobData.ForEach(n =>
            {
                childXml.AppendFormat
                ("<row  recordID='{0}' jobNumber='{1}' isPosted='{2}' trip='{3}' postDate='{4}'/>", n.recordId, n.jobNumber, n.isPosted, n.trip, n.runDate);
            });
            childXml.Append("</root>");

            return childXml.ToString();
        }
    }
}
