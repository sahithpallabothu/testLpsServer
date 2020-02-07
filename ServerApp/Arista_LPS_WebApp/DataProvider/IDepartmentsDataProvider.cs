using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IDepartmentsDataProvider
    {
        Task<IEnumerable<Departments>> GetDepartments(bool fromDepartment);
        Task<string> AddOrUpdateDepartments(Departments department,bool isupdate = false);
        Task<string[]> DeleteDepartment(int recID);
    }
}
