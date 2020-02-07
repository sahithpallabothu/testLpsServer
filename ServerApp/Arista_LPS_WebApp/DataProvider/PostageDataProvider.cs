using Arista_LPS_WebApp.Entities;
using Arista_LPS_WebApp.Helpers;
using Dapper;
using LPS;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public class PostageDataProvider : IPostageDataProvider
    {
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        private ILoggerManager _logger;

        public PostageDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To Get the Postage by Job.
        /// </summary>
        /// <param name="jobNumber">jobNumber</param>
        /// <param name="runDate">runDate</param>
        /// <param name="recordId">recordId</param>
        /// <returns>JobDetail</returns>
        public async Task<IEnumerable<JobDetail>> getPostageByJob(string jobNumber, string runDate, string recordId)
        {
            IEnumerable<JobDetail> results = null;
            IEnumerable<Comments> commentsResults = null;
            IEnumerable<PostageAdditionalCharges> additionalChargesResults = null;

            try
            {
                using (var sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@jobNumber", jobNumber);
                    dynamicParameters.Add("@runDate", runDate == "-1" ? "" : runDate);
                    dynamicParameters.Add("@recordId", Convert.ToInt32(recordId));

                    results = await sqlConnection.QueryAsync<JobDetail>(
                        "spGetJobDetail",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);

                    commentsResults = await sqlConnection.QueryAsync<Comments>(
                       "spGetJobComments",
                       dynamicParameters,
                       commandType: CommandType.StoredProcedure);

                    additionalChargesResults = await sqlConnection.QueryAsync<PostageAdditionalCharges>(
                        "spGetAdditionalCharges",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);

                }

                results = GetPostageDetails(results, commentsResults, additionalChargesResults);
            }
            catch
            {
                throw;
            }
            return results;
        }

        /// <summary>
        /// To Get the Postage Details.
        /// </summary>
        /// <param name="jobResults">jobResults</param>
        /// <param name="postalTypeDetails">postalTypeDetails</param>
        /// <param name="commentsResults">commentsResults</param>
        /// <param name="additionalChargesResults">additionalChargesResults</param>
        /// <param name="postalTypes">postalTypes</param>
        /// <returns>JobDetail</returns>
        private IEnumerable<JobDetail> GetPostageDetails(IEnumerable<JobDetail> jobResults, IEnumerable<Comments> commentsResults, IEnumerable<PostageAdditionalCharges> additionalChargesResults)
        {
            List<JobDetail> jobDetails = null;
            jobDetails = jobResults.ToList();
            if (jobDetails.Count > 0)
            {
                jobDetails[0].Comments = new List<Comments>();
                jobDetails[0].AdditionalCharges = new List<PostageAdditionalCharges>();
                jobDetails[0].Comments.AddRange(commentsResults);
                jobDetails[0].AdditionalCharges.AddRange(additionalChargesResults);
            }

            return jobDetails.AsEnumerable();
        }

        /// <summary>
        /// To Update JobDetail.
        /// </summary>
        /// <param name="jobDetail">jobDetail</param>
        /// <returns>ok</returns>
        public async Task<string> updateJobDetail(JobDetail jobDetail)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    await sqlConnection.ExecuteAsync(
                        "spJobDetail_Update",
                        GetJobDetailParameters(jobDetail),
                        commandType: CommandType.StoredProcedure
                    );

                    return "ok";
                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";
                    _logger.LogDebug(" postage Data Provider - updateJobDetail - SQL Exception:" + ex.StackTrace);
                    _logger.LogDebug(" postage Data Provider - updateJobDetail - SQL Exception:" + ex.Message);
                    DisplayErrorMessage = MessageConstants.BadRequest;
                    return DisplayErrorMessage;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" postage Data Provider - updateJobDetail - Exception:" + ex.StackTrace);
                    _logger.LogDebug(" postage Data Provider - updateJobDetail - Exception:" + ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Get the JobDetail parameters.
        /// </summary>
        /// <param name="jobDetail">jobDetail</param>
        /// <returns>DynamicParameters</returns>
        private DynamicParameters GetJobDetailParameters(JobDetail jobDetail)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();

            try
            {
                dynamicParameters.Add("@recordId", jobDetail.RecordID);
                dynamicParameters.Add("@jobNumber", jobDetail.JobName);
                dynamicParameters.Add("@custCode", jobDetail.CustomerCode);
             
                dynamicParameters.Add("@post", jobDetail.PostFlag);
                dynamicParameters.Add("@postedDate", jobDetail.PostDate);
                dynamicParameters.Add("@trip", jobDetail.USPSTripCode);
                dynamicParameters.Add("@runDate", jobDetail.RunDate);

                dynamicParameters.Add("@meterPieces", jobDetail.MeterPieces);
                dynamicParameters.Add("@held", jobDetail.Held);
                dynamicParameters.Add("@foreign", jobDetail.Foreign);
                dynamicParameters.Add("@outOfBalance", jobDetail.OutOfBalance);

                dynamicParameters.Add("@additional_Charges_Xml", BuildAdditionalChargesData(jobDetail.AdditionalCharges));
                dynamicParameters.Add("@jobComments_Xml", BuildComments(jobDetail.Comments));
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" postage Data Provider - GetJobDetailParameters - Exception:" + ex.StackTrace);
                _logger.LogDebug(" postage Data Provider - GetJobDetailParameters - Exception:" + ex.Message);
                throw;
            }
            return dynamicParameters;
        }

        /// <summary>
        /// To build the Additional charges Data.
        /// </summary>
        /// <param name="additionalChargesData">additionalChargesData</param>
        /// <returns>xmlstring</returns>
        private string BuildAdditionalChargesData(List<PostageAdditionalCharges> additionalChargesData)
        {
            if (additionalChargesData != null)
            {
                StringBuilder childXml = new StringBuilder();

                childXml.Append("<root>");
                additionalChargesData.ForEach(charge =>
                {
                    childXml.AppendFormat
                     ("<row  feeId='{0}'  amount='{1}' chargeDescription='{2}' chargeDate='{3}' userName='{4}'  chargeType='{5}' />",
                     charge.FeeId, charge.ChargeAmount, charge.Description.ConvertToCData(), charge.ChargeDate, charge.UserName, charge.ChargeType.ConvertToCData());
                });
                childXml.Append("</root>");

                return childXml.ToString();
            }
            else
            {
                return "<root></root>";
            }
        }

        /// <summary>
        /// To build the JobComments.
        /// </summary>
        /// <param name="jobComments">jobComments</param>
        /// <returns>xmlstring</returns>
        private string BuildComments(List<Comments> jobComments)
        {
            if (jobComments != null)
            {
                StringBuilder roleXml = new StringBuilder();
                roleXml.Append("<root>");
                jobComments.ForEach(comment =>
                {
                    // DateTime temp=  Convert.ToDateTime(comment.Date);
                    roleXml.AppendFormat("<row  comments='{0}' userName='{1}' commentsDate ='{2}' />",
                    comment.JobComments.ConvertToCData(), comment.UserName, comment.CommentsDate);
                });
                roleXml.Append("</root>");

                return roleXml.ToString();
            }
            else
            {
                return "<root></root>";
            }
        }
    }
}
