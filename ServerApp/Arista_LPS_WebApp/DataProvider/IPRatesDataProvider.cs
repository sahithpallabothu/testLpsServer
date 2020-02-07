using Arista_LPS_WebApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IPRateDataProvider
    {
        Task<IEnumerable<PRates>> GetPRates();
        Task<string> AddOrUpdatePRates(PRates pRates, bool isupdate = false);
        Task<string> DeletePRate(int pRateID);
    }
}
