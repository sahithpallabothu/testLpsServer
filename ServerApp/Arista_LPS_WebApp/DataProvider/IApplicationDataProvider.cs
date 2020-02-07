using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IApplicationDataProvider
    {
        Task<IEnumerable<Application>> GetApplications(int id = 0);
        Task<IEnumerable<Application>> GetApplicationBasedOnSearch(Application obj);
        Task<IEnumerable<PrintLocation>> GetLocations();
        Task<string> AddOrUpdateApplication(Application application, bool isupdate = false);
        Configuration GetConfiguration();
        Task<string> CheckActiveApplicationExist(int clientID, int recordID);
        Task<string> DeleteApplication(int appID);
    }
}
