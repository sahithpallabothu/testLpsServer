using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Entities
{
    public class AppType
    {
        [Key]
        public string AppID { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string Username { get; set; }
        public string SEDCRateNumber { get; set; }
        public string ISINUSE { get; set; }
    }
}
