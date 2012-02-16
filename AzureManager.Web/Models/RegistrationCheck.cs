using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureManager.Web.Models
{
    public class RegistrationCheck
    {
        public string PinDigit1 { get; set; }
        public string PinDigit2 { get; set; }
        public string PinDigit3 { get; set; }
        public string PinDigit4 { get; set; }
        public string PinDigit5 { get; set; }
        public string PinDigit6 { get; set; }

        public string ChannelUri { get; set; }
    }
}