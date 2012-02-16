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
using System.Collections.ObjectModel;
using AzureManager.Models;
using System.Windows.Threading;

namespace AzureManager.ViewModels
{
	public class HostedServiceViewModel : ViewModel
	{
		private string serviceName = "loading...";
		private string description = "loading...";
		private bool isLoadingHostedService;
		private string location = "loading...";
		private string affinityGroup = "loading...";
		private DeploymentInfo productionDeployment;
		private DeploymentInfo stagingDeployment;

		public HostedServiceViewModel(Dispatcher dispatcher)
			: base(dispatcher)
		{ }

		public string ServiceName
		{
			get { return serviceName; }
			set { serviceName = value; NotifyPropertyChanged("ServiceName"); }
		}

		public string Description
		{
			get { return description; }
			set { description = value; NotifyPropertyChanged("Description"); }
		}

		public string AffinityGroup
		{
			get { return affinityGroup; }
			set { affinityGroup = value; NotifyPropertyChanged("AffinityGroup"); }
		}

		public string Location
		{
			get { return location; }
			set { location = value; NotifyPropertyChanged("Location"); }
		}

		public bool IsLoadingHostedService
		{
			get { return isLoadingHostedService; }
			set { isLoadingHostedService = value; NotifyPropertyChanged("IsLoadingHostedService"); }
		}

		public DeploymentInfo ProductionDeployment { get { return productionDeployment; } private set { productionDeployment = value; NotifyPropertyChanged("ProductionDeployment"); NotifyPropertyChanged("ProductionDeploymentMissing"); } }
		public bool ProductionDeploymentMissing { get { return productionDeployment == null; } }
		public DeploymentInfo StagingDeployment { get { return stagingDeployment; } private set { stagingDeployment = value; NotifyPropertyChanged("StagingDeployment"); NotifyPropertyChanged("StagingDeploymentMissing"); } }
		public bool StagingDeploymentMissing { get { return stagingDeployment == null; } }

		public override void Load()
		{
			IsLoadingHostedService = true;

			AzureManagementModel model = new AzureManagementModel();
			model.GetHostedService(SubscriptionManager.CurrentSubscription, ServiceName, GetHostedServiceCallback);
		}

		private void GetHostedServiceCallback(HostedService data)
		{
			CurrentDispatcher.BeginInvoke(() =>
			{
				ServiceName = data.ServiceName;
				Description = data.Description;
				AffinityGroup = data.AffinityGroup;
				Location = data.Location;

				StagingDeployment = data.StagingDeployment;
				ProductionDeployment = data.ProductionDeployment;

				IsLoadingHostedService = false;
			});
		}
	}
}
