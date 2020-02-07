using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface ICustomerDataProvider
    {
        Task<IEnumerable<Customer>> GetCustomers(int customerId = 0,bool isCustomer = false);
        Task<string> AddOrUpdateCustomer(Customer customer,bool isupdate = false);
        Task<IEnumerable<Customer>> GetViewCustomer(Customer customer);
        Task<IEnumerable<HeldType>> GetHeldType();
    }
}
