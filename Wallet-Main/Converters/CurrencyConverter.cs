using System.Globalization;
using Microsoft.Maui.Storage;

namespace Wallet_Main.Converters;

public class CurrencyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal || value is double || value is int)
        {
            if (Preferences.Default.ContainsKey("UserCurrency"))
            {
                string symbolWaluty = Preferences.Default.Get("UserCurrency", "");

                return string.Format(culture, "{0:N0} {1}", value, symbolWaluty);
            }
            else
            {
                return string.Format(culture, "{0:C0}", value);
            }
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}