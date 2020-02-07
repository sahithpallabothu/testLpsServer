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
    public class AdditionalChargesDataProvider : IAdditionalChargesDataProvider
    {
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        private ILoggerManager _logger;
        private readonly int maxRecordCount;

        public AdditionalChargesDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            maxRecordCount = Convert.ToInt32(_appSettings.RecordCount);
            _logger = logger;
        }

        /// <summary>
        /// To Add or Update the Additional Charges
        /// </summary>
        /// <param name="addlCharges">addlCharges</param>
        /// <returns>ok</returns>
        public async Task<string> AddOrUpdateAdditionalCharges(AdditionalChargesInfo addlCharges)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();

                    dynamicParameters.Add("@userName", addlCharges.userName);
                    dynamicParameters.Add("@addtionalCharges_Xml", await GetAdditionalChargesXml(addlCharges.additionalCharges));

                    await sqlConnection.ExecuteAsync(
                        "spAddlCharges_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch(Exception ex)
                {
                    _logger.LogDebug("Error :: AdditionalChargesDataProvider:: AddOrUpdateAdditionalCharges" + ex.Message);
                    _logger.LogDebug("Error :: AdditionalChargesDataProvider:: AddOrUpdateAdditionalCharges" + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// To get Additional Charges in the XML Format.
        /// </summary>
        /// <param name="additionalCharges">additionalCharges</param>
        /// <returns>xmlstring</returns>
        private async Task<string> GetAdditionalChargesXml(List<AdditionalCharges> additionalCharges)
        {
            StringBuilder childXml = new StringBuilder();
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();

                    childXml.Append("<root>");
                    additionalCharges.ForEach(n =>
                    {
                        childXml.AppendFormat
                        ("<row  JobNumber='{0}' JobDetailRunDate='{1}' ChargeType='{2}' FeeDesc='{3}' charge='{4}' Comment='{5}' RunDate='{6}' JobRecordId='{7}'/>",
                            n.jobName, n.jobDetailRunDate, n.chargeType, n.feeDesc, n.amount, n.description.ConvertToCData(), n.runDate, n.jobRecordId);
                    });
                    childXml.Append("</root>");
                }
                catch
                {
                    throw;
                }
            }

            return childXml.ToString();
        }

        /// <summary>
        /// To Validate the JoBNumber.
        /// </summary>
        /// <param name="jobNumber">jobNumber</param>
        /// <param name="clientCode">clientCode</param>
        /// <returns>JobDetails</returns>
        public async Task<IEnumerable<ACJobDetails>> ValidateJobNumber(string jobNumber, string clientCode)
        {

            try
            {
                using (var sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@jobNumber", jobNumber);
                    dynamicParameters.Add("@clientCode", clientCode);
                    dynamicParameters.Add("@param", "ADDLCHARGES"); // to validate job number and get rundate

                    return await sqlConnection.QueryAsync<ACJobDetails>(
                        "spGetJobDetail",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// To Check the Duplicate Additional Charges.
        /// </summary>
        /// <param name="addlCharges">addlCharges</param>
        /// <returns>ok</returns>
        public async Task<string> CheckDuplicateData(AdditionalCharges addlCharges)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@jobName", addlCharges.jobName);
                    dynamicParameters.Add("@amount", addlCharges.amount);
                    dynamicParameters.Add("@feeDesc", addlCharges.feeDesc);
                    dynamicParameters.Add("@runDate", addlCharges.runDate);
                    dynamicParameters.Add("@jobDetailRunDate", addlCharges.jobDetailRunDate);
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);
                    await sqlConnection.ExecuteAsync(
                        "spCheckDuplicateOfAddlCharges",
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
                    };
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// To get the Additional Charges on Search Criteria.
        /// </summary>
        /// <param name="customerCode">customerCode</param>
        /// <param name="ClientName">ClientName</param>
        /// <returns></returns>
        public async Task<IEnumerable<GetACDetails>> GetAdditionalChargesBasedOnSearch(string customerCode, string clientName)
        {
            var cCode = customerCode == "-1" ? "" : customerCode;
            var cName = clientName == "-1" ? "" : clientName;
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@customerCode", cCode);
                    dynamicParameters.Add("@clientName", cName);
                    return await sqlConnection.QueryAsync<GetACDetails>(
                        "spGetAllAdditionalCharges",
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
        /// To Update the Additional Charges.
        /// </summary>
        /// <param name="acDeatils">acDeatils</param>
        /// <returns>ok</returns>
        public async Task<string> UpdateAdditionalChargeByID(PostageAdditionalCharges acDeatils)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@action", "U");
                    dynamicParameters.Add("@acRecordID", acDeatils.ChargeId);
                    dynamicParameters.Add("@desc", acDeatils.ChargeType);
                    dynamicParameters.Add("@dateEntered", acDeatils.ChargeDate);
                    dynamicParameters.Add("@feeId", acDeatils.FeeId);//@feeId
                    dynamicParameters.Add("@charge", acDeatils.ChargeAmount);//@feeId
                    dynamicParameters.Add("@comments", acDeatils.Description);
                    dynamicParameters.Add("@userName", acDeatils.UserName);

                    await sqlConnection.ExecuteAsync(
                        "spAddlCharges_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";
                    _logger.LogDebug(" Addl Charge Data Provider - UpdateAdditionalChargeByID - SQL Exception:" + ex.Message);
                    DisplayErrorMessage = MessageConstants.BadRequest;

                    return DisplayErrorMessage;
                }
                catch(Exception ex)
                {
                    _logger.LogDebug(" Addl Charge Data Provider - UpdateAdditionalChargeByID - SQL Exception:" + ex.Message);
                    _logger.LogDebug(" Addl Charge Data Provider - UpdateAdditionalChargeByID - SQL Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// To get the Additional Charges Count.
        /// </summary>
        /// <param name="customerCode">customerCode</param>
        /// <param name="ClientName">ClientName</param>
        /// <returns></returns>
        public async Task<bool> GetAdditionalChargesCount(string customerCode, string clientName)
        {
            var cCode = customerCode == "-1" ? "" : customerCode;
            var cName = clientName == "-1" ? "" : clientName;
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@customerCode", cCode);
                    dynamicParameters.Add("@clientName", cName);
                    dynamicParameters.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await sqlConnection.QueryAsync<dynamic>(
                       "spGetAllAdditionalChargesCount",
                           dynamicParameters,
                   commandType: CommandType.StoredProcedure);

                    var response = dynamicParameters.Get<int>("@result");
                    return response > maxRecordCount;
                }
                catch
                {
                    throw;
                }
            }
        }

    }
}
