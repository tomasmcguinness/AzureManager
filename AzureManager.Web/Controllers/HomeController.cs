using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AzureManager.Web.Models;
using System.IO;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace AzureManager.Web.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			RegistrationCheck check = new RegistrationCheck();
			return View(check);
		}

		[HttpPost]
		public ActionResult Index(RegistrationCheck check)
		{
			RegistrationModel model = new RegistrationModel();
			string pinCode = GetPin(check);
			DeviceRegistration deviceRegistration = model.GetRegistration(GetPin(check));

			if (deviceRegistration != null)
			{
				check.ChannelUri = deviceRegistration.ChannelUri;
			}

			return RedirectToAction("Information", new { pinCode = pinCode });
		}

		private string GetPin(RegistrationCheck check)
		{
			StringBuilder pin = new StringBuilder();
			pin.Append(check.PinDigit1);
			pin.Append(check.PinDigit2);
			pin.Append(check.PinDigit3);
			pin.Append(check.PinDigit4);
			pin.Append(check.PinDigit5);
			pin.Append(check.PinDigit6);
			return pin.ToString();
		}

		public ActionResult Information(string pinCode)
		{
			DeviceRegistration registration = null;

			using (AzureManagerContext context = new AzureManagerContext())
			{
				registration = context.DeviceRegistrations.Where(dv => dv.DevicePin == pinCode).FirstOrDefault();

				if (registration == null)
				{
					return View("NoMatch");
				}

				registration.SubscriptionId = "27b0e36c-5a3f-43e5-b104-1f5bdd1fa0e4";
			}

			return View(registration);
		}

		[HttpPost]
		public ActionResult Information(DeviceRegistration registration)
		{
			if (ModelState.IsValid)
			{
				using (AzureManagerContext context = new AzureManagerContext())
				{
					DeviceRegistration deviceRegistration = context.DeviceRegistrations.Find(registration.Id);

					if (HttpContext.Request.Files.Count > 0)
					{
						using (BinaryReader r = new BinaryReader(HttpContext.Request.Files[0].InputStream))
						{
							byte[] buffer = new byte[HttpContext.Request.ContentLength];
							r.Read(buffer, 0, HttpContext.Request.ContentLength);
							deviceRegistration.Certificate = buffer;
						}
					}

					deviceRegistration.SubscriptionId = registration.SubscriptionId;
					context.SaveChanges();

					RegistrationModel model = new RegistrationModel();
					String response = model.NotifyDeviceOfInformation(deviceRegistration);

					ViewBag.Response = response;
				}

				return RedirectToAction("InformationSent");
			}

			return View(registration);
		}

		public ActionResult InformationSent()
		{
			return View();
		}
	}
}
