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
using Microsoft.Phone.Shell;

namespace AzureManager
{
	public partial class CreateSubscription : PhoneApplicationPage
	{
		public CreateSubscription()
		{
			InitializeComponent();
			CreateSubscriptionViewModel model = new CreateSubscriptionViewModel(this.Dispatcher);
			model.SubscriptionDataLoaded += new EventHandler(model_SubscriptionDataLoaded);
			DataContext = model;

		}

		void model_SubscriptionDataLoaded(object sender, EventArgs e)
		{
			((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			((CreateSubscriptionViewModel)DataContext).SubscriptionId = NavigationContext.QueryString["subscriptionId"];
			((CreateSubscriptionViewModel)DataContext).DevicePin = NavigationContext.QueryString["devicePin"];
			((CreateSubscriptionViewModel)DataContext).CertificatePassword = NavigationContext.QueryString["certificatePassword"];
			((CreateSubscriptionViewModel)DataContext).DownloadAndStoreCertificate(null);
		}

		private void ApplicationBarIconButton_OK(object sender, EventArgs e)
		{
			((CreateSubscriptionViewModel)DataContext).SaveSubscription();
			NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
		}

		private void ApplicationBarIconButton_Cancel(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
		}
	}
}