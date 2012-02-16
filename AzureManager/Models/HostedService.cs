using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace AzureManager.Models
{
	public class HostedService
	{
		public string ServiceName { get; set; }
		public string Url { get; set; }
		public string Description { get; set; }
		public string Location { get; set; }
		public string AffinityGroup { get; set; }

		public DeploymentInfo ProductionDeployment { get; set; }
		public DeploymentInfo StagingDeployment { get; set; }
	}
}
