using Arista_LPS_WebApp.Helpers;
using System.Collections.Generic;

namespace Arista_LPS_WebApp.Entities
{
    public class AdditionalChargesInfo
    {
        public string userName { get; set; }
        public List<AdditionalCharges> additionalCharges { get; set; }
    }

    public class AdditionalCharges
    {
        public int id { get; set; }
        public string jobName { get; set; }
        public string amount { get; set; }
        public string description { get; set; }
        public string feeDesc { get; set; }
        public int chargeType { get; set; }

        public string runDate { get; set; }
        public string jobDetailRunDate { get; set; }
        public int jobRecordId { get; set; }

    }
    public class ACJobDetails
    {

        public string jobName { get; set; }
        private string rundate;
        public string runDate
        {
            get
            {
                return rundate.ConvertToCustomDateFormat();
            }
            set
            {
                rundate = value;
            }
        }
        public bool processedFlag;
        public int jobRecordId { get; set; }
    }

    public class GetACDetails
    {

        public string jobName { get; set; }
        public string customerCode { get; set; }
        public string customerName { get; set; }
        public string clientCode { get; set; }
        public string clientName { get; set; }
        public int jobRecordID { get; set; }
        public int acRecordID { get; set; }
        public decimal amount { get; set; }
        public string runDate { get; set; }
        public int feeID { get; set; }
        public string feeDesc { get; set; }
        public string comments { get; set; }
        public string enteredUser { get; set; }
        public string dateEntered { get; set; }
    }
}
