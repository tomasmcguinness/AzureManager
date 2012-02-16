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

namespace AzureManager.ViewModels
{
    public class RequestPrivateKeyViewModel : ViewModel
    {
        private string devicePin;
        private string certificatePassword;

        public RequestPrivateKeyViewModel(Dispatcher currentDispatcher)
            : base(currentDispatcher)
        {
            // TODO REMOVE
            this.CertificatePassword = "abc123";
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
    }
}
