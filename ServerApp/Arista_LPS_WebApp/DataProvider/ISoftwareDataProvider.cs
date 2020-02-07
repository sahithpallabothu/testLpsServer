using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arista_LPS_WebApp.Entities;

namespace Arista_LPS_WebApp.DataProvider
{
   public  interface ISoftwareDataProvider
    {
        Task<IEnumerable<Software>> GetAllSoftware();
        Task<string> AddOrUpdateSoftware(Software software, bool isupdate = false);
        Task<string> DeleteSoftware(int id);
    }
}
