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
    public class CustomerDataProvider : ICustomerDataProvider
    {
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        private ILoggerManager _logger;
        public CustomerDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To Add or Update Customer.
        /// </summary>
        /// <param name="customer">customer</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>ok</returns>
        public async Task<string> AddOrUpdateCustomer(Customer customer, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    Tuple<string, string> res = await ValidateClient(customer);
                    if (res.Item1 != null && (res.Item1.ToString() == MessageConstants.CLIENTEXISTS))
                    {
                        if (res.Item2 != null && (res.Item2.ToString() == MessageConstants.MAILERIDORCRIDEXISTS))
                        {
                            await sqlConnection.OpenAsync();
                            await sqlConnection.ExecuteAsync(
                                "spCustomer_Update",
                                GetCustomerParameters(customer, isupdate),
                                commandType: CommandType.StoredProcedure
                            );
                            return "ok";
                        }
                        else
                        {
                            if (res.Item2.ToString() != MessageConstants.MAILERIDORCRIDEXISTS && customer.IsAllowSave == true)
                            {
                                await sqlConnection.OpenAsync();
                                await sqlConnection.ExecuteAsync(
                                    "spCustomer_Update",
                                    GetCustomerParameters(customer, isupdate),
                                    commandType: CommandType.StoredProcedure
                                );
                                return "ok";
                            }
                        }
                        return res.ToString();
                    }
                    else
                    {
                        return res.ToString();
                    }
                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";
                    _logger.LogDebug(" Customer Data Provider - AddOrUpdateCustomer - SQL Exception:" + ex.Message);
                    DisplayErrorMessage = MessageConstants.BadRequest;
                    return DisplayErrorMessage;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" Customer Data Provider - AddOrUpdateCustomer - Exception:: " + ex.StackTrace);
                    _logger.LogDebug(" Customer Data Provider - AddOrUpdateCustomer - Exception:: " + ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// ValidateClient
        /// </summary>
        /// <param name="customer">customer</param>
        /// <returns></returns>
        private async Task<Tuple<string, string>> ValidateClient(Customer customer)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@param", "VALIDATECLIENT");
                    dynamicParameters.Add("@customerCode", customer.CustomerCode);
                    dynamicParameters.Add("@customerName", customer.CustomerName);
                    dynamicParameters.Add("@mailerId", customer.MailerId);
                    dynamicParameters.Add("@crid", customer.CRID);
                    dynamicParameters.Add("@clientID", customer.CustomerId);
                    dynamicParameters.Add("@isCustomerExist", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    dynamicParameters.Add("@isMailerIdOrCridExists", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    await sqlConnection.OpenAsync();
                    await sqlConnection.ExecuteAsync(
                        "spGetClients",
                         dynamicParameters,
                        commandType: CommandType.StoredProcedure
                    );
                    var custresponse = dynamicParameters.Get<String>("@isCustomerExist");
                    var mailerresponse = dynamicParameters.Get<String>("@isMailerIdOrCridExists");
                    Tuple<string, string> response = Tuple.Create(custresponse, mailerresponse);
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" Customer Data Provider - ValidateClient - Exception:" + ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Get the Customer Parameters.
        /// </summary>
        /// <param name="customer">customer</param>
        /// <param name="isUpdate">isUpdate</param>
        /// <returns>DynamicParameters</returns>
        private DynamicParameters GetCustomerParameters(Customer customer, bool isUpdate)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@action", isUpdate ? "U" : "I");
            dynamicParameters.Add("@clientId", customer.CustomerId);
            dynamicParameters.Add("@clientName", customer.CustomerName);
            dynamicParameters.Add("@clientCode", customer.CustomerCode);
            dynamicParameters.Add("@SEDCCustomer", customer.SEDC);
            dynamicParameters.Add("@telephone", customer.Telephone);
            dynamicParameters.Add("@fax", customer.Fax);
            dynamicParameters.Add("@IVRPhoneNumber", customer.IVRPhoneNumber);
            dynamicParameters.Add("@active", customer.Active);
            dynamicParameters.Add("@comment", customer.Comment);
            dynamicParameters.Add("@mailerID", customer.MailerId);
            dynamicParameters.Add("@CRID", customer.CRID);
            dynamicParameters.Add("@IVR", customer.IVR);

            // Physical Address
            dynamicParameters.Add("@physicalAddress", customer.PhysicalAddress1);
            dynamicParameters.Add("@physicalAddress2", customer.PhysicalAddress2);
            dynamicParameters.Add("@physicalCity", customer.PhysicalCity);
            dynamicParameters.Add("@physicalState", customer.PhysicalState);
            dynamicParameters.Add("@physicalZip", customer.PhysicalZip);

            // Mailing Address
            dynamicParameters.Add("@mailingAddress", customer.MailingAddress1);
            dynamicParameters.Add("@mailingAddress2", customer.MailingAddress2);
            dynamicParameters.Add("@mailingCity", customer.MailingCity);
            dynamicParameters.Add("@mailingState", customer.MailingState);
            dynamicParameters.Add("@mailingZip", customer.MailingZip);

            // Held Address
            dynamicParameters.Add("@held1Type", customer.CheckHeld1TypeId);
            dynamicParameters.Add("@held2Type", customer.Held2TypeId);
            dynamicParameters.Add("@CheckHeldaddress", customer.CheckHeldAddress1);
            dynamicParameters.Add("@CheckHeldaddress", customer.CheckHeldAddress1);
            dynamicParameters.Add("@CheckHeldaddress2", customer.CheckHeldAddress2);
            dynamicParameters.Add("@CheckHeldcity", customer.CheckHeldCity);
            dynamicParameters.Add("@CheckHeldstate", customer.CheckHeldState);
            dynamicParameters.Add("@CheckHeldzip", customer.CheckHeldZip);
            dynamicParameters.Add("@CheckHeldcontactName", customer.CheckHeldContact);
            dynamicParameters.Add("@CheckHeldshipmentMethod", customer.CheckHeldShipmentMethod);

            dynamicParameters.Add("@Heldaddress", customer.HeldAddress1);
            dynamicParameters.Add("@Heldaddress2", customer.HeldAddress2);
            dynamicParameters.Add("@Heldcity", customer.HeldCity);
            dynamicParameters.Add("@Heldstate", customer.HeldState);
            dynamicParameters.Add("@Heldzip", customer.HeldZip);
            dynamicParameters.Add("@HeldcontactName", customer.HeldContact);
            dynamicParameters.Add("@HeldshipmentMethod", customer.HeldShipmentMethod);

            dynamicParameters.Add("@CustomerName", customer.applicationName);
            dynamicParameters.Add("@CustomerCode", customer.applicationCode);
            dynamicParameters.Add("@ApplicationActive", customer.ApplicationActive);

            return dynamicParameters;
        }

        /// <summary>
        /// To Get the Cutomers.
        /// </summary>
        /// <param name="customerId">customerId</param>
        /// <param name="isCustomer">isCustomer</param>
        /// <returns>clients</returns>
        public async Task<IEnumerable<Customer>> GetCustomers(int customerId = 0, bool isCustomer = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@clientID", customerId);
                    if (isCustomer)
                    {
                        dynamicParameters.Add("@param", "CUSTOMER");
                    }
                    else
                    {
                        dynamicParameters.Add("@param", "OTHER");
                    }
                    return await sqlConnection.QueryAsync<Customer>(
                        "spGetClients",
                        dynamicParameters,
                    commandType: CommandType.StoredProcedure);
                }
                catch(Exception ex)
                {
                    _logger.LogDebug(" Customer Data Provider - GetCustomers - Exception:" + ex.StackTrace);
                    _logger.LogDebug(" Customer Data Provider - GetCustomers - Exception:" + ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Get the Customers.
        /// </summary>
        /// <param name="cust">cust</param>
        /// <returns>customers</returns>
        public async Task<IEnumerable<Customer>> GetViewCustomer(Customer cust)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@searchOption", cust.selectedValue);
                    dynamicParameters.Add("@searchText", cust.searchText.Replace("*", "%"));

                    return await sqlConnection.QueryAsync<Customer>(
                        "spGetViewClients",
                        dynamicParameters,
                    commandType: CommandType.StoredProcedure);
                }
                catch(Exception ex)
                {
                    _logger.LogDebug(" Customer Data Provider - GetViewCustomer - Exception:" + ex.StackTrace);
                    _logger.LogDebug(" Customer Data Provider - GetViewCustomer - Exception:" + ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// To get the HeldType.
        /// </summary>
        /// <returns>Heldtype</returns>
        public async Task<IEnumerable<HeldType>> GetHeldType()
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    return await sqlConnection.QueryAsync<HeldType>(
                     "spGetHeldType",
                      null,
                 commandType: CommandType.StoredProcedure);
                }
                catch(Exception ex)
                {
                    _logger.LogDebug(" Customer Data Provider - GetHeldType - Exception:" + ex.StackTrace);
                    _logger.LogDebug(" Customer Data Provider - GetHeldType - Exception:" + ex.Message);
                    throw;
                }
            }
        }
    }
}
