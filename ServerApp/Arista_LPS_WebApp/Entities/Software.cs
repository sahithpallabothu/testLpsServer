using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Entities
{
    public class Software
    {
        public int Id { get; set; }
        public string software { get; set; }
        public Boolean Active { get; set; }
        public string ISINUSE { get; set; }
    }
}
