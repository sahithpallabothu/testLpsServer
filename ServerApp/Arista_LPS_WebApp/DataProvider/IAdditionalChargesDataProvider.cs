using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IAdditionalChargesDataProvider
    {
        Task<IEnumerable<ACJobDetails>> ValidateJobNumber(string jobNumber, string clientCode);
        Task<string> AddOrUpdateAdditionalCharges(AdditionalChargesInfo addlChargesInfo);
        Task<string> CheckDuplicateData(AdditionalCharges addlChargesInfo);
        Task<IEnumerable<GetACDetails>> GetAdditionalChargesBasedOnSearch(string customerCode, string clientName);
        Task<string> UpdateAdditionalChargeByID(PostageAdditionalCharges acDeatils);
        Task<bool> GetAdditionalChargesCount(string customerCode, string clientName);
    }
}
