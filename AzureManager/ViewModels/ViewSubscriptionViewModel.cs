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
using System.Windows.Threading;

namespace AzureManager.ViewModels
{
    public class ViewSubscriptionViewModel : ViewModel
    {
        private string subscriptionName;

        public ViewSubscriptionViewModel(Dispatcher dispatcher)
            : base(dispatcher)
        {
            HostedServices = new HostedServicesViewModel(dispatcher);
        }

        public HostedServicesViewModel HostedServices { get; set; }

        public override string SubscriptionId
        {
            set
            {
                subscriptionId = value;
                HostedServices.SubscriptionId = value;
            }
            get
            {
                return subscriptionId;
            }
        }

        public string SubscriptionName
        {
            get { return subscriptionName; }
            set { subscriptionName = value; NotifyPropertyChanged("SubscriptionName"); }
        }
    }
}
