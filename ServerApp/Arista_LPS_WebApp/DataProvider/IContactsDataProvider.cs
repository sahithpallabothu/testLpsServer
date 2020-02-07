using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IContactsDataProvider
    {
        Task<IEnumerable<Contacts>> GetContacts(int clientId);
        Task<IEnumerable<ContactApplication>> GetApplications(int clientId,int contactId);
        Task<string> AddOrUpdateContact(Contacts Contact,bool isupdate = false);
        Task<string> DeleteContact(int roleId, int clientID, int lastDeleteFlag);
    }
}
