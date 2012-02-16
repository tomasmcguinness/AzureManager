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
using System.Linq;
using System.Xml.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using AzureManager.ViewModels;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace AzureManager.Models
{
	public class AzureManagementModel
	{
		private const string managementRoot = "https://management.core.windows.net/";
		public class InvokeRequestState<T>
		{
			public Action<T> FinalCallback;
			public XElement AzureManagementXmlResponse;
		}

		public void GetSubscriptionInformation(string subscriptionId,
											   byte[] certificate,
											   string certificatePassword,
											   Action<SubscriptionInformationModel> callback)
		{
			string url = "https://management.core.windows.net/" + subscriptionId;

			InvokeRequestState<SubscriptionInformationModel> state = new InvokeRequestState<SubscriptionInformationModel>
			{
				FinalCallback = callback
			};

			InvokeRequest<SubscriptionInformationModel>(url, certificate, certificatePassword, null, state, GetSubscriptionInformationCallback);
		}

		public object GetSubscriptionInformationCallback(object returnedObject, string xml)
		{
			// Parse the response:
			InvokeRequestState<SubscriptionInformationModel> state = returnedObject as InvokeRequestState<SubscriptionInformationModel>;

			//<Subscription xmlns="http://schemas.microsoft.com/windowsazure" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
			//<SubscriptionID>27b0e36c-5a3f-43e5-b104-1f5bdd1fa0e4</SubscriptionID> 
			//<SubscriptionName>Caffeine Club MSDN</SubscriptionName> 
			//<SubscriptionStatus>Active</SubscriptionStatus> 
			//<AccountAdminLiveEmailId>tomas.mcguinness@barcap.com</AccountAdminLiveEmailId> 
			//<ServiceAdminLiveEmailId>tomas@chewedpencil.net</ServiceAdminLiveEmailId> 
			//<MaxCoreCount>20</MaxCoreCount> 
			//<MaxStorageAccounts>5</MaxStorageAccounts> 
			//<MaxHostedServices>6</MaxHostedServices> 
			//<CurrentCoreCount>1</CurrentCoreCount> 
			//<CurrentHostedServices>1</CurrentHostedServices> 
			//<CurrentStorageAccounts>1</CurrentStorageAccounts> 
			//</Subscription

			XElement rootElement = XElement.Parse(xml);
			XNamespace ns = "http://schemas.microsoft.com/windowsazure";
			SubscriptionInformationModel model = new SubscriptionInformationModel()
			{
				SubscriptionName = rootElement.Element(ns + "SubscriptionName").Value,
				SubscriptionStatus = rootElement.Element(ns + "SubscriptionStatus").Value,
				AccountAdminEmail = rootElement.Element(ns + "AccountAdminLiveEmailId").Value,
				ServiceAdminEmail = rootElement.Element(ns + "ServiceAdminLiveEmailId").Value
			};

			state.FinalCallback(model);

			return null;
		}

		public void GetHostedServicesForSubscription(string subscriptionId,
													 byte[] certificate,
													 string certificatePassword,
													 Action<HostedServicesModel> callback)
		{
			string url = "https://management.core.windows.net/" + subscriptionId + "/services/hostedservices";

			InvokeRequestState<HostedServicesModel> state = new InvokeRequestState<HostedServicesModel>
			{
				FinalCallback = callback
			};

			InvokeRequest<HostedServicesModel>(url, certificate, certificatePassword, null, state, GetHostedServicesForSubscriptionCallback);
		}

		public object GetHostedServicesForSubscriptionCallback(object returnedObject, string xml)
		{
			InvokeRequestState<HostedServicesModel> state = returnedObject as InvokeRequestState<HostedServicesModel>;

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
				model.HostedServices.Add(new HostedService()
				{
					ServiceName = service.Element(ns + "ServiceName").Value,
					Url = service.Element(ns + "Url").Value
				});
			}

			state.FinalCallback(model);
			return null;
		}

		public void GetHostedService(Subscription subscription, string serviceName, Action<HostedService> callback)
		{
			string url = string.Format("{0}{1}/services/hostedservices/{2}?embed-detail=true",
									   managementRoot,
									   subscription.SubscriptionId,
									   serviceName);

			InvokeRequestState<HostedService> state = new InvokeRequestState<HostedService>
			{
				FinalCallback = callback
			};

			byte[] certificate = subscription.Certificate;
			string password = subscription.CertificatePassword;

			InvokeRequest<HostedService>(url, certificate, password, null, state, GetHostedServiceCallback);
		}

		public object GetHostedServiceCallback(object returnedObject, string xml)
		{
			InvokeRequestState<HostedService> state = returnedObject as InvokeRequestState<HostedService>;

			HostedService model = new HostedService();

			//            <HostedService xmlns="http://schemas.microsoft.com/windowsazure">
			//  <Url>hosted-service-url</Url>
			//  <ServiceName>hosted-service-name</ServiceName>
			//  <HostedServiceProperties>
			//    <Description>description</Description>
			//    <Location>location</Location>
			//    <AffinityGroup>affinity-group</AffinityGroup>
			//    <Label>base-64-encoded-name-of-the-service</Label>
			//  </HostedServiceProperties>
			//  <Deployments>
			//    <Deployment>
			//      <Name>deployment-name</Name>
			//      <DeploymentSlot>deployment-slot</DeploymentSlot>
			//      <PrivateID>deployment-id</PrivateID>
			//      <Status>deployment-status</Status>
			//      <Label>base64-encoded-deployment-label</Label>
			//      <Url>deployment-url</Url>
			//      <Configuration>base-64-encoded-configuration-file</Configuration>
			//      <RoleInstanceList>
			//        <RoleInstance>
			//          <RoleName>role-name</RoleName>
			//          <InstanceName>role-instance-name</InstanceName>
			//          <InstanceStatus>instance-status</InstanceStatus>
			//        </RoleInstance>
			//      </RoleInstanceList>
			//      <UpgradeDomainCount>upgrade-domain-count</UpgradeDomainCount>
			//      <RoleList>
			//        <Role>
			//          <RoleName>role-name</RoleName>
			//          <OsVersion>operating-system-version</OsVersion>
			//        </Role>
			//      </RoleList>
			//      <SdkVersion>sdk-version-used-to-create-package</SdkVersion>
			//      <InputEndpointList>
			//         <InputEndpoint>
			//            <RoleName>role-name</RoleName>
			//            <Vip>virtual-ip-address</Vip>
			//            <Port>port-number</Port>
			//         </InputEndpoint>
			//         …
			//      </InputEndpointList>
			//      <Locked>deployment-write-allowed-status</Locked>
			//      <RollbackAllowed>rollback-operation-allowed</RollbackAllowed>
			//    </Deployment>
			//  </Deployments>
			//</HostedService>


			XElement rootElement = XElement.Parse(xml);
			XNamespace ns = "http://schemas.microsoft.com/windowsazure";

			XElement hostedServicePropertiesElement = rootElement.Element(ns + "HostedServiceProperties");

			model.ServiceName = rootElement.Element(ns + "ServiceName").Value;
			model.Description = hostedServicePropertiesElement.Element(ns + "Description").Value;

			if (hostedServicePropertiesElement.Element(ns + "Location") != null)
			{
				model.Location = hostedServicePropertiesElement.Element(ns + "Location").Value;
			}
			else
			{
				model.AffinityGroup = hostedServicePropertiesElement.Element(ns + "AffinityGroup").Value;
			}

			foreach (var deploymentElement in rootElement.Element(ns + "Deployments").Elements(ns + "Deployment"))
			{
				DeploymentInfo deployment = new DeploymentInfo()
				{
					Name = deploymentElement.Element(ns + "Name").Value,
					Slot = deploymentElement.Element(ns + "DeploymentSlot").Value,
					Status = deploymentElement.Element(ns + "Status").Value,
					Url = deploymentElement.Element(ns + "Url").Value
				};

				deployment.RoleCount = deploymentElement.Element(ns + "RoleList").Elements(ns + "Role").Count();
				deployment.InstanceCount = deploymentElement.Element(ns + "RoleInstanceList").Elements(ns + "RoleInstance").Count();
				deployment.EndPointCount = deploymentElement.Element(ns + "InputEndpointList").Elements(ns + "InputEndpoint").Count();

				if (deployment.Slot == "Production")
				{
					model.ProductionDeployment = deployment;
				}
				else
				{
					model.StagingDeployment = deployment;
				}
			}

			state.FinalCallback(model);
			return null;
		}

		public void GetDeploymentsForService(Subscription subscription, string serviceName, Action<List<DeploymentInfo>> callback)
		{
			string url = string.Format("{0}{1}/services/hostedservices/{2}?embed-detail=true",
									   managementRoot,
									   subscription.SubscriptionId,
									   serviceName);

			InvokeRequestState<List<DeploymentInfo>> state = new InvokeRequestState<List<DeploymentInfo>>
			{
				FinalCallback = callback
			};

			byte[] certificate = subscription.Certificate;
			string password = subscription.CertificatePassword;

			InvokeRequest<List<DeploymentInfo>>(url, certificate, password, null, state, GetDeploymentsForServiceCallback);
		}

		public object GetDeploymentsForServiceCallback(object returnedObject, string xml)
		{
			InvokeRequestState<List<DeploymentInfo>> state = returnedObject as InvokeRequestState<List<DeploymentInfo>>;

			List<DeploymentInfo> deployments = new List<DeploymentInfo>();

			//            <HostedService xmlns="http://schemas.microsoft.com/windowsazure">
			//  <Url>hosted-service-url</Url>
			//  <ServiceName>hosted-service-name</ServiceName>
			//  <HostedServiceProperties>
			//    <Description>description</Description>
			//    <Location>location</Location>
			//    <AffinityGroup>affinity-group</AffinityGroup>
			//    <Label>base-64-encoded-name-of-the-service</Label>
			//  </HostedServiceProperties>
			//  <Deployments>
			//    <Deployment>
			//      <Name>deployment-name</Name>
			//      <DeploymentSlot>deployment-slot</DeploymentSlot>
			//      <PrivateID>deployment-id</PrivateID>
			//      <Status>deployment-status</Status>
			//      <Label>base64-encoded-deployment-label</Label>
			//      <Url>deployment-url</Url>
			//      <Configuration>base-64-encoded-configuration-file</Configuration>
			//      <RoleInstanceList>
			//        <RoleInstance>
			//          <RoleName>role-name</RoleName>
			//          <InstanceName>role-instance-name</InstanceName>
			//          <InstanceStatus>instance-status</InstanceStatus>
			//        </RoleInstance>
			//      </RoleInstanceList>
			//      <UpgradeDomainCount>upgrade-domain-count</UpgradeDomainCount>
			//      <RoleList>
			//        <Role>
			//          <RoleName>role-name</RoleName>
			//          <OsVersion>operating-system-version</OsVersion>
			//        </Role>
			//      </RoleList>
			//      <SdkVersion>sdk-version-used-to-create-package</SdkVersion>
			//      <InputEndpointList>
			//         <InputEndpoint>
			//            <RoleName>role-name</RoleName>
			//            <Vip>virtual-ip-address</Vip>
			//            <Port>port-number</Port>
			//         </InputEndpoint>
			//         …
			//      </InputEndpointList>
			//      <Locked>deployment-write-allowed-status</Locked>
			//      <RollbackAllowed>rollback-operation-allowed</RollbackAllowed>
			//    </Deployment>
			//  </Deployments>
			//</HostedService>


			XElement rootElement = XElement.Parse(xml);
			XNamespace ns = "http://schemas.microsoft.com/windowsazure";

			foreach (var deploymentElement in rootElement.Element(ns + "Deployments").Elements(ns + "Deployment"))
			{
				DeploymentInfo deployment = new DeploymentInfo()
				{
					Name = deploymentElement.Element(ns + "Name").Value
				};

				deployments.Add(deployment);
			}

			state.FinalCallback(deployments);
			return null;
		}

		#region Remove Invokation Code

		public void InvokeRequest<T>(string managementUrl,
									 byte[] certificate,
									 string certificatePassword,
									 string requestBody,
									 InvokeRequestState<T> requestState,
									 Func<object, string, object> callback)
		{
			// The request must be broken up into several parts using MIME split message
			// # Part 1 - information (URL to call, certificate password)
			// # Part 2 - the certificate to use
			// # Part 3 - the request body (if any)

			// Define the MIME content boundary
			//
			//string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
			//byte[] boundarybytes = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");

			string url = string.Format("http://{0}/Services/InvokeSignedRequest", App.ServiceHostName);
			HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
			wr.Method = "POST";
			//wr.ContentType = "multipart/form-data; boundary=" + boundary;

			var state = new
			{
				Request = wr,
				//Boundary = boundary,
				ManagementUrl = managementUrl,
				CertificatePassword = certificatePassword,
				Certificate = certificate,
				RequestBody = requestBody,
				Callback = callback,
				RequestState = requestState
			};

			wr.BeginGetRequestStream(BeginGetRequestStreamCallback, state);
		}

		private void BeginGetRequestStreamCallback(IAsyncResult asynchronousResult)
		{
			Type stateType = asynchronousResult.AsyncState.GetType();
			object state = asynchronousResult.AsyncState;

			// Get all the variables we need here.
			//
			HttpWebRequest request = stateType.GetProperty("Request").GetValue(state, null) as HttpWebRequest;
			//string boundary = stateType.GetProperty("Boundary").GetValue(state, null) as string;
			string certificatePassword = stateType.GetProperty("CertificatePassword").GetValue(state, null) as string;
			string managementUrl = stateType.GetProperty("ManagementUrl").GetValue(state, null) as string;
			byte[] certificate = stateType.GetProperty("Certificate").GetValue(state, null) as byte[];

			// Open the request stream.
			//
			Stream requestStream = request.EndGetRequestStream(asynchronousResult);

			// Turn the boundayr into bytes.
			//
			//byte[] boundarybytes = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");

			// Add the variables in their own boundary block.
			//
			//requestStream.Write(boundarybytes, 0, boundarybytes.Length);

			string requestData = string.Format("managementUrl={0}&certificatePassword={1}", managementUrl, certificatePassword);
			byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(HttpUtility.UrlEncode(requestData));
			requestStream.Write(dataBytes, 0, dataBytes.Length);

			byte[] breakPoint = System.Text.Encoding.UTF8.GetBytes("\r\n\r\n\r\n");

			requestStream.Write(breakPoint, 0, breakPoint.Length);

			//requestStream.Write(boundarybytes, 0, boundarybytes.Length);

			// Output the certificate
			//
			requestStream.Write(certificate, 0, certificate.Length);

			// Add the trailing boundary.
			//byte[] trailer = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
			//requestStream.Write(trailer, 0, trailer.Length);
			requestStream.Close();

			request.BeginGetResponse(BeginGetResponseCallback, state);
		}

		private void BeginGetResponseCallback(IAsyncResult asynchronousResult)
		{
			Type stateType = asynchronousResult.AsyncState.GetType();
			object state = asynchronousResult.AsyncState;

			HttpWebRequest request = stateType.GetProperty("Request").GetValue(state, null) as HttpWebRequest;
			HttpWebResponse response = null;
			Stream responseStream = null;

			try
			{
				response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
				responseStream = response.GetResponseStream();
			}
			catch (WebException exp)
			{
				responseStream = exp.Response.GetResponseStream();
				StreamReader sr = new StreamReader(responseStream);
				string returnedData = sr.ReadToEnd();
				throw;
			}

			string xmlServerResponse = null;

			using (StreamReader sr = new StreamReader(responseStream))
			{
				xmlServerResponse = sr.ReadToEnd();
			}

			responseStream.Close();

			// Invoke the callback!
			//
			Func<object, string, object> callback = stateType.GetProperty("Callback").GetValue(state, null) as Func<object, string, object>;
			object invokeRequestState = stateType.GetProperty("RequestState").GetValue(state, null);

			callback.Invoke(invokeRequestState, xmlServerResponse);
		}

		private void DownloadAndStoreCertificateCallback(IAsyncResult asynchronousResult)
		{
			var state = asynchronousResult.AsyncState;
			HttpWebRequest myHttpWebRequest2 = (HttpWebRequest)state.GetType().GetProperty("Request").GetValue(this, null);

			HttpWebResponse resp = (HttpWebResponse)myHttpWebRequest2.EndGetResponse(asynchronousResult);

			//using (BinaryReader r = new BinaryReader(resp.GetResponseStream()))
			//{
			//    certificateData = r.ReadBytes((int)resp.ContentLength);
			//}

			//currentDispatcher.BeginInvoke(() =>
			//{
			//    CertificateStatus = "downloaded successfully";
			//    RetrieveSubscriptionName();
			//});

			//return null;
		}

		#endregion
	}
}
