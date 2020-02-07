using Arista_LPS_WebApp.Helpers;
using System;
using System.Collections.Generic;

namespace Arista_LPS_WebApp.Entities
{
    public class JobDetail
    {
        public int? RecordID { get; set; }
        public string JobName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationCode { get; set; }

        private string processedDate;
        public string ProcessedDate
        {
            get
            {
                return processedDate.ConvertToCustomDateFormat();
            }
            set
            {
                processedDate = value;
            }
        }

        private string runDate;

        public string RunDate
        {
            get
            {
                return runDate.ConvertToCustomDateFormat();
            }
            set
            {
                runDate = value;
            }
        }

        private string billDate;
        public string BillDate
        {
            get
            {
                return billDate.ConvertToCustomDateFormat();
            }
            set
            {
                billDate = value;
            }
        }

        public  string PostDate;

        public bool PostFlag { get; set; }
        public bool ProcessedFlag { get; set; }
        public bool Printed { get; set; }
        public string USPSTripCode { get; set; }

        public int AccountsDetail { get; set; }
        public int AccountsInvoice { get; set; }
        public int PagesDetail { get; set; }
        public int PagesInvoice { get; set; }
        public int OverflowDetail { get; set; }
        public int OverflowInvoice { get; set; }
        public int Suppressed { get; set; }

        public int Digit5 { get; set; }
        public int AADC { get; set; }
        public int MixedAADC { get; set; }
        public int Pressorted { get; set; }
        public int SinglePieces { get; set; }
       
        public int MeterPieces { get; set; }
        public decimal MeterAmount { get; set; }
        public int Held { get; set; }
        public int Foreign { get; set; }
        public int OutOfBalance { get; set; }
      
        public int Oz1 { get; set; }
        public int Oz2 { get; set; }
        public int Oz3 { get; set; }
        public int Oz4 { get; set; }
        public int Oz5 { get; set; }
        public int Oz6 { get; set; }
        public int Oz7 { get; set; }
        public int Oz8 { get; set; }
        public int Oz9 { get; set; }
        public int Oz10 { get; set; }
        public int Oz11 { get; set; }
        public int Oz12 { get; set; }
        public int Oz13 { get; set; }
        public int Oz14 { get; set; }
        public int Oz15 { get; set; }
        public int Oz16 { get; set; }
        public int OzPermit2 { get; set; }
        public int OzSingle2 { get; set; }

        /*insetd section */
        public int GlobalInsert { get; set; }
        public int Selective1 { get; set; }
        public int Selective2 { get; set; }
        public int Selective3 { get; set; }
        public int Selective4 { get; set; }
        public int Selective5 { get; set; }
        public int InsertsTotal { get; set; }

        public Decimal USPSPostageTotal { get; set; }
        public List<PostageAdditionalCharges> AdditionalCharges { get; set; }
        public List<Comments> Comments { get; set; }

        //for home
        public string InputName { get; set; }
        public string OutputName { get; set; }

        private string upload;
        public string Upload
        {
            get
            {
                return upload.ConvertToCustomDateFormat();
            }
            set
            {
                upload = value;
            }
        }

        private string received;
        public string Received
        {
            get
            {
                return received.ConvertToCustomDateFormat();
            }
            set
            {
                received = value;
            }
        }
    }

    public class PostageAdditionalCharges
    {
        public int ChargeId { get; set; }
        public int FeeId { get; set; }
        public string ChargeType { get; set; }
        public decimal ChargeAmount { get; set; }
        public string Description { get; set; }

        private string chargeDate;
        public string ChargeDate
        {
            get
            {
                return chargeDate.ConvertToCustomDateFormat();
            }
            set
            {
                chargeDate = value;
            }
        }
        //public string RunDate { get; set; }
        private string runDate;

        public string RunDate
        {
            get
            {
                return runDate.ConvertToCustomDateFormat();
            }
            set
            {
                runDate = value;
            }
        }
        public int UserId { get; set; }
        public string UserName { get; set; }

    }

    public class Comments
    {
        public int CommentsId { get; set; }
        public string JobComments { get; set; }
        private string commentsDate;
        public string CommentsDate
        {
            get
            {
                return commentsDate.ConvertToCustomDateFormat();
            }
            set
            {
                commentsDate = value;
            }
        }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
