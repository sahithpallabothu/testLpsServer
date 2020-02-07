using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Entities
{
    public class ChangePrintLocation
    {
       
        public int position { get; set; }
        
        public int clientID { get; set; }

        public string appName { get; set; }

        public string appCode { get; set; }

        public string hold { get; set; }

        public string auto { get; set; }
        
        public string inserts { get; set; }

        public int insert2Customer { get; set; }

        public int isPrintWinSalem { get; set; }
        public string heldReason { get; set; }
        
    }

     public class UpdateApplicationLocation
    {
       
        public int ApplicationId { get; set; }
        
        public Boolean isPrintWinSalem { get; set; }
    }
}
