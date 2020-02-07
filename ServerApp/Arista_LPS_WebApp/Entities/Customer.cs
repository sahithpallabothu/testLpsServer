using System;
using System.Collections.Generic;
using System.Linq;

namespace Arista_LPS_WebApp.Entities
{
    public class Customer
    {
        public int CustomerId { set; get; }
        public string CustomerCode { set; get; }
        public string CustomerName { set; get; }
        public bool Active { set; get; }
        public bool SEDC { set; get; }
        public string Telephone { set; get; }
        public string Fax { set; get; }
   
        public string IVRPhoneNumber { set; get; }
        public int MailerId { set; get; }
        public int CRID { set; get; }
        public bool IVR { set; get; }
        public string Comment { set; get; }

        public bool IsAllowSave { set; get; }
      

        // Mailing Address
        public string MailingAddress1 { set; get; }
        public string MailingAddress2 { set; get; }
        public string MailingCity { set; get; }
        public string MailingState { set; get; }
        public string MailingZip { set; get; }

        // Physical Address
        public string PhysicalAddress1 { set; get; }
        public string PhysicalAddress2 { set; get; }
        public string PhysicalCity { set; get; }
        public string PhysicalState { set; get; }
        public string PhysicalZip { set; get; }


        // Check Held Address
        public string CheckHeldAddress1 { set; get; }
        public string CheckHeldAddress2 { set; get; }
        public string CheckHeldCity { set; get; }
        public string CheckHeldState { set; get; }
        public string CheckHeldZip { set; get; }
        public int CheckHeldContact { set; get; }
        public int CheckHeldShipmentMethod { set; get; }

        // Held Address
        public string HeldAddress1 { set; get; }
        public string HeldAddress2 { set; get; }
        public string HeldCity { set; get; }
        public string HeldState { set; get; }
        public string HeldZip { set; get; }
        public int HeldContact { set; get; }
        public int HeldShipmentMethod { set; get; }

        //search criteria for view customer.

        public string selectedValue { set; get; }
        public string searchText { set; get; }

        //held type
        public int CheckHeld1TypeId { set; get; }
        public int Held2TypeId { set; get; }

        //for population autocompletes
        public string CheckShipment { set; get; }
        public string HeldShipment { set; get; }
        public string HeldContactName { set; get; }
        public string CheckHeldContactName { set; get; }

        public string applicationName { set; get; }
        public string applicationCode { set; get; }
        public bool ApplicationActive { get; set; } = true;

    }

    public class HeldType
    {
        public int HeldTypeId { set; get; }
        public string Description { set; get; }
    }
}
