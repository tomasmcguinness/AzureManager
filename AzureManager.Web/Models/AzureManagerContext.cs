using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace AzureManager.Web.Models
{
    public class AzureManagerContext : DbContext
    {
        public DbSet<DeviceRegistration> DeviceRegistrations { get; set; }
    }
}