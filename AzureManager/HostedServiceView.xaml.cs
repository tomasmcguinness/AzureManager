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
	public partial class HostedServiceView : PhoneApplicationPage
	{
		public HostedServiceView()
		{
			InitializeComponent();
			HostedServiceViewModel model = new HostedServiceViewModel(Dispatcher);
			this.DataContext = model;
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			((HostedServiceViewModel)DataContext).ServiceName = NavigationContext.QueryString["serviceName"];
			((HostedServiceViewModel)DataContext).SubscriptionId = NavigationContext.QueryString["subscriptionId"];
			((HostedServiceViewModel)DataContext).Load();
			base.OnNavigatedTo(e);
		}

		private void ApplicationBarIconButton_Refresh(object sender, EventArgs e)
		{
			((HostedServiceViewModel)DataContext).Load();
		}
	}
}