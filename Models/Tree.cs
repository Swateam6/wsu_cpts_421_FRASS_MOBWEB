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

    public double CrownRatioPercent { get; set; }
    public double LiveCrownHeight { get; set; }

    public double StumpHeight { get; set; }
    public double CFV_Target { get; set; }

    public double BaseLiveCrown { get; set; }

    public double BaseAngle { get; set; }
    public double TopAngle { get; set; }
    public double LiveCrownBaseAngle { get; set; }

    public string DisplaySummary => $"{Species} - {Dbh}\" DBH, {Height}' Tall";

    public ObservableCollection<Defects> DefectList { get; set; } = new();
}