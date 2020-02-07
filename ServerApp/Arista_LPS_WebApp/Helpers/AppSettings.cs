namespace Arista_LPS_WebApp.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
		
        public string ConnectionString { get; set; } 

        public string DOMAIN { get; set; }

        public string NetworkSharedFolder { get; set; }
        public string DefaultSTID { get; set; }
        public string TrackingSTID { get; set; }
        public string Laser { get; set; }
        public string SSO { get; set; }
        public string RecordCount { get; set; }
    }
}


