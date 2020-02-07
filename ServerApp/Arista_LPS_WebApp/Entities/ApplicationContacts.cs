using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Entities
{
    public class ApplicationContacts
    {
        public int ContactID { get; set; }
        public int ClientID { get; set; }

        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }

        public string ContactTitle { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }

        public string ContactExtension { get; set; }

        public string ContactCell { get; set; }

        public string ContactHome { get; set; }
        public bool BillingContact { get; set; }

        public bool BillingAlternateContact { get; set; }

        public bool OOBContact { get; set; }

        public bool DELQContact { get; set; }

        public bool CCContact { get; set; }

        public bool EBPPContact { get; set; }

        public bool InsertContact { get; set; }
        public bool InsertAlternateContact { get; set; }

        public bool EmailConfirmations { get; set; }
        public bool USPS { get; set; }
        public bool FedExUPSContact { get; set; }

        public string FedExUPSContactComment { get; set; }

        public string Comment { get; set; }

        public bool NotifyPDF { get; set; } = false;
        public bool NotifyJobComplete { get; set; } = false;
        public bool NotifyFileReceived { get; set; } = false;
        public bool EmailRMT { get; set; } = false;
        public bool EmailCode1 { get; set; } = false;
        public bool AlarmExist { get; set; }
        public int ApplicationID { get; set; }

    }

   


    public class ApplicationNotifications
    {
        /*  public int ClientID { get; set; }*/
         public int ContactID { get; set; } 
        public int ApplicationID { get; set; }
        public string EmailAddr { get; set; }
        public bool NotifyPDF { get; set; } = false;
        public bool NotifyJobComplete { get; set; } = false;
        public bool NotifyFileReceived { get; set; } = false;
        public bool EmailRMT { get; set; } = false;
        public bool EmailCode1 { get; set; } = false;
    }

    public class ApplicationNotificationsData{
       //   public string ContactIDs { get; set; }
          public int ApplicationID { get; set; }

           public List<ApplicationNotifications> NotificationList { get; set; }
    }

}