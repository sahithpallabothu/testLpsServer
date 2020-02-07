using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Entities
{
    public class States
    {
        [Key]
        public int StateID { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public string ISINUSE { get; set; }
    }
}
