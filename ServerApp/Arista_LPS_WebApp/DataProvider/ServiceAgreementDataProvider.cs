using Arista_LPS_WebApp.Entities;
using Arista_LPS_WebApp.Helpers;
using Dapper;
using LPS;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace Arista_LPS_WebApp.DataProvider
{

    public class ServiceAgreementDataProvider : IServiceAgreementDataProvider
    {
        #region  Private variables
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        private readonly ILoggerManager _logger;
        #endregion  Private variables

        #region Constuctor
        public ServiceAgreementDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }
        #endregion Constuctor

        /// <summary>
        /// To Add Or Update Customer based on isupdate.
        /// </summary>
        /// <param name="ClientServiceAgreement"></param>
        /// <param name="isupdate"></param>
        /// <returns></returns>
        public async Task<string> AddOrUpdateServiceAgreement(ClientServiceAgreement serviceAgreement, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    await sqlConnection.ExecuteAsync(
                        "spServiceAgreement_Update",
                        GetServiceAgreementParameters(serviceAgreement, isupdate),
                        commandType: CommandType.StoredProcedure
                    );
                    return "ok";
                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";
                    _logger.LogDebug(" ServiceAgreement Data Provider - AddOrUpdateServiceAgreement - SQL Exception:" + ex.StackTrace);
                    _logger.LogDebug(" ServiceAgreement Data Provider - AddOrUpdateServiceAgreement - SQL Exception:" + ex.Message);
                    DisplayErrorMessage = MessageConstants.BadRequest;
                    return DisplayErrorMessage;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" ServiceAgreement Data Provider - AddOrUpdateServiceAgreement - Exception:: " + ex.StackTrace);
                    _logger.LogDebug(" ServiceAgreement Data Provider - AddOrUpdateServiceAgreement - Exception:: " + ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// To make Dynamic Parameters for ServiceAgreement.
        /// </summary>
        /// <param name="ClientServiceAgreement"></param>
        /// <param name="isUpdate"></param>
        /// <returns></returns>
        private DynamicParameters GetServiceAgreementParameters(ClientServiceAgreement serviceAgreement, bool isUpdate)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@action", isUpdate ? "U" : "I");
            dynamicParameters.Add("@clientId", serviceAgreement.ClientID);
            dynamicParameters.Add("@startDate", serviceAgreement.StartDate);
            dynamicParameters.Add("@endDate", serviceAgreement.EndDate);
            dynamicParameters.Add("@termNoticeDate", serviceAgreement.TermNoticeDate);
            dynamicParameters.Add("@initialTerm", serviceAgreement.InitialTerm);
            dynamicParameters.Add("@renewalTerm", serviceAgreement.RenewalTerm);
            dynamicParameters.Add("@sla", serviceAgreement.SLA);
            dynamicParameters.Add("@minChargeAmount", serviceAgreement.MinChargeAmt);
            dynamicParameters.Add("@sedcMbrNo", serviceAgreement.SEDCMBRNO);
            dynamicParameters.Add("@billType", serviceAgreement.BillType);

            dynamicParameters.Add("@MinCharge", serviceAgreement.MinCharge);
            dynamicParameters.Add("@SystemNumber", serviceAgreement.SystemNumber);
            dynamicParameters.Add("@PGBillRate", serviceAgreement.PGBillRate);
            dynamicParameters.Add("@CSBillRate", serviceAgreement.CSBillRate);
            dynamicParameters.Add("@NonAutoFeeRate", serviceAgreement.NonAutoFeeRate);
            dynamicParameters.Add("@PDFProcessingFee", serviceAgreement.PDFProcessingFee);
            dynamicParameters.Add("@EBPPProcessingRate", serviceAgreement.EBPPProcessingRate);
            
            return dynamicParameters;
        }

        //Deletes the previous files which are unused or orphanes
        private void DeletePreviouslyAddedFiles(List<string> filenames, string customerName, string customerType)
        {
            foreach (var fname in filenames)
            {
                ServiceHelper.DeleteFile(_appSettings.NetworkSharedFolder, customerType, customerName, fname);
            }
        }

        /// <summary>
        /// To Get ServiceAgreement data.
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ClientServiceAgreement>> GetClientServiceAgreement(int clientID = 0)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@clientID", clientID);
                    return await sqlConnection.QueryAsync<ClientServiceAgreement>(
                        "spGetServiceAgreements",
                        dynamicParameters,
                    commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" ServiceAgreement Data Provider - GetClientServiceAgreement - Exception:: " + ex.StackTrace);
                    _logger.LogDebug(" ServiceAgreement Data Provider - GetClientServiceAgreement - Exception:: " + ex.Message);
                    throw;
                }
            }
        }



        //=====Clients Contracts======//

        //To Get All Contract Types.
        public async Task<IEnumerable<ContractType>> GetContractTypes()
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    return await sqlConnection.QueryAsync<ContractType>(
                        "spGetContractType",
                        null,
                        commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" ServiceAgreement Data Provider - GetContractTypes - Exception:: " + ex.StackTrace);
                    _logger.LogDebug(" ServiceAgreement Data Provider - GetContractTypes - Exception:: " + ex.Message);
                    throw;
                }
            }
        }

        public async Task<string> UploadFile(IFormFile FileData, string customerName, string customerType)
        {
            if (FileData.Length > 0)
            {
                string fileName = ContentDispositionHeaderValue.Parse(FileData.ContentDisposition).FileName.Trim('"');
                string newPath = ServiceHelper.GetNetworkPath(_appSettings.NetworkSharedFolder, customerType, customerName, fileName);

                if (!Directory.Exists(Path.GetDirectoryName(newPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                }

                if (!File.Exists(newPath))
                {
                    using (var stream = new FileStream(newPath, FileMode.Create))
                    {
                        await FileData.CopyToAsync(stream);
                    }
                }
                else {
                    return "File already exist.";
                }    
            }

            return "ok";
        }

        private void DeleteFiles(string fileName, string customerName, string customerType)
        {         
            ServiceHelper.DeleteFile(_appSettings.NetworkSharedFolder, customerType, customerName, fileName);
        }

        public byte[] GetFileData(string fileName, string customerName, string clientType)
        {
            byte[] file = null;
            string filePath = ServiceHelper.GetNetworkPath(_appSettings.NetworkSharedFolder, clientType, customerName, fileName);
            if (File.Exists(filePath))
            {
                file = File.ReadAllBytes(filePath);
            }

            return file;
        }


       //======== Contracts ===========//
        //To Get All Contract Types.
        public async Task<IEnumerable<Contracts>> GetContracts(int clientID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@clientID", clientID);
                    dynamicParameters.Add("@param", "CONTRACTS");
                    return await sqlConnection.QueryAsync<Contracts>(
                        "spGetServiceAgreements",
                        dynamicParameters,
                    commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" ServiceAgreement Data Provider - GetContracts - Exception:: " + ex.StackTrace);
                    _logger.LogDebug(" ServiceAgreement Data Provider - GetContracts - Exception:: " + ex.Message);
                    throw;
                }
            }
        }


        public async Task<string> AddOrUpdateContract(Contracts contracts, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@action", isupdate ? "U" : "I");
                    dynamicParameters.Add("@clientContractID", contracts.ClientContractID);
                    dynamicParameters.Add("@clientId", contracts.ClientID);
                    dynamicParameters.Add("@contractDate", contracts.ContractDate);
                    dynamicParameters.Add("@contractType", contracts.ContractType);
                    dynamicParameters.Add("@contract", contracts.Contract);
                    await sqlConnection.ExecuteAsync(
                        "spClientContract_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure
                    );                   

                    DeletePreviouslyAddedFiles(contracts.OldFiles, contracts.ClientName, contracts.CustomerType);

                    return "ok";
                }
                catch (SqlException ex)
                {
                    DeletePreviouslyAddedFiles(contracts.OldFiles, contracts.ClientName, contracts.CustomerType);
                    String DisplayErrorMessage = "";
                    _logger.LogDebug(" ServiceAgreement Data Provider - AddOrUpdateContract - SQL Exception:: " + ex.StackTrace);
                    _logger.LogDebug(" ServiceAgreement Data Provider - AddOrUpdateContract - SQL Exception:" + ex.Message);
                    DisplayErrorMessage = MessageConstants.BadRequest;
                    return DisplayErrorMessage;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" ServiceAgreement Data Provider - AddOrUpdateContract - Exception:: " + ex.StackTrace);
                    _logger.LogDebug(" ServiceAgreement Data Provider - AddOrUpdateContract - Exception:: " + ex.Message);
                    throw;
                }
            }
        }

        //Deletes the previous files which are unused or orphanes
        //private void DeletePreviouslyAddedFiles(List<string> filenames, string customerName, string customerType)
        //{
        //    foreach (var fname in filenames)
        //    {
        //        ServiceHelper.DeleteFile(_appSettings.NetworkSharedFolder, customerType, customerName, fname);
        //    }
        //}


        public async Task<string> DeleteContract(int clientContractID,string customerType,string clientName,string selectedRowFileName)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@clientContractID", clientContractID);
                    await sqlConnection.ExecuteAsync(
                        "spClientContract_Delete",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                
                    DeleteFiles(selectedRowFileName, clientName, customerType);

                    return "ok";                  
                }
                catch (Exception ex)
                {
                    DeleteFiles(selectedRowFileName, clientName, customerType);
                    _logger.LogDebug(" ServiceAgreement Data Provider - DeleteContract - Exception:: " + ex.StackTrace);
                    _logger.LogDebug(" ServiceAgreement Data Provider - DeleteContract - Exception:: " + ex.Message);
                    throw;
                }
            }
        }
        


        //==== Billing Rates ======//


        //To Get Billing Rates.
        public async Task<IEnumerable<BillingRates>> GetBillingRates(int clientID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@clientID", clientID);
                    dynamicParameters.Add("@param", "BILLINGRATES");
                    dynamic result = await sqlConnection.QueryAsync<dynamic>(
                        "spGetServiceAgreements",dynamicParameters, 
                        commandType: CommandType.StoredProcedure);

                    if (result.Count == null)
                        return null;

                    Slapper.AutoMapper.Cache.ClearInstanceCache();
                    Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(BillingRates), new List<string> { "ApplicationID" });
                    Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(CustomerRate), new List<string> { "CustomerRateID" });

                    List<BillingRates> serviceResult = null;
                    serviceResult = (Slapper.AutoMapper.MapDynamic<BillingRates>(result) as IEnumerable<BillingRates>).ToList();

                    // return null if user not found
                    if (serviceResult == null)
                        return null;

                    return serviceResult;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" ServiceAgreement Data Provider - GetBillingRates - Exception:: " + ex.StackTrace);
                    _logger.LogDebug(" ServiceAgreement Data Provider - GetBillingRates - Exception:: " + ex.Message);
                    throw;
                }
            }
        }



        public async Task<string> AddOrUpdateBillingRates(BillingRateInfo billingRateInfo, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@clientID", billingRateInfo.ClientID);
                    dynamicParameters.Add("@service_BillingRates_Xml", BuildBillingRates(billingRateInfo.billingRates));
                    dynamicParameters.Add("@service_CustomerRates_Xml", BuildBillingRateDetails(billingRateInfo.billingRates));
                    await sqlConnection.ExecuteAsync(
                        "spBillinRate_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure
                    );

                   
                    return "ok";
                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";
                    _logger.LogDebug(" ServiceAgreement Data Provider - AddOrUpdateBillingRates - SQL Exception:" + ex.StackTrace);
                    _logger.LogDebug(" ServiceAgreement Data Provider - AddOrUpdateBillingRates - SQL Exception:" + ex.Message);
                    DisplayErrorMessage = MessageConstants.BadRequest;
                    return DisplayErrorMessage;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" ServiceAgreement Data Provider - AddOrUpdateBillingRates - Exception:: " + ex.StackTrace);
                    _logger.LogDebug(" ServiceAgreement Data Provider - AddOrUpdateBillingRates - Exception:: " + ex.Message);
                    throw;
                }
            }   
        }

        /// <summary>
        ///  Build the Agreements XML.
        /// </summary>
        /// <param name="billingRates"></param>
        /// <returns></returns>
        private string BuildBillingRates(List<BillingRates> billingRates)
        {
            StringBuilder billingRateXml = new StringBuilder();
            billingRateXml.Append("<root>");
            billingRates.ForEach(br =>
            {
                billingRateXml.AppendFormat("<row brAppID ='{0}' brStmtName ='{1}' brConAcc ='{2}' brIsActive ='{3}' brPrintOrder ='{4}'/>",
                br.ApplicationID, br.StatementName, br.ConsolidationAcc, br.IsActive, br.PrintOrder);
            });
            billingRateXml.Append("</root>");

            return billingRateXml.ToString();
        }

        /// <summary>
        /// BuildBillingRateDetails
        /// </summary>
        /// <param name="billingRates"></param>
        /// <returns></returns>
        private string BuildBillingRateDetails(List<BillingRates> billingRates)
        {
            StringBuilder customerRateDetailsXml = new StringBuilder();
            customerRateDetailsXml.Append("<root>");
            billingRates.ForEach(br =>
            {
                br.customerRateDetails.ForEach(crd =>
                {
                    if (br.ApplicationID == crd.CustomerID)
                    {
                        customerRateDetailsXml.AppendFormat("<row crdCustId ='{0}' crdAppID  ='{1}' crdRateTypeID ='{2}' crdRate ='{3}' crdCustomerRateID ='{4}' />",
                        crd.CustomerID, crd.AppID, crd.RateTypeID, crd.Rate, crd.CustomerRateID);
                    }
                });
            });
            customerRateDetailsXml.Append("</root>");
            return customerRateDetailsXml.ToString();
        }
    }
}

