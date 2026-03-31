using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBWEB_TEST.Models
{
    public class Parcel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public double TotalAcres { get; set; }

        // Township, Range, Section (The forestry "GPS" before GPS existed)
        public string LegalDescription { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; } = DateTime.Now;

        // The "Many" in the 1-to-Many relationship
        public List<Stand> Stands { get; set; } = new List<Stand>();

        // Quick helper for your Picker or List view labels
        public string DisplayLabel => $"{Name} ({TotalAcres} ac) | {Owner}";
    }
}
