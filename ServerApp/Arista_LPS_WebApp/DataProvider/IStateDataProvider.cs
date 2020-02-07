using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IStateDataProvider
    {
        Task<IEnumerable<States>> GetStates(bool fromStates);
        Task<string> AddOrUpdateState(States state, bool isupdate = false);
        Task<string> DeleteState(int stateID);
    }
}
