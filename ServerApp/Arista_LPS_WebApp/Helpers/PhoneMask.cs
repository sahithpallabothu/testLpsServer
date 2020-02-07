namespace Arista_LPS_WebApp.Helpers
{
    public static class PhoneMask
    {
        public static string UnMaskPhone(this string phone)
        {
            string unMaskedPhone = string.Empty;
            unMaskedPhone = phone.Replace("(","").Replace(")","").Replace("-","").Replace(" ","").Trim();

            return unMaskedPhone;
        }

        public static string MaskPhone(this string phone)
        {
            string maskedPhone = string.Empty;
            if (!string.IsNullOrEmpty(phone) && phone.Trim().Length == 10)
            {
                maskedPhone = string.Format("({0}) {1}-{2}", phone.Substring(0, 3), phone.Substring(3, 3), phone.Substring(6, 4));
            }
            return maskedPhone;
        }
    }
}