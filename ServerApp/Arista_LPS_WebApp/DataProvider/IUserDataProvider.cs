using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IUserDataProvider
    {
        Task<UserList> AuthenticateUser(string userName);
        IEnumerable<UserList> GetUsers(int userID = 0);
        Task<IEnumerable<UserList>> GetUserById(string userName);
        Task<string> AddOrUpdateUser(UserList userList, bool isupdate = false);
        Task<IEnumerable<LocationClass>> GetLocations();
        UserList ValidateUserInDomain(string userName);
        string GetDomainName();
        Task<int> CheckAdminExist();
    }
}