using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureManager.ViewModels
{
    class RequestState
    {
        public System.Net.HttpWebRequest Request { get; set; }
        public string SubscriptionId { get; set; }
        public byte[] Certificate { get; set; }
        public string DevicePin { get; set; }
    }
}
