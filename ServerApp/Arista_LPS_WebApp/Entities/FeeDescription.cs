using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Entities
{
    public class FeeDescription
    {

        [Key]
        public string RecId { get; set; }

        public string Description { get; set; }
        public Boolean Active { get; set; }
        public string ISINUSE { get; set; }
    }
}
