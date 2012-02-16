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
using System.IO;
using System.IO.IsolatedStorage;
using System.ComponentModel;
using System.Windows.Threading;
using AzureManager.Models;

namespace AzureManager.ViewModels
{
	public class CreateSubscriptionViewModel : ViewModel
	{
		private string subscriptionName = "loading...";
		private string subscriptionStatus = "loading...";
		private string accountAdminEmail = "loading...";
		private string serviceAdminEmail = "loading...";
		private string certificateStatus = "loading...";
		private string devicePin;
		private string certificatePassword;
		private byte[] certificateData;
		private bool loadingSubscriptionData;

		public event EventHandler SubscriptionDataLoaded;

		public CreateSubscriptionViewModel(Dispatcher currentDispatcher)
			: base(currentDispatcher)
		{ }

		public string CertificateStatus
		{
			get { return certificateStatus; }
			set { certificateStatus = value; NotifyPropertyChanged("CertificateStatus"); }
		}

		public string SubscriptionName
		{
			get { return subscriptionName; }
			set { subscriptionName = value; NotifyPropertyChanged("SubscriptionName"); }
		}

		public string AccountAdminEmail
		{
			get { return accountAdminEmail; }
			set { accountAdminEmail = value; NotifyPropertyChanged("AccountAdminEmail"); }
		}

		public string ServiceAdminEmail
		{
			get { return serviceAdminEmail; }
			set { serviceAdminEmail = value; NotifyPropertyChanged("ServiceAdminEmail"); }
		}

		public string DevicePin
		{
			get { return devicePin; }
			set { devicePin = value; NotifyPropertyChanged("DevicePin"); }
		}

		public string CertificatePassword
		{
			get { return certificatePassword; }
			set { certificatePassword = value; NotifyPropertyChanged("PrivateKey"); }
		}

		public bool LoadingSubscriptionData { get { return loadingSubscriptionData; } set { loadingSubscriptionData = value; NotifyPropertyChanged("LoadingSubscriptionData"); } }

		public void DownloadAndStoreCertificate(object state)
		{
			LoadingSubscriptionData = true;

			string url = string.Format("http://{0}/Services/RetrieveCertificate/{1}/{2}", App.ServiceHostName, DevicePin, SubscriptionId);
			HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(url);
			sendNotificationRequest.Method = "GET";

			RequestState myRequestState = new RequestState();
			myRequestState.SubscriptionId = SubscriptionId;
			myRequestState.DevicePin = DevicePin;
			myRequestState.Request = sendNotificationRequest;
			sendNotificationRequest.BeginGetResponse(DownloadAndStoreCertificateCallback, myRequestState);
		}

		private void DownloadAndStoreCertificateCallback(IAsyncResult asynchronousResult)
		{
			RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
			HttpWebRequest myHttpWebRequest2 = myRequestState.Request;

			HttpWebResponse resp = (HttpWebResponse)myHttpWebRequest2.EndGetResponse(asynchronousResult);

			using (BinaryReader r = new BinaryReader(resp.GetResponseStream()))
			{
				certificateData = r.ReadBytes((int)resp.ContentLength);
			}

			CurrentDispatcher.BeginInvoke(() =>
			{
				CertificateStatus = "downloaded successfully";
				RetrieveSubscriptionName();
			});
		}

		private void RetrieveSubscriptionName()
		{
			// Tell the user we're doing something...
			//
			SubscriptionName = "loading...";

			// Create a request for the intermediate.
			//
			AzureManagementModel model = new AzureManagementModel();
			model.GetSubscriptionInformation(SubscriptionId, certificateData, CertificatePassword, SubscriptionInformationRetrieved);
		}

		private void SubscriptionInformationRetrieved(SubscriptionInformationModel model)
		{
			CurrentDispatcher.BeginInvoke(() =>
			{
				SubscriptionName = model.SubscriptionName;
				subscriptionStatus = model.SubscriptionStatus;
				AccountAdminEmail = model.AccountAdminEmail;
				ServiceAdminEmail = model.ServiceAdminEmail;

				LoadingSubscriptionData = false;

				if (SubscriptionDataLoaded != null)
				{
					SubscriptionDataLoaded(this, null);
				}
			});
		}

		public void SaveSubscription()
		{
			IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
			myStore.CreateDirectory(subscriptionId);

			SaveSubscriptionInfo();

			SaveCertificate();
		}

		private void SaveSubscriptionInfo()
		{
			IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();

			Subscription info = new Subscription()
			{
				SubscriptionId = SubscriptionId,
				Name = SubscriptionName
			};

			// Build a small XML document to hold the details...
			//
			byte[] fileBytes = SubscriptionManager.CreateSubscriptionFile(info);

			string detailsFileName = SubscriptionId + "\\details.xml";
			FileMode mode = FileMode.Create;

			using (var isoFileStream = new IsolatedStorageFileStream(detailsFileName, mode, myStore))
			{
				isoFileStream.Write(fileBytes, 0, fileBytes.Length);
			}
		}

		private void SaveCertificate()
		{
			IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();

			using (var isoFileStream = new IsolatedStorageFileStream(SubscriptionId + "\\management.cer", FileMode.Create, myStore))
			{
				isoFileStream.Write(certificateData, 0, certificateData.Length);
			}
		}
	}
}
