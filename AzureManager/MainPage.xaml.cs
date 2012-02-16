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
using AzureManager.Models;
using System.Diagnostics;
using AzureManager.ViewModels;

namespace AzureManager
{
	public partial class MainPage : PhoneApplicationPage
	{
		public MainPage()
		{
			InitializeComponent();
			DataContext = new AddSubscriptionViewModel();

			SubscriptionNotificationManager.NewSubscriptionReceived += SubscriptionManager_NewSubscriptionReceived;
		}

		void SubscriptionManager_NewSubscriptionReceived(string subscriptionId, string devicePin)
		{
			Dispatcher.BeginInvoke(() =>
			{
				MessageBoxResult result = MessageBox.Show("Tap here to continue", "New Subscription Available", MessageBoxButton.OKCancel);

				if (result == MessageBoxResult.OK)
				{
					string url = string.Format("/RequestPrivateKey.xaml?subscriptionId={0}&devicePin={1}", subscriptionId, devicePin);
					NavigationService.Navigate(new Uri(url, UriKind.Relative));
				}
			});
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			// May want to process a notifications
			if (NavigationContext.QueryString.ContainsKey("mode") && NavigationContext.QueryString["mode"] == "notification")
			{
				string subscriptionId = NavigationContext.QueryString["subscriptionId"];
				string devicePin = NavigationContext.QueryString["devicePin"];

				string url = string.Format("/RequestPrivateKey.xaml?subscriptionId={0}&devicePin={1}", subscriptionId, devicePin);
				NavigationService.Navigate(new Uri(url, UriKind.Relative));
			}

			((AddSubscriptionViewModel)DataContext).LoadAllSubscriptions();
			base.OnNavigatedTo(e);
		}

		private void ApplicationBarIconButton_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/AddSubscription.xaml", UriKind.Relative));
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Subscription selectedSubscription = ((Button)sender).DataContext as Subscription;

			SubscriptionManager.CurrentSubscription = selectedSubscription;

			string url = null;

			if (selectedSubscription.RequiresCertificatePassword)
			{

			}
			else
			{
				url = string.Format("/ViewSubscription.xaml?subscriptionId={0}&subscriptionName={1}",
										   selectedSubscription.SubscriptionId,
										   selectedSubscription.Name);
			}

			NavigationService.Navigate(new Uri(url, UriKind.Relative));
		}
	}
}