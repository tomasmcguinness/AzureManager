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
using System.Collections.Generic;

namespace AzureManager.ViewModels
{
    public class DeploymentsViewModel : ViewModel
    {
        private bool isLoadingDeploymentList;

        public DeploymentsViewModel(Dispatcher dispatcher)
            : base(dispatcher)
        {
            DeploymentList = new ObservableCollection<DeploymentInfo>();
        }

        public string ServiceName { get; set; }
        public ObservableCollection<DeploymentInfo> DeploymentList { get; set; }

        public bool IsLoadingDeploymentList
        {
            get
            {
                return isLoadingDeploymentList;
            }
            set
            {
                isLoadingDeploymentList = value;
                NotifyPropertyChanged("IsLoadingDeploymentList");
            }
        }

        public override void Load()
        {
            IsLoadingDeploymentList = true;

            AzureManagementModel model = new AzureManagementModel();

            try
            {
                model.GetDeploymentsForService(SubscriptionManager.CurrentSubscription,
                                               ServiceName,
                                               GetDeploymentsForServiceCallback);
            }
            catch
            {
                CurrentDispatcher.BeginInvoke(() =>
                {
                    MessageBoxResult result = MessageBox.Show("Tap here to continue", "Subscription Details could not be retrieved due to an error.", MessageBoxButton.OKCancel);
                });
            }
        }

        private void GetDeploymentsForServiceCallback(List<DeploymentInfo> deployments)
        {
            CurrentDispatcher.BeginInvoke(() =>
            {
                DeploymentList.Clear();

                foreach (var depoyment in deployments)
                {
                    DeploymentList.Add(depoyment);
                }

                IsLoadingDeploymentList = false;
            });
        }
    }
}
