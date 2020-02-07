using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Entities
{
    public class Size
    {
        [Key]
        public int SizeID { get; set; }
        public string size { get; set; }
        public string ISINUSE { get; set; }
    }
}
