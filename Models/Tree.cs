using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBWEB_TEST.Models;

public class Tree
{
    public string? Species { get; set; }
    public double Dbh { get; set; }
    public double Height { get; set; }
    public double CR { get; set; }           // Crown Ratio
    public double StumpHeight { get; set; }
    public double CFV_Target { get; set; }   // Cubic Foot Volume Target

    // A handy string formatter to make displaying the tree in your UI easy
    public string DisplaySummary => $"{Species} - {Dbh}\" DBH, {Height}' Tall";


    public ObservableCollection<Defects> DefectList { get; set; } = new();
}