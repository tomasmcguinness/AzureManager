using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AzureManager.ViewModels;
using System.Xml.Linq;

namespace AzureManager.Models
{
    public class HostedServicesModel
    {
        public HostedServicesModel()
        {
            HostedServices = new List<HostedService>();
        }

        public void GetHostedServicesForSubscription(string subscriptionId,
                                                     byte[] certificate,
                                                     string certificatePassword,
                                                     Action<HostedServicesModel> callback)
        {
            string url = "https://management.core.windows.net/" + subscriptionId + "/services/hostedservices";

            AzureManagementModel.InvokeRequestState<HostedServicesModel> state = new AzureManagementModel.InvokeRequestState<HostedServicesModel>
            {
                FinalCallback = callback
            };

            AzureManagementModel azureModel = new AzureManagementModel();
            azureModel.InvokeRequest<HostedServicesModel>(url, certificate, certificatePassword, null, state, GetHostedServicesForSubscriptionCallback);
        }

        public object GetHostedServicesForSubscriptionCallback(object returnedObject, string xml)
        {
            AzureManagementModel.InvokeRequestState<HostedServicesModel> state = returnedObject as AzureManagementModel.InvokeRequestState<HostedServicesModel>;

            HostedServicesModel model = new HostedServicesModel();

            //<HostedServices xmlns="http://schemas.microsoft.com/windowsazure">
            //<HostedService>
            //  <Url>hosted-service-request-uri</Url>
            //  <ServiceName>hosted-service-name</ServiceName>
            //</HostedService>  

            XElement rootElement = XElement.Parse(xml);
            XNamespace ns = "http://schemas.microsoft.com/windowsazure";

            foreach (var service in rootElement.Elements(ns + "HostedService"))
            {
                HostedServices.Add(new HostedService()
                {
                    ServiceName = service.Element(ns + "ServiceName").Value,
                    Url = service.Element(ns + "Url").Value
                });
            }

            state.FinalCallback(model);
            return null;
        }

        public List<HostedService> HostedServices { get; private set; }
    }
}
