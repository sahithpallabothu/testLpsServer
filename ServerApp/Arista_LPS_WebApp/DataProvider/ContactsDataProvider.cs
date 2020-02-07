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
    public class ContactsDataProvider : IContactsDataProvider
    {
        private readonly string connectionString;
        private readonly AppSettings _appSettings;
        private ILoggerManager _logger;
        public ContactsDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To get the Contacts. 
        /// </summary>
        /// <param name="contactId">contactId</param>
        /// <returns>contacts</returns>
        public async Task<IEnumerable<Contacts>> GetContacts(int contactId = 0)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@clientID", contactId);
                    return await sqlConnection.QueryAsync<Contacts>(
                         "spGetContacts",
                         dynamicParameters,
                         commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" Contacts Data Provider - GetContacts - Exception:: " + ex.StackTrace);
                    _logger.LogDebug(" Contacts Data Provider - GetContacts - Exception:: " + ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Get the Applications.
        /// </summary>
        /// <param name="clientID">clientID</param>
        /// <param name="contactID">contactID</param>
        /// <returns></returns>
        public async Task<IEnumerable<ContactApplication>> GetApplications(int clientID, int contactID = 0)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    IEnumerable<AppNotifications> notificationlist = null;
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@clientID", clientID);
                    var result = await sqlConnection.QueryAsync<ContactApplication>(
                           "spGetApplicationsForContacts",
                           dynamicParameters,
                        commandType: CommandType.StoredProcedure);

                    if (contactID > 0)
                    {
                        notificationlist = await GetNotificationsResult(contactID);

                        foreach (var item in result)  // Applications
                        {
                            foreach (var notif in notificationlist) // Notifications
                            {
                                if (item.ApplicationID == notif.ApplicationID)
                                {
                                    item.NotifyPDF = notif.NotifyPDF;
                                    item.NotifyFileReceived = notif.NotifyFileReceived;
                                    item.NotifyJobComplete = notif.NotifyJobComplete;
                                    item.EmailRMT = notif.EmailRMT;
                                    item.EmailCode1 = notif.EmailCode1;

                                    if (item.NotifyPDF || item.NotifyFileReceived || item.NotifyJobComplete || item.EmailRMT || item.EmailCode1)
                                    {
                                        item.AlarmExist = true;
                                    }
                                }
                            }
                        }
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" Contacts Data Provider - GetApplications - Exception:: " + ex.StackTrace);
                    _logger.LogDebug(" Contacts Data Provider - GetApplications - Exception:: " + ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// To get the Notifications.
        /// </summary>
        /// <param name="contactId">contactId</param>
        /// <returns>notifications</returns>
        private async Task<IEnumerable<AppNotifications>> GetNotificationsResult(int contactId = 0)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);
                    dynamicParameters.Add("@contactID", contactId);

                    var data = await sqlConnection.QueryAsync<AppNotifications>(
                        "spGetNotificationsByContactID",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return data;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" Contacts Data Provider - GetNotificationsResult - Exception:: " + ex.StackTrace);
                    _logger.LogDebug(" Contacts Data Provider - GetNotificationsResult - Exception:: " + ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Add or Update Contacts.
        /// </summary>
        /// <param name="contact">contact</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>response</returns>
        public async Task<string> AddOrUpdateContact(Contacts contact, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@action", isupdate ? "U" : "I");
                    dynamicParameters.Add("@currentContactId", contact.ContactID);
                    dynamicParameters.Add("@clientId", contact.ClientID);
                    dynamicParameters.Add("@contactFirstName", contact.ContactFirstName);
                    dynamicParameters.Add("@contactLastName", contact.ContactLastName);
                    dynamicParameters.Add("@contactTitle", contact.ContactTitle);
                    dynamicParameters.Add("@contactEmail", contact.ContactEmail);
                    dynamicParameters.Add("@contactPhone", contact.ContactPhone);
                    dynamicParameters.Add("@contactExtension", contact.ContactExtension);
                    dynamicParameters.Add("@contactCell", contact.ContactCell);
                    dynamicParameters.Add("@contactHome", contact.ContactHome);
                    dynamicParameters.Add("@billingContact", contact.BillingContact);
                    dynamicParameters.Add("@billingAlternateContact", contact.BillingAlternateContact);
                    dynamicParameters.Add("@oobContact", contact.OOBContact);
                    dynamicParameters.Add("@delqContact", contact.DELQContact);
                    dynamicParameters.Add("@cccontact", contact.CCContact);
                    dynamicParameters.Add("@ebppContact", contact.EBPPContact);
                    dynamicParameters.Add("@insertContact", contact.InsertContact);
                    dynamicParameters.Add("@emailConfirmations", contact.EmailConfirmations);
                    dynamicParameters.Add("@insertAlternateContact", contact.InsertAlternateContact);
                    dynamicParameters.Add("@comment", contact.Comment);
                    dynamicParameters.Add("@notification_Xml", GetNotifications(contact.ApplicationList, contact.ContactEmail));
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);
                    await sqlConnection.ExecuteAsync(
                        "spContact_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    //Getting result from out parameter.    
                    var response = dynamicParameters.Get<string>("@result");
                    return response;
                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";
                    _logger.LogDebug("Contacts Data Provider - AddOrUpdateContact - SQL Exception:" + ex.StackTrace);
                    _logger.LogDebug("Contacts Data Provider - AddOrUpdateContact - SQL Exception:" + ex.Message);
                    DisplayErrorMessage = MessageConstants.BadRequest;

                    return DisplayErrorMessage;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(" Contacts Data Provider - AddOrUpdateContact - Exception:: " + ex.StackTrace);
                    _logger.LogDebug(" Contacts Data Provider - AddOrUpdateContact - Exception:: " + ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Get the Notifications.
        /// </summary>
        /// <param name="notifications">notifications</param>
        /// <param name="emailID">emailID</param>
        /// <returns>xmlstring</returns>
        private string GetNotifications(List<ContactApplication> notifications, string emailID)
        {
            StringBuilder childXml = new StringBuilder();
            try
            {
                childXml.Append("<root>");
                notifications.ForEach(n =>
                {
                    childXml.AppendFormat
                     ("<row ApplicationId='{0}' NotifyPDF='{1}' NotifyJobComplete='{2}' NotifyFileReceived='{3}' EmailRMT='{4}' EmailCode1='{5}' EmailAddr='{6}'/>",
                        n.ApplicationID, n.NotifyPDF, n.NotifyJobComplete, n.NotifyFileReceived, n.EmailRMT, n.EmailCode1, emailID);
                });
                childXml.Append("</root>");
            }
            catch (Exception ex)
            {
                _logger.LogDebug(" Contacts Data Provider - GetNotifications - Exception:: " + ex.StackTrace);
                _logger.LogDebug(" Contacts Data Provider - GetNotifications - Exception:: " + ex.Message);
                throw;
            }
            return childXml.ToString();
        }

        /// <summary>
        /// To delete the Contact.
        /// </summary>
        /// <param name="contactid">contactid</param>
        /// <returns>ok</returns>
        public async Task<string> DeleteContact(int contactid, int clientID, int lastDeleteFlag)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);
                    dynamicParameters.Add("@contactID", contactid);
                    dynamicParameters.Add("@clientID", clientID);
                    dynamicParameters.Add("@lastDeleteFlag", lastDeleteFlag);

                    await sqlConnection.ExecuteAsync(
                        "spContact_Delete",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    //Getting result from out parameter.    
                    var response = dynamicParameters.Get<string>("@result");
                    if (response == MessageConstants.RECORDINUSE)
                    {
                        return response;
                    }
                    else if (response == "LAST_CONTACT")
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
                    _logger.LogDebug(" Contacts Data Provider - DeleteContact - Exception:: " + ex.StackTrace);
                    _logger.LogDebug(" Contacts Data Provider - DeleteContact - Exception:: " + ex.Message);
                    throw;
                }
            }
        }
    }
}
