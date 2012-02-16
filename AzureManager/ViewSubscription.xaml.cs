using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using AzureManager.ViewModels;

namespace AzureManager
{
    public partial class ViewSubscription : PhoneApplicationPage
    {
        public ViewSubscription()
        {
            InitializeComponent();
            ViewSubscriptionViewModel model = new ViewSubscriptionViewModel(Dispatcher);
            model.HostedServices.HostedServiceSelected += new EventHandler(HostedServices_HostedServiceSelected);
            this.DataContext = model;
        }

        private void HostedServices_HostedServiceSelected(object sender, EventArgs e)
        {
            string url = string.Format("/HostedServiceView.xaml?subscriptionId={0}&serviceName={1}",
                                       ((ViewSubscriptionViewModel)this.DataContext).SubscriptionId,
                                       "caffeineclub");
            NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            ((ViewSubscriptionViewModel)this.DataContext).SubscriptionName = NavigationContext.QueryString["subscriptionName"];
            ((ViewSubscriptionViewModel)this.DataContext).SubscriptionId = NavigationContext.QueryString["subscriptionId"];
            base.OnNavigatedTo(e);
        }

        private void PanoramaItem_Loaded(object sender, RoutedEventArgs e)
        {
            PivotItem item = sender as PivotItem;

            if (item != null)
            {
                HostedServicesViewModel model = item.DataContext as HostedServicesViewModel;

                if (model != null)
                {
                    model.Load();
                }
            }
        }

    }
}