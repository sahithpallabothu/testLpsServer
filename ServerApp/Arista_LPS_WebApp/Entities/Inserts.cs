using Arista_LPS_WebApp.Helpers;

namespace Arista_LPS_WebApp.Entities
{
    public class Inserts
    {
        public int RecordID { get; set; }
        public int ApplicationID { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string ClientCode { get; set; }

        private string startDate;
        public string StartDate
        {
            get
            {
                return startDate.ConvertToCustomDateFormat();
            }
            set
            {
                startDate = value;
            }
        }

        private string endDate;
        public string EndDate
        {
            get
            {
                return endDate.ConvertToCustomDateFormat();
            }
            set
            {
                endDate = value;
            }
        }
        
        public string ReceivedDate { get; set; }
        public string ReceivedBy { get; set; }
        public string BinLocation { get; set; }
        
        public int InsertType { get; set; }
        public string InsertTypeName { get; set; }
        public string Description { get; set; }
        public int Weight { get; set; }
        public string Code { get; set; }
        public int Number { get; set; }
        public int ReturnedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public int ReorderQuantity { get; set; }
        public int UsedQuantity { get; set; }
        public bool ReturnInserts { get; set; }
        public string LocationInserts { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationCode { get; set; }
        public bool Active { get; set; }
        public int IsDelete { get; set; }

        public string UserAdded { get; set; }
        public string UserLast { get; set; }
        public string DateAdded { get; set; }
        public string DateLast { get; set; }

    }
}
