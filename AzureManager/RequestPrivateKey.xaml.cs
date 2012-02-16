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
using System.Windows.Controls.Primitives;

namespace AzureManager
{
    public partial class RequestPrivateKey : PhoneApplicationPage
    {
        public RequestPrivateKey()
        {
            InitializeComponent();
            DataContext = new RequestPrivateKeyViewModel(Dispatcher);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            ((RequestPrivateKeyViewModel)DataContext).SubscriptionId = NavigationContext.QueryString["subscriptionId"];
            ((RequestPrivateKeyViewModel)DataContext).DevicePin = NavigationContext.QueryString["devicePin"];
			base.OnNavigatedTo(e);
        }

        private void ApplicationBarIconButton_OK(object sender, EventArgs e)
        {
            RequestPrivateKeyViewModel context = DataContext as RequestPrivateKeyViewModel;
            string url = string.Format("/CreateSubscription.xaml?subscriptionId={0}&devicePin={1}&certificatePassword={2}",
                                       context.SubscriptionId,
                                       context.DevicePin,
                                       context.CertificatePassword);

            NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }

        private void ApplicationBarIconButton_Cancel(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}