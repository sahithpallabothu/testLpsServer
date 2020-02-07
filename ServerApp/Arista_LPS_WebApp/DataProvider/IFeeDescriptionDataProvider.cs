using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IFeeDescriptionDataProvider
    {
        Task<IEnumerable<FeeDescription>> GetFeeDescriptions(bool fromFeeDesc);
        Task<string> AddOrUpdateFeeDescription(FeeDescription feeDescription,bool isupdate = false);
        Task<string> DeleteFeeDescription(int RecID);
    }
}
