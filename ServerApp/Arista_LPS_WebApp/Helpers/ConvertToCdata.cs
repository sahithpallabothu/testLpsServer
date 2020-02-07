namespace Arista_LPS_WebApp.Helpers
{
    public static class ConvertToCdata
    {
        public static string ConvertToCData(this string orginaldata)
        {
            string converteddata = string.Empty;
            converteddata = orginaldata.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("”", "&quot;").Replace("'", "&apos;");
            return converteddata;
        }
    }
}