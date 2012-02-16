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
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.IO.IsolatedStorage;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace AzureManager
{
    public partial class ViewSubscriptionOld : PhoneApplicationPage
    {
        public ObservableCollection<HostedService> HostedServices { get; set; }

        public ViewSubscriptionOld()
        {
            InitializeComponent();
            HostedServices = new ObservableCollection<HostedService>();
            DataContext = this;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Load the hosted services!
            //
            string subscriptionId = "27b0e36c-5a3f-43e5-b104-1f5bdd1fa0e4";

            // The opperation to be performed. This value can be modified to reflect the operation being performed.
            string operationName = "hostedservices";

            // Build a URI for https://management.core.windows.net/<subscription-id>/services/<operation-type>
            Uri requestUri = new Uri("https://management.core.windows.net/"
                                    + subscriptionId
                                    + "/services/"
                                    + operationName);

            byte[] certificate = GetCertificateDataForSubscription(subscriptionId);

            if (certificate == null)
            {
                // We need the subscription's certificate??
            }
            else
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri("http://localhost:6285/Services/InvokeSignedRequest"));
                request.Headers["X-Management-Url"] = requestUri.ToString();
                request.Method = "POST";

                RequestState state = new RequestState();
                state.Request = request;
                state.SubscriptionId = subscriptionId;
                state.Certificate = certificate;

                request.BeginGetRequestStream(GetRequestStreamCallback, state);
            }
        }

        void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            RequestState state = (RequestState)asynchronousResult.AsyncState;
            HttpWebRequest request = state.Request;

            Stream postStream = request.EndGetRequestStream(asynchronousResult);
            postStream.Write(state.Certificate, 0, state.Certificate.Length);
            postStream.Close();
            request.BeginGetResponse(new AsyncCallback(GetResponseCallback), state);
        }

        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            RequestState state = (RequestState)asynchronousResult.AsyncState;
            HttpWebRequest request = state.Request;

            // End the operation
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);

            XDocument doc = XDocument.Load(streamRead);

            // Close the stream object
            streamRead.Close();

            // Release the HttpWebResponse
            response.Close();
        }

        private byte[] GetCertificateDataForSubscription(string subscriptionId)
        {
            // Go to isolated storage!
            //
            byte[] data = null;

            // Obtain the virtual store for the application.
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();

            try
            {
                // Specify the file path and options.
                using (var isoFileStream = new IsolatedStorageFileStream(subscriptionId + "\\management.cer", FileMode.Open, FileAccess.Read, myStore))
                {
                    data = new byte[isoFileStream.Length];
                    isoFileStream.Read(data, 0, (int)isoFileStream.Length);
                }
            }
            catch { }

            return data;
        }

        private X509Certificate GetCertificateForSubscription(string subscriptionId)
        {
            // Go to isolated storage!
            //
            X509Certificate certificate = null;

            // Obtain the virtual store for the application.
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();

            // Specify the file path and options.
            using (var isoFileStream = new IsolatedStorageFileStream(subscriptionId + "\\management.cer", FileMode.Open, FileAccess.Read, myStore))
            {
                byte[] data = new byte[isoFileStream.Length];
                isoFileStream.Read(data, 0, (int)isoFileStream.Length);
                certificate = new X509Certificate(data);
            }

            return certificate;
        }

        private void HostedServicesLoaded(IAsyncResult asynchronousResult)
        {
            RequestState requestState = (RequestState)asynchronousResult.AsyncState;
            HttpWebRequest request = requestState.Request;

            try
            {
                HttpWebResponse resp = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            }
            catch (WebException exp)
            {
                Debug.WriteLine(exp.Status);
            }
        }
    }
}