using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AzureManager.Data;
using System.ServiceModel.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using AzureManager.Common.Data;
using AzureManager.Web.Models;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Diagnostics;
using System.Xml.Linq;

namespace AzureManager.Web.Controllers
{
  [ServiceContract]
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class ServiceController
  {
    [WebInvoke(UriTemplate = "/RegisterDevice", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
    public RegistrationResponse RegisterDevice(string ChannelUri, short DeviceType)
    {
      // Get the body. This will be the devices channel URI
      RegistrationModel model = new RegistrationModel();
      string pinCode = model.SaveDeviceRegistration(ChannelUri);
      return new RegistrationResponse() { PinCode = pinCode };
    }

    [WebGet(UriTemplate = "/RetrieveCertificate/{devicePin}/{subscriptionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
    public Stream RetrieveCertificate(string devicePin, string subscriptionId)
    {
      DeviceRegistration registration = null;

      using (AzureManagerContext context = new AzureManagerContext())
      {
        registration = context.DeviceRegistrations.Where(dv => dv.DevicePin == devicePin && dv.SubscriptionId == subscriptionId).FirstOrDefault();
        registration.Exhausted = true;
        context.SaveChanges();
      }

      WebOperationContext.Current.OutgoingResponse.ContentType = "application/octet-stream";
      return new MemoryStream(registration.Certificate);
    }

    /// <summary>
    /// This method will receieve the requested URL in the headers and the certificate in the body.
    /// </summary>
    [WebInvoke(UriTemplate = "/InvokeSignedRequest", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare)]
    public Stream InvokeSignedRequest(Stream body)
    {
      int bodyLength = (int)HttpContext.Current.Request.ContentLength;
      byte[] incomingStream = null;

      using (BinaryReader r = new BinaryReader(body))
      {
        incomingStream = r.ReadBytes(bodyLength);
      }

      int lengthMark = 0;

      // Read until we hit the first return character...
      //
      for (int index = 0; index < incomingStream.Length; index++)
      {
        if (incomingStream[index] == System.Text.Encoding.UTF8.GetBytes("\r")[0])
        {
          // Check for the rest of the 6 characters...
          //
          lengthMark = index;
          break;
        }
      }

      // Extract the first part.
      //
      byte[] contentArray = new byte[lengthMark];

      for (int i = 0; i < lengthMark; i++)
      {
        contentArray[i] = incomingStream[i];
      }

      string message = System.Text.Encoding.UTF8.GetString(contentArray);
      message = HttpUtility.UrlDecode(message);

      var requestParameters = HttpUtility.ParseQueryString(message);

      int certificateLength = (bodyLength - lengthMark - 6);
      contentArray = new byte[certificateLength];
      int j = 0;
      for (int i = lengthMark + 6; i < bodyLength; i++)
      {
        contentArray[j++] = incomingStream[i];
      }

      X509Certificate2 certificate = null;
      string certificatePassword = requestParameters["certificatePassword"]; ;

      try
      {
        certificate = new X509Certificate2(contentArray, certificatePassword, X509KeyStorageFlags.DefaultKeySet);
      }
      catch (Exception)
      {
        throw;
      }

      // Request and response variables.
      //
      HttpWebRequest httpWebRequest = null;
      HttpWebResponse httpWebResponse = null;

      // Stream variables.
      //
      Stream responseStream = null;
      StreamReader reader = null;

      try
      {
        // URI variable.
        Uri requestUri = null;

        // The ID for the Windows Azure subscription.
        string requestedUrl = requestParameters["managementUrl"];

        // Create the request.
        requestUri = new Uri(requestedUrl);

        httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(requestUri);
        httpWebRequest.Method = "GET";
        httpWebRequest.ClientCertificates.Add(certificate);

        // Specify the version information in the header.
        httpWebRequest.Headers.Add("x-ms-version", "2011-10-01");

        try
        {
          // Make the call using the web request.
          httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        }
        catch (WebException exp)
        {
          throw;
        }

        // Display the web response status code.
        Console.WriteLine("Response status code: " + httpWebResponse.StatusCode);

        // Display the request ID returned by Windows Azure.
        if (null != httpWebResponse.Headers)
        {
          Console.WriteLine("x-ms-request-id: " + httpWebResponse.Headers["x-ms-request-id"]);
        }

        // Parse the web response.
        responseStream = httpWebResponse.GetResponseStream();

        return responseStream;
      }
      finally
      {
        if (httpWebResponse != null) httpWebResponse.Close();
        if (responseStream != null) responseStream.Close();
        if (reader != null) reader.Close();
      }
    }
  }
}
