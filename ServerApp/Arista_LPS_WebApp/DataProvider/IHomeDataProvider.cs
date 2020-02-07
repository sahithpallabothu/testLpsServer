using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IHomeDataProvider
    {
        Task<IEnumerable<Inserts>> GetInserts(SearchDataForHome search);
        Task<IEnumerable<JobDetail>> GetJobs(SearchDataForHome search);
        Task<bool> ValidateJobAndInsertCount(SearchDataForHome search);
    }
}