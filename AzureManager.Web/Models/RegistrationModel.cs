using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;

namespace AzureManager.Web.Models
{
	public class RegistrationModel
	{
		private static Random r = new Random(DateTime.Now.Millisecond);

		public string SaveDeviceRegistration(string channelUri)
		{
			string pinCode = null;
			using (AzureManagerContext context = new AzureManagerContext())
			{
				pinCode = GeneratePin();
				DeviceRegistration registration = new DeviceRegistration()
				{
					ChannelUri = channelUri,
					DevicePin = pinCode,
					Registered = DateTime.Now
				};

				context.DeviceRegistrations.Add(registration);
				context.SaveChanges();
			}

			return pinCode;
		}

		public string GeneratePin()
		{
			// Pin length is 6.
			//
			StringBuilder pin = new StringBuilder();
			string pinParts = "0123456789ABCDEFGHIJKLMONPQRSTUVWXYZ";

			for (int i = 0; i < 6; i++)
			{
				int index = r.Next(pinParts.Length);
				pin.Append(pinParts[index]);
			}

			return pin.ToString();
		}

		public DeviceRegistration GetRegistration(string pin)
		{
			DeviceRegistration registration = null;

			using (AzureManagerContext context = new AzureManagerContext())
			{
				registration = context.DeviceRegistrations.Where(dr => dr.DevicePin == pin).Select(dr => dr).FirstOrDefault();
			}

			return registration;
		}

		public string NotifyDeviceOfInformation(DeviceRegistration deviceRegistration)
		{
			DeviceRegistration registration = null;

			using (AzureManagerContext context = new AzureManagerContext())
			{
				registration = context.DeviceRegistrations.Where(dr => dr.DevicePin == deviceRegistration.DevicePin).Select(dr => dr).FirstOrDefault();
			}

			if (registration == null)
			{
				return "No registration found";
			}

			try
			{
				// Use this channel id to send information
				//
				HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(registration.ChannelUri);

				// Create an HTTPWebRequest that posts the toast notification to the Microsoft Push Notification Service.
				// HTTP POST is the only method allowed to send the notification.
				sendNotificationRequest.Method = "POST";

				// The optional custom header X-MessageID uniquely identifies a notification message. 
				// If it is present, the same value is returned in the notification response. It must be a string that contains a UUID.
				sendNotificationRequest.Headers.Add("X-MessageID", Guid.NewGuid().ToString());
				string parameters = string.Format("/MainPage.xaml?mode=notification&amp;subscriptionId={0}&amp;devicePin={1}",
												  deviceRegistration.SubscriptionId,
												  deviceRegistration.DevicePin);

				// Create the toast message.
				string toastMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
				"<wp:Notification xmlns:wp=\"WPNotification\">" +
				   "<wp:Toast>" +
						"<wp:Text1>" + "Azure Manager" + "</wp:Text1>" +
						"<wp:Text2>" + "New Subscription information available." + "</wp:Text2>" +
						"<wp:Param>" + parameters + "</wp:Param>" +
				   "</wp:Toast> " +
				"</wp:Notification>";

				//+
				//       "<wp:Param>" + "1" + "</wp:Param>" +

				// Set the notification payload to send.
				byte[] notificationMessage = Encoding.Default.GetBytes(toastMessage);

				// Set the web request content length.
				sendNotificationRequest.ContentLength = notificationMessage.Length;
				sendNotificationRequest.ContentType = "text/xml";
				sendNotificationRequest.Headers.Add("X-WindowsPhone-Target", "toast");
				sendNotificationRequest.Headers.Add("X-NotificationClass", "2");

				using (Stream requestStream = sendNotificationRequest.GetRequestStream())
				{
					requestStream.Write(notificationMessage, 0, notificationMessage.Length);
					requestStream.Close();
				}

				// Send the notification and get the response.
				HttpWebResponse response = (HttpWebResponse)sendNotificationRequest.GetResponse();
				string notificationStatus = response.Headers["X-NotificationStatus"];
				string notificationChannelStatus = response.Headers["X-SubscriptionStatus"];
				string deviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"];

				// Display the response from the Microsoft Push Notification Service.  
				// Normally, error handling code would be here. In the real world, because data connections are not always available,
				// notifications may need to be throttled back if the device cannot be reached.
				string responseString = notificationStatus + " | " + deviceConnectionStatus + " | " + notificationChannelStatus;

				return responseString;
			}
			catch (Exception exp)
			{
				return exp.Message;
			}

		}
	}
}