using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Entities
{
    public class RunningSummary
    {
       
        public int position { get; set; }
        
        public string jobname { get; set; }

        public string rundate { get; set; }

        public Boolean postflag { get; set; }

        public string pieces { get; set; }

        public string amount { get; set; }
        
        public string _2Ounces { get; set; }

        public Boolean isprintwinsalem { get; set; }

        public Boolean isautorun { get; set; }

        public Boolean isPrintWinSalem { get; set; }
        
    }

    
     public class UpdateJobPostFlag
    {
       
        public int recordId { get; set; }
        public int isPosted { get; set; }
        public string jobNumber { get; set; }
        public string trip { get; set; }
        public string runDate { get; set; }
    }
}
