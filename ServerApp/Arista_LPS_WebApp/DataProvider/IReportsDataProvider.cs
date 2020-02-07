using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IReportsDataProvider
    {
        Task<IEnumerable<Reports>> GetSearchDetails(Reports obj);
    }
}
