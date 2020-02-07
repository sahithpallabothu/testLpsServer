using System;

namespace Arista_LPS_WebApp.Entities
{
    public class SearchDataForHome
    {
        public int SelectedClientId { get; set; } = 0;
        public DateTime? SelectedStartDate { get; set; }
        public DateTime? selectedEndDate { get; set; }
        public string telephone { get; set; } = string.Empty;
        public string fax { get; set; } = string.Empty;

        public string IVRPhoneNumber { get; set; } = string.Empty;
        public int mailerId { get; set; } = 0;
        public int crid { get; set; } = 0;
        public string comments { get; set; } = string.Empty;
        public string selectedCustName { get; set; } = string.Empty;
        public string selectedCustCode { get; set; } = string.Empty;
        public string selectedAppName { get; set; } = string.Empty;
        public string selectedAppCode { get; set; } = string.Empty;
        public bool pdf { get; set; } = false;
        public bool ivr { get; set; } = false;
        public bool ebpp { get; set; } = false;
        public bool dm { get; set; } = false;
    }
}
