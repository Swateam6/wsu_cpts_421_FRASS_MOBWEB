using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MOBWEB_TEST.Models;
using System.Collections.ObjectModel;
public class Stand
{
    public string? StandId { get; set; }
    public double? Acres { get; set; }
    public string? Market { get; set; }
    public string? CruiserName { get; set; }
    public DateTime CruiseDate { get; set; }

    // Holds all the plots you cruise within this stand
    public ObservableCollection<Plot> PlotList { get; set; } = new();
}