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
    public class ApplicationNotificationsDataProvider : IApplicationNotificationsDataProvider
    {
        private readonly string connectionString;
        private readonly AppSettings _appSettings;
        private ILoggerManager _logger;
        public ApplicationNotificationsDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }

        /// <summary>
        /// To get all the application Contacts based on ClientID and ApplicaionID.
        /// </summary>
        /// <param name="clientID">clientID</param>
        /// <param name="ApplicationID">ApplicationID</param>
        /// <returns>result</returns>
        public async Task<IEnumerable<ApplicationContacts>> GetApplicationContacts(int clientID, int ApplicationID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    IEnumerable<ApplicationNotifications> notificationlist = null;
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@clientID", clientID);
                    var result = await sqlConnection.QueryAsync<ApplicationContacts>(
                         "spGetContacts",
                         dynamicParameters,
                         commandType: CommandType.StoredProcedure);

                    notificationlist = await GetNotificationsResult(ApplicationID);
                    foreach (var contact in result)
                    {
                        contact.ApplicationID = ApplicationID;
                        foreach (var notif in notificationlist) // Notifications
                        {
                            if (contact.ContactID == notif.ContactID)
                            {
                                contact.NotifyPDF = notif.NotifyPDF;
                                contact.NotifyFileReceived = notif.NotifyFileReceived;
                                contact.NotifyJobComplete = notif.NotifyJobComplete;
                                contact.EmailCode1 = notif.EmailCode1;
                                contact.EmailRMT = notif.EmailRMT;
                            }
                        }
                        if (contact.NotifyPDF || contact.NotifyFileReceived || contact.NotifyJobComplete || contact.EmailCode1 || contact.EmailRMT)
                        {
                            contact.AlarmExist = true;
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
        /// To get the Notification Result.
        /// </summary>
        /// <param name="ApplicationID">ApplicationID</param>
        /// <returns>data</returns>
        private async Task<IEnumerable<ApplicationNotifications>> GetNotificationsResult(int ApplicationID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@applicationID", ApplicationID);
                    var data = await sqlConnection.QueryAsync<ApplicationNotifications>(
                        "[spGetNotificationsByApplID]",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);

                    return data;
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// To Add or Update Notifications.
        /// </summary>
        /// <param name="appNotifications">appNotifications</param>
        /// <param name="isupdate">isupdate</param>
        /// <returns>ok</returns>
        public async Task<string> AddOrUpdateNotifications(ApplicationNotificationsData appNotifications, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@applicationId", appNotifications.ApplicationID);
                    dynamicParameters.Add("@notification_Xml", await GetNotifications(appNotifications.NotificationList));
                    await sqlConnection.ExecuteAsync(
                        "spApplNotifications_Update",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);
                    return "ok";
                }
                catch (SqlException ex)
                {
                    String DisplayErrorMessage = "";
                    _logger.LogDebug("Application notifications Data Provider - notifications - SQL Exception:" + ex.Message);
                    _logger.LogDebug("Application notifications Data Provider - notifications - SQL Exception:" + ex.StackTrace);
                    DisplayErrorMessage = MessageConstants.BadRequest;

                    return DisplayErrorMessage;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug("Application notifications Data Provider - notifications - Exception:" + ex.Message);
                    _logger.LogDebug("Application notifications Data Provider - notifications - Exception:" + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// To Get the Notification xml.
        /// </summary>
        /// <param name="notifications">notifications</param>
        /// <returns>xmlstring</returns>
        private async Task<string> GetNotifications(List<ApplicationNotifications> notifications)
        {
            StringBuilder childXml = new StringBuilder();

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                   await sqlConnection.OpenAsync();

                   childXml.Append("<root>");
                   notifications.ForEach(n =>
                   {
                       childXml.AppendFormat
                       ("<row  ApplicationId='{0}' NotifyPDF='{1}' NotifyJobComplete='{2}' NotifyFileReceived='{3}' EmailRMT='{4}' EmailCode1='{5}' EmailAddr='{6}' contactId='{7}'/>",
                       n.ApplicationID, n.NotifyPDF, n.NotifyJobComplete, n.NotifyFileReceived, n.EmailRMT, n.EmailCode1, n.EmailAddr, n.ContactID);
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
    }
}