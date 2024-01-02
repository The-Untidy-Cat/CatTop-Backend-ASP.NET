using System.Globalization;

namespace asp.net.Services
{
    public class CustomService
    {
        public static string FormatVietnameseCurrency(double amount)
        {
            // Create a CultureInfo for Vietnamese culture
            CultureInfo vietnameseCulture = new CultureInfo("vi-VN");

            // Format the currency using the "C" format specifier and Vietnamese culture
            string formattedCurrency = amount.ToString("C", vietnameseCulture);

            return formattedCurrency;
        }
    }
}
