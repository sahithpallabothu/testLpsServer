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
    public class ApplicationDataProvider : IApplicationDataProvider
    {
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        private ILoggerManager _logger;

        private string TrackingSTID = "041";
        private string DefaultSTID = "036";
        private string Laser = "10.30.10.100";

        public ApplicationDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To get the Applications
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>application</returns>
        public async Task<IEnumerable<Application>> GetApplications(int id = 0)
        {
            IDictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(0, "No Set Frequency");
            dict.Add(1, "Daily");
            dict.Add(2, "Weekly");
            dict.Add(3, "Monthly");
            dict.Add(4, "Bi-Monthly");
            dict.Add(7, "Quarterly");
            dict.Add(8, "Annually");
            dict.Add(9, "Daily");
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@recordID", id);
                    var result = await sqlConnection.QueryAsync<Application>(
                        "spGetAllApplications",
                            dynamicParameters,
                    commandType: CommandType.StoredProcedure);
                    foreach (var result1 in result)
                    {
                        if (result1.RunFrequency != null)
                        {
                            result1.RunFrequencyName = dict[Convert.ToInt32(result1.RunFrequency)];
                        }
                    }
                    return result;
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// To get all the Applications on the Seacrh criteria.
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns>applications</returns>
        public async Task<IEnumerable<Application>> GetApplicationBasedOnSearch(Application obj)
        {
            IDictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(0, "No Set Frequency");
            dict.Add(1, "Daily");
            dict.Add(2, "Weekly");
            dict.Add(3, "Monthly");
            dict.Add(4, "Bi-Monthly");
            dict.Add(7, "Quarterly");
            dict.Add(8, "Annually");
            dict.Add(9, "Daily");

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@searchOption", obj.SearchOption);
                    dynamicParameters.Add("@searchText", obj.SearchText.Replace("*", "%"));
                    var result = await sqlConnection.QueryAsync<Application>(
                        "spGetViewCustomers",
                            dynamicParameters,
                    commandType: CommandType.StoredProcedure);
                    foreach (var result1 in result)
                    {
                        if (result1.RunFrequency != null) {
                            result1.RunFrequencyName = dict[Convert.ToInt32(result1.RunFrequency)];
                        }
                        
                    }
                    return result;
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// To Get the Locations.
        /// </summary>
        /// <returns>locations</returns>
        public async Task<IEnumerable<PrintLocation>> GetLocations()
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    return await sqlConnection.QueryAsync<PrintLocation>(
                        "spGetLocation",
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
        /// To Add or Update the Application. 
        /// </summary>
        /// <param name="application">application</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>OK</returns>
        public async Task<string> AddOrUpdateApplication(Application application, bool isupdate = false)
        {
            int LatestAppId;

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters = GetApplicationParameters(application, isupdate);
                    await sqlConnection.OpenAsync();
                    LatestAppId = await sqlConnection.ExecuteAsync(
                        "spApplication_Update",
                         dynamicParameters,
                        commandType: CommandType.StoredProcedure
                    );
                    
                    var response = dynamicParameters.Get<string>("@result");
                    if (isupdate)
                    {
                        return "Ok";
                    }
                    else
                    {
                        return response;
                    }

                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";
                    _logger.LogDebug(" ApplicationDataProvider - AddOrUpdateApplication - SQL Exception:" + ex.Message);
                    _logger.LogDebug(" ApplicationDataProvider - AddOrUpdateApplication - SQL Exception:" + ex.StackTrace);
                    DisplayErrorMessage = MessageConstants.BadRequest;
                    return DisplayErrorMessage;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" ApplicationDataProvider - AddOrUpdateApplication - Exception:" + ex.Message);
                    _logger.LogDebug(" ApplicationDataProvider - AddOrUpdateApplication - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// GetApplicationParameters
        /// </summary>
        /// <param name="application">application</param>
        /// <param name="isUpdate">isUpdate</param>
        /// <returns>DynamicParameters</returns>
        private DynamicParameters GetApplicationParameters(Application application, bool isUpdate)
        {
            string custNumber = string.Empty;
            string custID = Convert.ToString(application.CustomerID);
            if (_appSettings.DefaultSTID != string.Empty)
            {
                DefaultSTID = _appSettings.DefaultSTID;
            }
            if (_appSettings.TrackingSTID != string.Empty)
            {
                TrackingSTID = _appSettings.TrackingSTID;
            }
            if (_appSettings.Laser != string.Empty)
            {
                Laser = _appSettings.Laser;
            }
            string stid = application.ImbTracing ? TrackingSTID : DefaultSTID;
            stid += application.Endorsement;
            string color = application.Color == "Black" ? "X" : application.Color.Substring(0, 1);
            for (; custID.Length < 3;)
            {
                custID = "0" + custID;
            }
            custNumber = application.CustomerState + application.CustomerFlag + custID + application.AppType + color;
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@action", isUpdate ? "U" : "I");
            dynamicParameters.Add("@ClientID", application.ClientID);
            dynamicParameters.Add("@RecordID", application.ApplicationID);
            dynamicParameters.Add("@CustomerName", application.ApplicationName);
            dynamicParameters.Add("@CustomerCode", application.ApplicationCode);
            dynamicParameters.Add("@CustomerNumber", custNumber);
            dynamicParameters.Add("@CustomerState", application.CustomerState);
            dynamicParameters.Add("@CustomerFlag", application.CustomerFlag);
            if (application.CustomerID != -1)
            {
                dynamicParameters.Add("@CustomerID", application.CustomerID);
            }
            dynamicParameters.Add("@CustomerAppType", application.AppType);
            dynamicParameters.Add("@CustomerPrintColor", application.Color);
            dynamicParameters.Add("@PrintWinSalem", application.PrintLocation);
            dynamicParameters.Add("@FormCode", application.Paper);
            dynamicParameters.Add("@WindowEnvCode", application.OutsideEnvelope);
            dynamicParameters.Add("@ReturnEnvCode", application.ReturnEnvelope);
            dynamicParameters.Add("@PDF", application.Pdf);
            dynamicParameters.Add("@ABPP", application.Ebpp);
            /* if (application.RunFrequency != -1)
             {
                 dynamicParameters.Add("@RunFrequency", application.RunFrequency);
             }*/
            dynamicParameters.Add("@RunFrequency", application.RunFrequency);
            dynamicParameters.Add("@STID", stid);
            dynamicParameters.Add("@Active", application.Active);
            dynamicParameters.Add("@ProcessingHold", application.ProcessingHold);
            dynamicParameters.Add("@HeldReason", application.HoldReason);
            dynamicParameters.Add("@DetailBill", application.DetailBill);
            dynamicParameters.Add("@InvoiceBill", application.InvoiceBill);
            dynamicParameters.Add("@SummaryBill ", application.SummaryBill);
            dynamicParameters.Add("@AncillaryBill", application.AncillaryBill);
            dynamicParameters.Add("@MultiMeterBill", application.MultimeterBill);
            dynamicParameters.Add("@MunicipalBill", application.MunicipalBill);
            dynamicParameters.Add("@TVA", application.Tva);
            dynamicParameters.Add("@TDHUD", application.Tdhud);
            dynamicParameters.Add("@Itemized", application.ItemizedBill);
            dynamicParameters.Add("@Unbundled", application.UnBundled);
            dynamicParameters.Add("@MDM", application.Mdm);
            dynamicParameters.Add("@Delinquent", application.Delinquent);
            dynamicParameters.Add("@ThirdParty", application.ThirdParty);
            dynamicParameters.Add("@Check", application.Check);
            dynamicParameters.Add("@Software", application.Software);
            dynamicParameters.Add("@Overlay", application.Overlay);
            dynamicParameters.Add("@Backer", application.Backer);
            dynamicParameters.Add("@IsAutoRun", application.AutoRun);
            dynamicParameters.Add("@ClientActive", application.ClientActive);
            /*if (application.Perf != -1)
            {
                dynamicParameters.Add("@PerfPattern", application.Perf);
            }*/
            dynamicParameters.Add("@PerfPattern", application.Perf);
            if (application.Size != -1)
            {
                dynamicParameters.Add("@PrintSizeId", application.Size);
            }
            if (application.PrintSide != -1)
            {
                dynamicParameters.Add("@PrintSideId", application.PrintSide);
            }
            dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);
            return dynamicParameters;
        }

        /// <summary>
        /// To get the Configuration Values.
        /// </summary>
        /// <returns></returns>
        public Configuration GetConfiguration()
        {
            Configuration config = new Configuration();
            try
            {
                if (_appSettings.DefaultSTID != string.Empty)
                {
                    config.DefaultSTID = _appSettings.DefaultSTID;
                }
                if (_appSettings.TrackingSTID != string.Empty)
                {
                    config.TrackingSTID = _appSettings.TrackingSTID;
                }
                if (_appSettings.Laser != string.Empty)
                {
                    config.Laser = _appSettings.Laser;
                }
                return config;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// To Check AdminExists.
        /// </summary>
         /// <param name="clientID">clientID</param>
        /// <returns></returns>
        public async Task<string> CheckActiveApplicationExist(int clientID, int recordID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@clientID", clientID);
                    dynamicParameters.Add("@recordID", recordID);
                    dynamicParameters.Add("@response", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);

                    await sqlConnection.ExecuteAsync(
                        "GetActiveApplicationCount",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);

                    var response = dynamicParameters.Get<string>("@response");
                    return response;
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// To Delete the Application
        /// </summary>
        /// <param name="appID">appID</param>
        /// <returns>ok</returns>
        public async Task<string> DeleteApplication(int appID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();

                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@applicationID", appID);
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);

                    await sqlConnection.ExecuteAsync(
                        "spCustomer_Delete",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    var response = dynamicParameters.Get<string>("@result");
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" ApplicationDataProvider - DeleteApplication - Exception:" + ex.Message);
                    _logger.LogDebug(" ApplicationDataProvider - DeleteApplication - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }
    }
}

