using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AzureManager.Common.Data
{
    public enum DeviceType
    {
        WindowsPhone7
    }

    public class RegistrationRequest
    {
        public string ChannelUri { get; set; }
        public short DeviceType { get; set; }
    }
}
