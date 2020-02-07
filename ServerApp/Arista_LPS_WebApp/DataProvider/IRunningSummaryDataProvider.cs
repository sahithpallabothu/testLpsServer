using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IRunningSummaryDataProvider
    {
        Task<IEnumerable<RunningSummary>> GetAllJobDetails(string runDate, int isPrintWinSalem, int isAutoRun, int postFlag, string trip);
        Task<string> UpdateJobPostFlag(List<UpdateJobPostFlag> jobData,bool isupdate = false);   
    }
}
