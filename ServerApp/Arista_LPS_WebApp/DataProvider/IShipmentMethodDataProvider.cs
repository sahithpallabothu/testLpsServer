using Arista_LPS_WebApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.DataProvider
{
    public interface IShipmentMethodDataProvider
    {
        Task<IEnumerable<ShipmentMethod>> GetShipmentMethod(bool fromShipment);
        Task<string> AddOrUpdateShipmentMethod(ShipmentMethod shipmentMethod,bool isupdate = false);
        Task<string> DeleteShipmentMethod(int shipmentMethodID);
    }
}
