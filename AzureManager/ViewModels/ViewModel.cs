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
using System.ComponentModel;
using System.Windows.Threading;

namespace AzureManager.ViewModels
{
	public class ViewModel : INotifyPropertyChanged
	{
		private Dispatcher currentDispatcher;
		protected string subscriptionId;

		public ViewModel()
		{ }

		public ViewModel(Dispatcher currentDispatcher)
		{
			this.currentDispatcher = currentDispatcher;
		}

		public virtual void Load()
		{

		}

		public Dispatcher CurrentDispatcher { get { return currentDispatcher; } }

		public virtual string SubscriptionId { get { return subscriptionId; } set { subscriptionId = value; NotifyPropertyChanged("SubscriptionId"); } }

		protected void NotifyPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
