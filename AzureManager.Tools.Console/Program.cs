using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.IO;

namespace AzureManager.Tools.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // X.509 certificate variables.
                X509Store certStore = null;
                X509Certificate2Collection certCollection = null;
                X509Certificate2 certificate = null;

                // Request and response variables.
                HttpWebRequest httpWebRequest = null;
                HttpWebResponse httpWebResponse = null;

                // Stream variables.
                Stream responseStream = null;
                StreamReader reader = null;

                // URI variable.
                Uri requestUri = null;

                // Specify operation to use for the service management call.
                // This sample will use the operation for listing the hosted services.
                string operation = "hostedservices";

                // The ID for the Windows Azure subscription.
                string subscriptionId = "27b0e36c-5a3f-43e5-b104-1f5bdd1fa0e4";

                // The thumbprint for the certificate. This certificate would have been
                // previously added as a management certificate within the Windows Azure management portal.
                //string thumbPrint = "your_certificate_thumbprint";

                // Open the certificate store for the current user.
                //certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                //certStore.Open(OpenFlags.ReadOnly);

                //// Find the certificate with the specified thumbprint.
                //certCollection = certStore.Certificates.Find(
                //                     X509FindType.FindByThumbprint,
                //                     thumbPrint,
                //                     false);

                // Close the certificate store.
                //certStore.Close();

                // Check to see if a matching certificate was found.
                //if (0 == certCollection.Count)
                //{
                //    throw new Exception("No certificate found containing thumbprint " + thumbPrint);
                //}

                // A matching certificate was found.
                //certificate = certCollection[0];
                //Console.WriteLine("Using certificate with thumbprint: " + thumbPrint);

                //certificate = new X509Certificate2(@"C:\Users\Public\Documents\azuremanagerwithkey.pfx", "abc123");
				certificate = new X509Certificate2(@"C:\Users\Tomas\Downloads\azuremanagerwithkey.pfx", "abc123");

                string url = string.Format("{0}{1}/services/hostedservices/{2}?embed-detail=true",
                                      "https://management.core.windows.net/",
                                      subscriptionId,
                                      "caffeineclub");

                // Create the request.
                //requestUri = new Uri("https://management.core.windows.net/" + subscriptionId);
                //                     //+ "/services/"
                //                     //+ operation
                //                     //);
                requestUri = new Uri(url);

                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(requestUri);

                // Add the certificate to the request.
                httpWebRequest.ClientCertificates.Add(certificate);

                // Specify the version information in the header.
                httpWebRequest.Headers.Add("x-ms-version", "2011-10-01");

                // Make the call using the web request.
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                // Display the request ID returned by Windows Azure.
                if (null != httpWebResponse.Headers)
                {
                    System.Console.WriteLine("x-ms-request-id: " + httpWebResponse.Headers["x-ms-request-id"]);
                }

                // Parse the web response.
                responseStream = httpWebResponse.GetResponseStream();
                reader = new StreamReader(responseStream);
                // Display the raw response.
                System.Console.WriteLine("Response output:");
                string value = reader.ReadToEnd();
                System.Console.WriteLine(value);

                // Close the resources no longer needed.
                httpWebResponse.Close();
                responseStream.Close();
                reader.Close();
            }
            catch (Exception e)
            {

                System.Console.WriteLine("Error encountered: " + e.Message);

                // Exit the application with exit code 1.
                System.Environment.Exit(1);

            }
            finally
            {
                // Exit the application.
                System.Environment.Exit(0);
            }
        }
    }
}
