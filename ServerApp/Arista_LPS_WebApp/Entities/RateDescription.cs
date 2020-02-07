using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Entities
{
    public class RateDescription
    {
        public int RateTypeID { get; set; }
        public string Description { get; set; }
        public Boolean Active { get; set; }
        public string ISINUSE { get; set; }
    }
}
