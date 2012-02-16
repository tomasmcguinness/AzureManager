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
using System.Security.Cryptography.X509Certificates;
using System.IO.IsolatedStorage;
using System.IO;

namespace AzureManager.Models
{
    public class SubscriptionStore
    {
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
