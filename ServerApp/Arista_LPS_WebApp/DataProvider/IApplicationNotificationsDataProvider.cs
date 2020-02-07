using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IApplicationNotificationsDataProvider
    {
        Task<IEnumerable<ApplicationContacts>> GetApplicationContacts(int clientID, int applicationID);
        Task<string> AddOrUpdateNotifications(ApplicationNotificationsData appNotifications,bool isupdate = false);
    }
}
