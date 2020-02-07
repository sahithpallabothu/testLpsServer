using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IRateDescriptionDataProvider
    {
        Task<IEnumerable<RateDescription>> GetRateDescriptions(bool fromRateType);
        Task<string> AddOrUpdateRateDescription(RateDescription rateDescription, bool isupdate = false);
        Task<string> DeleteRateDescription(int rateTypeID);
    }
}
