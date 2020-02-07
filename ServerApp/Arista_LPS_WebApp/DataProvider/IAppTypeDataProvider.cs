using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IAppTypeDataProvider
    {
        Task<IEnumerable<AppType>> GetAppTypes(bool fromAppType);
        Task<string> AddOrUpdateAppType(AppType apptype,bool isupdate = false);
        Task<string[]> DeleteAppType(string appID);
    }
}
