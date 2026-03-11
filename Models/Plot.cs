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
    public double Slope { get; set; }
    public double Aspect { get; set; }

    // This is the list that actually grows as you add trees
    public ObservableCollection<Tree> TreeList { get; set; } = new();
}
