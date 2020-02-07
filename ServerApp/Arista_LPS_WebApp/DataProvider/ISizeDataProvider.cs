using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface ISizeDataProvider
    {
        Task<IEnumerable<Size>> GetSize(bool fromSize);
        Task<string> AddOrUpdateSize(Size size, bool isupdate = false);
        Task<string> DeleteSize(int sizeID);
    }
}
