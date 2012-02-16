using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureManager.Models
{
    public class SubscriptionInformationModel
    {
        public string SubscriptionName { get; set; }
        public string SubscriptionStatus { get; set; }
        public string AccountAdminEmail { get; set; }
        public string ServiceAdminEmail { get; set; }
    }
}
