using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IInsertDataProvider
    {
        Task<IEnumerable<Inserts>> GetAllInsertsOrByID(int id = 0, string custName = null, string custCode = null, string startDate = null, string endDate = null, string screenCPLName=null,bool active=true);
        Task<IEnumerable<InsertType>> GetAllInsertTypes();
        Task<IEnumerable<Application>> GetApplicationsForInserts();
        Task<string> AddOrUpdateInsert(Inserts[] insert, bool isupdate = false);
        Task<int> CheckInsertType(string applicationCode, int insertType, int insertNumber, int active, string startDate, string endDate);
        Task<string> DeleteInsertByID(int RecID);
    }
}
