using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IRoleDataProvider
    {
        Task<IEnumerable<Roles>> GetAllRoles(bool fromRoles);
        Task<IEnumerable<RolePrivileges>> GetRolePrivileges(int roleID = 0);
        Task<string> AddOrUpdateRole(Roles role, bool isupdate = false);
        Task<string[]> DeleteRole(int roleId);
        Task<IEnumerable<RolePrivileges>> GetPrivilegedScreensByUserID(int userID);
    }
}
