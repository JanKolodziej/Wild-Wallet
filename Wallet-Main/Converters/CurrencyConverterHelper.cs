using System.Globalization;

namespace Wallet_Main.Converters
{
    public static class CurrencyConverterHelper
    {
        public static object Convert(object value)
        {
            if (value is decimal || value is double || value is int)
            {
                if (Preferences.Default.ContainsKey("UserCurrencyCode"))
                {
                    string symbolWaluty = Preferences.Default.Get("UserCurrencyCode", "");

                    return string.Format(CultureInfo.CurrentCulture, "{0:N0} {1}", value, symbolWaluty);
                }
                else
                {
                    return string.Format(CultureInfo.CurrentCulture, "{0:C0}", value);
                }
            }

            return value;
        }
    }
}
