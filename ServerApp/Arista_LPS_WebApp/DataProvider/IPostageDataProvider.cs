using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IPostageDataProvider
    {
        Task<IEnumerable<JobDetail>> getPostageByJob(string jobNumber, string runDate, string recordId);
        Task<string> updateJobDetail(JobDetail jobDetail);
    }
}
