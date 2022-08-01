using CsvHelper;
using Discord;
using RailsIO.Commands.Slash;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailsIO.Utilities
{
    public static class CRSUtility
    {
        public static IEnumerable<StationRecord> Records { get; set; }

        public static void LoadCrsRecords()
        {

            var reader = new StreamReader(@"./Data/crs.csv");
            var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            Records = csv.GetRecords<StationRecord>().ToList();
            Console.WriteLine($"{Records.Count()} CRS records were loaded!");
        }
    }
}
