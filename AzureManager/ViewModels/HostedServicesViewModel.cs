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
    public class HostedServicesViewModel : ViewModel
    {
        private bool isLoadingHostedService;
        public event EventHandler HostedServiceSelected;

        public HostedServicesViewModel(Dispatcher dispatcher)
            : base(dispatcher)
        {
            HostedServiceList = new ObservableCollection<HostedService>();
            SelectHostedServiceCommand = new DelegateCommand(SelectHostedService);
        }

        public ObservableCollection<HostedService> HostedServiceList { get; set; }
        public bool IsLoadingHostedServices { get { return isLoadingHostedService; } set { isLoadingHostedService = value; NotifyPropertyChanged("IsLoadingHostedServices"); } }
        public ICommand SelectHostedServiceCommand { get; set; }

        public override void Load()
        {
            IsLoadingHostedServices = true;

            AzureManagementModel model = new AzureManagementModel();
            byte[] certificate = SubscriptionManager.GetSubscriptionCertificate(SubscriptionId);

            try
            {
                model.GetHostedServicesForSubscription(SubscriptionId, certificate, "abc123", HostedServiceListCallback);
            }
            catch
            {
                CurrentDispatcher.BeginInvoke(() =>
                   {
                       MessageBoxResult result = MessageBox.Show("Tap here to continue", "Subscription Details could not be retrieved due to an error.", MessageBoxButton.OKCancel);
                   });
            }
        }

        private void HostedServiceListCallback(HostedServicesModel data)
        {
            CurrentDispatcher.BeginInvoke(() =>
            {
                HostedServiceList.Clear();

                foreach (var service in data.HostedServices)
                {
                    HostedServiceList.Add(service);
                }

                IsLoadingHostedServices = false;
            });
        }

        public void SelectHostedService(object state)
        {
            if (HostedServiceSelected != null)
            {
                HostedServiceSelected(this, null);
            }
        }
    }
}
