using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using AzureManager.Web.Models;

namespace AzureManager.Helpers
{
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<AzureManagerContext>
    {
        //public new void InitializeDatabase(WAMSContext context)
        //{
        //    base.InitializeDatabase(context);

        //    bool dbExists = context.Database.Exists();

        //    if (dbExists)
        //    {
        //        context.Database.ExecuteSqlCommand(@"EXEC sp_addrolemember 'db_datareader', 'NT AUTHORITY\NETWORK SERVICE'");
        //        context.Database.ExecuteSqlCommand(@"EXEC sp_addrolemember 'db_datawriter', 'NT AUTHORITY\NETWORK SERVICE'");
        //        context.SaveChanges();
        //    }
        //}

        //protected override void Seed(WAMSContext context)
        //{
        //    var lines = new List<TransportLine>
        //    {
        //        new TransportLine
        //        {
        //            Order = 1,
        //            LineId = 1,
        //            Name = "Bakerloo",
        //            Color = "#996633",
        //            LineCode = "BAKERLOO"
        //        },
        //        new TransportLine
        //        {
        //            Order = 2,
        //            LineId = 2,
        //            Name = "Central",
        //            Color = "#CC3333",
        //            LineCode = "CENTRAL"
        //        },
        //        new TransportLine
        //        {
        //            Order = 2,
        //            LineId = 3,
        //            Name = " Victoria",
        //            Color = "#0099CC",
        //            LineCode = "VICTORIA"
        //        },
        //        new TransportLine
        //        {
        //            LineId = 4,
        //            Order = 4,
        //            Name = "Jubilee",
        //            Color = "#868F98",
        //            LineCode = "JUBILEE"
        //        },
        //        new TransportLine
        //        {
        //            Order = 5,
        //            LineId = 5,
        //            Name = " Northern",
        //            Color = "#000000",
        //            LineCode = "NORTHERN"
        //        },
        //         new TransportLine
        //        {
        //            Order = 6,
        //            LineId = 6,
        //            Name = " Piccadilly",
        //            Color = "#003399",
        //            LineCode = "PICCADILLY"
        //        },
        //        new TransportLine
        //        {
        //            Order = 7,
        //            LineId = 7,
        //            Name = "Circle",
        //            Color = "#FFCC00",
        //            LineCode = "CIRCLE"
        //        },
        //        new TransportLine
        //        {
        //            Order = 8,
        //            LineId = 8,
        //            Name = "Hammersmith & City",
        //            Color = "#CC9999",
        //            LineCode = "HANDC"
        //        },
        //        new TransportLine
        //        {
        //            Order = 9,
        //            LineId = 9,
        //            Name = "District",
        //            Color = "#006633",
        //            LineCode = "DISTRICT"
        //        },
        //        new TransportLine
        //        {
        //            Order = 10,
        //            LineId = 11,
        //            Name = "Metropolitian",
        //            Color = "#660066",
        //            LineCode = "METROPOLITIAN"
        //        },
        //         new TransportLine
        //        {
        //            Order = 11,
        //            LineId = 12,
        //            Name = "Waterlook & City",
        //            Color = "#66CCCC",
        //            LineCode = "METROPOLITIAN"
        //        },
        //          new TransportLine
        //        {
        //            Order = 12,
        //            LineId = 81,
        //            Name = "DLR",
        //            Color = "#009999",
        //            LineCode = "OVERGROUND1"
        //        },
        //         new TransportLine
        //        {
        //            Order = 13,
        //            LineId = 82,
        //            Name = "Overground",
        //            Color = "#FF6600",
        //            LineCode = "OVERGROUND1"
        //        }
        //    };

        //    lines.ForEach(tl => context.TransportLines.Add(tl));
        //}
    }
}