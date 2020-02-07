using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IPerfPatternDataProvider
    {
        Task<IEnumerable<PerfPatterns>> GetPerfPatterns(bool fromPerfPattern);
        Task<string> AddOrUpdatePerfPatterns(PerfPatterns perfPatterns, bool isupdate = false);
        Task<string> DeletePerfPatterns(int perfPattern);
    }
}
