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
using System.Xml.Linq;
using System.Text;
using AzureManager.Models;
using System.IO.IsolatedStorage;
using System.IO;

namespace AzureManager.ViewModels
{
    public class SubscriptionManager
    {
        public static Subscription CurrentSubscription { get; set; }

        public static byte[] CreateSubscriptionFile(Subscription info)
        {
            XElement rootElement = new XElement("subscription");
            rootElement.Add(new XElement("id", info.SubscriptionId));
            rootElement.Add(new XElement("name", info.Name));

            return Encoding.UTF8.GetBytes(rootElement.ToString());
        }

        public static Subscription GetSubscriptionFromFile(byte[] file)
        {
            if (file.Length <= 0) return null;

            XDocument doc = XDocument.Parse(System.Text.Encoding.UTF8.GetString(file, 0, file.Length));

            Subscription info = new Subscription()
            {
                SubscriptionId = doc.Root.Element("id").Value,
                Name = doc.Root.Element("name").Value,
                RequiresCertificatePassword = false,
                CertificatePassword = "abc123"
            };

            return info;
        }

        public static byte[] GetSubscriptionCertificate(string subscriptionId)
        {
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
            byte[] certificateBytes = null;

            // Go into the isolated storage and retrieve the certificate.
            //
            using (var isoFileStream = new IsolatedStorageFileStream(subscriptionId + "\\management.cer", FileMode.Open, myStore))
            {
                certificateBytes = new byte[isoFileStream.Length];
                isoFileStream.Read(certificateBytes, 0, (int)isoFileStream.Length);
            }

            return certificateBytes;
        }
    }
}
