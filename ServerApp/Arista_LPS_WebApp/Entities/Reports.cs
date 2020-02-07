using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Entities
{
    public class Reports
    {
        public string Hold;
        public string Active;
        public string PDF;
        public string EBPP;
        public string CustomerFlag;
        public string ClientActive;

        public string HeldReason { get; set; }
        public string AppType { get; set; }
        public string Stid { get; set; }
        public string AncillaryBill { get; set; }
        public string DetailBill { get; set; }
        public string InvoiceBill { get; set; }
        public string Itemized { get; set; }
        public string MultimeterBill { get; set; }
        public string MunicipalBill { get; set; }
        public string SummaryBill { get; set; }
        public string Mdm { get; set; }
        public string Tdhud { get; set; }
        public string Tva { get; set; }
        public string UnBundled { get; set; }
        public string Check { get; set; }
        public string Delinquent { get; set; }
        public string ThirdParty { get; set; }
        public string DM { get; set; }
        public string MailingState { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationCode { get; set; }
        public string ProcessingHold { get; set; }
        public string CustomerState { get; set; }
        public string Software { get; set; }
        public string PrintLocation { get; set; }
        public string Abpp { get; set; }
        public int RunFrequency { get; set; }
        public string PerfPatternDescription { get; set; }
        public string WindowEnvCode { get; set; }
        public string ReturnEnvCode { get; set; }
        public string MultiUp { get; set; }
        public string EmailAddress { get; set; }
        public string FormCode { get; set; }

    }
}
