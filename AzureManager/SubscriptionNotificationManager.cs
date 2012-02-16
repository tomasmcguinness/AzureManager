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
using AzureManager.Models;

namespace AzureManager
{
    public static class SubscriptionNotificationManager
    {
        public static event NewSubscriptionEvent NewSubscriptionReceived;
        public delegate void NewSubscriptionEvent(string subscriptionId, string devicePin);

        public static void SendNewSubscriptionReceivedEvents(string subscriptionId, string devicePin)
        {
            if (NewSubscriptionReceived != null)
            {
                NewSubscriptionReceived(subscriptionId, devicePin);
            }
        }
    }
}
