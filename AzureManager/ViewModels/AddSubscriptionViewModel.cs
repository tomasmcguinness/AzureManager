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
using Microsoft.Phone.Notification;
using System.Diagnostics;
using AzureManager.Common.Data;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.IO.IsolatedStorage;
using AzureManager.ViewModels;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace AzureManager.Models
{
	public class AddSubscriptionViewModel : ViewModel
	{
		private bool isRegistering;
		private bool isRegistered;
		private string registrationCode;

		public AddSubscriptionViewModel()
		{
			Subscriptions = new ObservableCollection<Subscription>();
			RegisterDeviceCommand = new DelegateCommand(this.RegisterDevice, CanRegisterDevice);
		}

		public bool IsRegistering
		{
			get { return isRegistering; }
			set { isRegistering = value; RegisterDeviceCommand.RaiseCanExecuteChanged(); NotifyPropertyChanged("IsRegistering"); }
		}

		public bool IsRegistered
		{
			get { return isRegistered; }
			set { isRegistered = value; RegisterDeviceCommand.RaiseCanExecuteChanged(); NotifyPropertyChanged("IsRegistered"); }
		}

		public string RegistrationCode
		{
			get { return registrationCode; }
			set { registrationCode = value; NotifyPropertyChanged("RegistrationCode"); }
		}

		public bool CanRegisterDevice(object state)
		{
			return !IsRegistering && !IsRegistered;
		}

		public void RegisterDevice(object state)
		{
			IsRegistering = true;

			HttpNotificationChannel pushChannel;
			pushChannel = HttpNotificationChannel.Find("RegistrationChannel");

			if (pushChannel == null)
			{
				pushChannel = new HttpNotificationChannel("RegistrationChannel");
			}

			RegistrationRequest req = new RegistrationRequest()
			{
				ChannelUri = pushChannel.ChannelUri.ToString(),
				DeviceType = (short)Common.Data.DeviceType.WindowsPhone7
			};

			string json = null;

			using (MemoryStream ms = new MemoryStream())
			{
				DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RegistrationRequest));
				serializer.WriteObject(ms, req);
				json = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
			}

			WebClient registrationClient = new WebClient();
			registrationClient.Headers["content-type"] = "application/json";
			registrationClient.UploadStringCompleted += registrationClient_UploadStringCompleted;
			string url = string.Format("http://{0}/Services/RegisterDevice", App.ServiceHostName);
			registrationClient.UploadStringAsync(new Uri(url), json);
		}

		void registrationClient_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
		{
			IsRegistering = false;

			if (e.Error == null)
			{
				DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(RegistrationResponse));
				using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(e.Result)))
				{
					RegistrationResponse resp = json.ReadObject(ms) as RegistrationResponse;
					RegistrationCode = resp.PinCode;
				}

				IsRegistered = true;
			}
			else
			{
				Debug.WriteLine(e.Error.StackTrace);
			}
		}

		private void pushChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
		{
			if (SubscriptionReceived != null)
			{
				SubscriptionReceived(this, null);
			}
		}

		public void LoadAllSubscriptions()
		{
			// List all the folders for now
			//
			IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();

			string[] directoryNames = myStore.GetDirectoryNames();

			Subscriptions.Clear();

			foreach (string name in directoryNames)
			{
				if (name == "Shared") continue;

				// Read all details.xml files.
				//
				using (var isoFileStream = new IsolatedStorageFileStream(name + "\\details.xml", FileMode.Open, myStore))
				{
					byte[] file = new byte[isoFileStream.Length];
					isoFileStream.Read(file, 0, (int)isoFileStream.Length);

					Subscription info = ViewModels.SubscriptionManager.GetSubscriptionFromFile(file);
					Subscriptions.Add(info);
				}
			}

			if (Subscriptions.Count == 0)
			{

			}
		}

		public ObservableCollection<Subscription> Subscriptions { get; set; }

		// Commands
		public DelegateCommand RegisterDeviceCommand { get; set; }
		public ICommand DownloadCertificateCommand { get; set; }
		public event EventHandler SubscriptionReceived;
	}
}
