using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureManager.Web.Models
{
    public class DeviceRegistration
    {
        public long Id { get; set; }
        public DateTime Registered { get; set; }
        public string ChannelUri { get; set; }
        public string DevicePin { get; set; }
        public short DeviceType { get; set; }
        public short UsageAttempts { get; set; }
        public bool Exhausted { get; set; }

        public byte[] Certificate { get; set; }
        public string SubscriptionId { get; set; }

        public void ClearInfomation()
        {
            Certificate = null;
            SubscriptionId = null;
        }
    }
}
