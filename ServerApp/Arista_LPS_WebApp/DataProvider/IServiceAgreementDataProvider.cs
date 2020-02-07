using Arista_LPS_WebApp.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IServiceAgreementDataProvider
    {
        //Service Agreement
        Task<IEnumerable<ClientServiceAgreement>> GetClientServiceAgreement(int clientID = 0);
        Task<string> AddOrUpdateServiceAgreement(ClientServiceAgreement ServiceAgreement, bool isupdate = false);

        //Contracts
        Task<IEnumerable<ContractType>> GetContractTypes();
        Task<IEnumerable<Contracts>> GetContracts(int clientID = 0);
        Task<string> AddOrUpdateContract(Contracts contracts, bool isupdate = false);
        Task<string> UploadFile(IFormFile FileData, string CustomerName, string CustomerType);

        byte[] GetFileData(string fileName, string customerName, string clientType);
        Task<string> DeleteContract(int clientContractID, string customerType, string clientName, string selectedRowFileName);

        //Billing Rates
        Task<IEnumerable<BillingRates>> GetBillingRates(int clientID = 0);
        Task<string> AddOrUpdateBillingRates(BillingRateInfo billingRateInfo, bool isupdate = false);
    }
}
