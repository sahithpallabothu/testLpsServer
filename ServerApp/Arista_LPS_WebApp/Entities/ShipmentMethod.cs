using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Entities
{
    public class ShipmentMethod
    {
        [Key]
        public int ShipmentMethodID { get; set; }
        public string shipmentMethod { get; set; }
        public string ISINUSE { get; set; }
    }
}
