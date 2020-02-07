using System;
using System.Collections.Generic;
using System.Linq;

namespace Arista_LPS_WebApp.Entities
{
    public class ClientServiceAgreement
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string CustomerType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string TermNoticeDate { get; set; }
        public int InitialTerm { get; set; }
        public int RenewalTerm { get; set; }
        public int SLA { get; set; }   
        public Decimal MinChargeAmt { get; set; }
        public string SEDCMBRNO { get; set; }
        public int BillType { get; set; }
        public bool MinCharge { get; set; }
        public string SystemNumber { get; set; }
        public Decimal PGBillRate { get; set; }
        public Decimal CSBillRate { get; set; }
        public Decimal NonAutoFeeRate { get; set; }
        public Decimal PDFProcessingFee { get; set; }
        public Decimal EBPPProcessingRate { get; set; }
        public bool ClientType { get; set; }

        // EBPPMonthlyServFee?:number;
        // EBPPMinMonthlyFee?:number;
        // public List<Contracts> contractDetails { get; set; }     
        //public List<BillingRates> billingRates { get; set; }   

    }

    public class Contracts
    {


        public int ClientContractID { get; set; }
        public int ClientID { get; set; }
        public string ContractType { get; set; }
        public DateTime ContractDate { get; set; }
        public string Contract { get; set; }
        public string ContractTypeDesc { get; set; }


        public string ClientName { get; set; }
        public string CustomerType { get; set; }
        public List<Contracts> DeletedFiles { get; set; }
        public List<Contracts> ModifiedFiles { get; set; }
        public List<String> OldFiles { get; set; }
    }

    public class BillingRateInfo{
         public int ClientID { get; set; }   
         public List<BillingRates> billingRates { get; set; }    
    }

    public class BillingRates
    {
        public int ApplicationID { get; set; }
        public int ClientID { get; set; }    
        public string ApplicationName { get; set; }
        public string ApplicationCode { get; set; }
        public string AppID { get; set; }
        public string PrintOrder { get; set; }
        public string StatementName { get; set; }
        public string ConsolidationAcc { get; set; }
        public bool IsActive { get; set; }
        public List<CustomerRate> customerRateDetails { get; set; }   
    }



    public class CustomerRate
    {
        public int CustomerRateID { get; set; }
        public int CustomerID { get; set; }
        public string AppID { get; set; }
        public int RateTypeID { get; set; }
        public Decimal Rate { get; set; }
        public string Description { get; set; }
        //public int MaxCustRateID { get; set; }
    }

    public class ContractType
    {
        public int ContractTypeID { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }

    
    public class RateType
    {
        public int RateTypeID { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}
