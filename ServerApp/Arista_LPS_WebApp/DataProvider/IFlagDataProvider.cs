using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IFlagDataProvider
    {
        Task<IEnumerable<Flagtype>> GetFlags(bool fromFlag);
        Task<string> AddOrUpdateFlagType(Flagtype flagtype, bool isupdate = false);
        Task<string> DeleteFlag(int flagTypeID);
    }
}
