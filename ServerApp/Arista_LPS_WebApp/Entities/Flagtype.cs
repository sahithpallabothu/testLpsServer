using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Entities
{
    public class Flagtype
    {
        [Key]
        public int FlagId { get; set; }

        public string Description { get; set; }

        public string ISINUSE { get; set; }
    }
}
