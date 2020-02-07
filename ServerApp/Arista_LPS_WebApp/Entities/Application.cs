using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Entities
{
    public class Application
    {
       /*Customer ID */
       public int ClientID { get; set; } 
        public int ApplicationID { get; set; }
         public string Stid { get; set; }/*combination of imbTracing & Endorsement*/
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationCode { get; set; }
        public bool ProcessingHold { get; set; }
         public string CustomerState {get; set;}
         public string CustomerFlag {get; set;}
         public int CustomerID {get; set;}
         public string CustomerNumber{get;set;}="";
         public string AppType {get; set;}
         public string Color {get; set;}
         //newly added
          public string HeldAddressState {get; set;}
          public string HeldAddress1 {get; set;}
          public string HeldAddress2 {get; set;}
          public string HeldCity {get; set;}
          public string HeldZipCode {get; set;}
          public string ContactName {get; set;}
          public string ShipmentMethod {get; set;}
          public string HeldAddressOther1 {get; set;}
          public string HeldAddressOther2 {get; set;}
          public string HeldCityOther {get; set;}
          public string HeldAddressStateOther {get; set;}
          public string HeldZipCodeOther {get; set;}
          public string HeldContactNameOther {get; set;}
          public string HeldShipmentMethodOther {get; set;}
          
          
          
          
          public string MailingAddress1 {get; set;}
          public string MailingAddress2 {get; set;}
          public string MailingState {get; set;}
          public string MailingZipCode {get; set;}
          public string MailngCity {get; set;}
         
         
          public string PhysicalAddress1 {get; set;}
          public string PhysicalAddress2 {get; set;}
          public string PhysicalCity {get; set;}
          public string PhysicalState {get; set;}
          public string PhysicalZipCode {get; set;}
         //newly added
         public string HoldReason {get; set;}
        public bool ClientActive { get; set; } = true;
         public bool Active {get; set;}
         public bool AutoRun {get; set;}
         public bool Ebpp {get; set;}
         public bool Abpp {get; set;}
         public bool ImbTracing {get; set;}
         public bool Pdf {get; set;}
         public bool Overlay {get; set;}
         public bool Backer {get; set;}
         public bool Duplex {get; set;}/*Field not in DB*/
         public string Software {get; set;}
         public string PrintLocation {get; set;}/*Field not in DB*/
                                                //public int RunFrequency { get; set; } = 0;
        public int? RunFrequency { get; set; } = null;
        public string RunFrequencyName { get; set; }
        public string Endorsement {get; set;}/*Field not in DB*/
         public string Paper {get; set;}/*Field not in DB*/
        public int? Perf { get; set; } = null;
        
        public string PerfPatternDescription { get; set; } 
         public int Size {get; set;}
         public int PrintSide {get; set;}
         public string OutsideEnvelope {get; set;}
         public string ReturnEnvelope {get; set;}
         public bool AncillaryBill {get; set;}
         public bool DetailBill {get; set;}
         public bool InvoiceBill {get; set;}
         public bool ItemizedBill {get; set;}
         public bool MultimeterBill {get; set;}
         public bool MunicipalBill {get; set;}
         public bool SummaryBill {get; set;}
         public bool Mdm {get; set;}
         public bool Tdhud {get; set;}
         public bool Tva {get; set;}
         public bool UnBundled {get; set;}
         public bool Check {get; set;}
         public bool Delinquent {get; set;}
         public bool ThirdParty {get; set;}
        public bool DM { get; set; }

        public string SearchText { get; set; }
        public string SearchOption { get; set; }

        //held type
        public int CheckHeld1TypeId { set; get; } = 0;
        public int Held2TypeId { set; get; } = 0;



    }
}
