using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IChangePrintLocationDataProvider
    {
        Task<IEnumerable<ChangePrintLocation>> GetAllApplicationData(int isPrintWinSalem);
        Task<string> UpdateApplicationPrintLocation(List<UpdateApplicationLocation> applicationLocationData, bool isupdate = false);
    }
}
