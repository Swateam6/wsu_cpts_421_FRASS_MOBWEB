using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace MOBWEB_TEST.Models;
using System.Collections.ObjectModel;
public class Plot
{
    public int PlotNumber { get; set; }

    // --- Mesic Species Subsystem ---

    // Most Mesic Tree
    public string? PrimaryMesicTree { get; set; }

    // Next most seeking Mesic Tree
    public string? SecondaryMesicTree { get; set; }

    // Bush Species
    public string? DominantBushSpecies { get; set; }

    // Most mesic bush
    public string? PrimaryMesicBush { get; set; }

    // Next mesic bush
    public string? SecondaryMesicBush { get; set; }

    // -------------------------------

    public double Slope { get; set; }

    // Direction in degrees (0-360)
    public double Aspect { get; set; }

    // Navigation Data
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // The individual tree measurements
    public ObservableCollection<Tree> TreeList { get; set; } = new();

    public double limitingDBH { get; set; }
}