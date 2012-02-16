using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureManager.Models
{
	public class DeploymentInfo : Model
	{
		public string Name { get; set; }
		public string Slot { get; set; }
		public string Status { get; set; }
		public string Url { get; set; }
		public int RoleCount { get; set; }
		public int InstanceCount { get; set; }
		public int EndPointCount { get; set; }
		public string Size { get; set; }
	}
}
