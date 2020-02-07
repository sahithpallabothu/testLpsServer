using System.IO;

namespace Arista_LPS_WebApp.Helpers
{
    public class ServiceHelper
    {
        public static string GetNetworkPath(string networkSharedFolder, string customerType, string clientName, string fileName)
        {
            string networkFilePath = string.Empty;
            networkFilePath = string.Format(@"\{0}\{1}\{2}\{3}", networkSharedFolder, customerType, clientName, fileName);

            return networkFilePath;
        }

        public static bool DeleteFile(string networkSharedFolder, string customerType, string clientName, string fileName)
        {
            bool isDeleted = false;
            string filePath = string.Format(@"\{0}\{1}\{2}\{3}", networkSharedFolder, customerType, clientName, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                isDeleted = true;
            }

            return isDeleted;
        }
    }
}
