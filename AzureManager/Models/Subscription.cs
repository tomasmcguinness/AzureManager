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
using AzureManager.ViewModels;

namespace AzureManager.Models
{
    public class Subscription
    {
        public string Name { get; set; }
        public string SubscriptionId { get; set; }
        public string CertificatePassword { get; set; }
        public bool RequiresCertificatePassword { get; set; }

        public byte[] Certificate
        {
            get
            {
                return SubscriptionManager.GetSubscriptionCertificate(SubscriptionId);
            }
        }
    }
}
