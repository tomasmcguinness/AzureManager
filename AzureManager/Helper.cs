using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace AzureManager
{
    public class Helper
    {
        public static string GetUrlWithHostName(string url)
        {
            string hostname = App.ServiceHostName;
            string newUrl = string.Format(url, hostname);
            return newUrl;
        }
    }
}
