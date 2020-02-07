using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Entities
{
    public class PRates
    {
        public int PostalTypeID { get; set; }
        public int TypeCode { get; set; }
        public decimal Rate { get; set; }
        public string Description { get; set; }
        public int? DisplayOrder { get; set; }
        public string StartDate { get; set; }
        public Boolean Active { get; set; }
    }
}
