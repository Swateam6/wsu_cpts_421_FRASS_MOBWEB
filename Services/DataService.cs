using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOBWEB_TEST.Models;

namespace MOBWEB_TEST.Services
{
    public static class DataService
    {
        public static Parcel CurrentParcel { get; set; } = new Parcel();
        public static Stand CurrentStand { get; set; } = new Stand();
        public static Plot CurrentPlot { get; set; } = new Plot();
        public static Tree CurrentTree { get; set; } = new Tree();
        public static Defects CurrentDefect { get; set; } = new Defects();

        public static void SaveTreeToPlot()
        {
            // ANTI-GHOST GATE: Only save if the cruiser actually measured DBH or logged a species/defect
            if (CurrentTree.Dbh > 0 || !string.IsNullOrEmpty(CurrentTree.Species) || CurrentTree.DefectList.Count > 0)
            {
                CurrentPlot.TreeList.Add(CurrentTree);
            }
            CurrentTree = new Tree();
        }

        public static void SavePlotToStand()
        {
            // ANTI-GHOST GATE: Only save if the plot actually has trees in it (or environmental data)
            if (CurrentPlot.TreeList.Count > 0 || CurrentPlot.Slope > 0 || CurrentPlot.Aspect > 0)
            {
                CurrentStand.PlotList.Add(CurrentPlot);
            }
            CurrentPlot = new Plot();
        }

        public static void SaveDefectToTree()
        {
            // ANTI-GHOST GATE: Only save if the defect actually has angles or a description
            if (!string.IsNullOrEmpty(CurrentDefect.Description) || CurrentDefect.BaseAngle != 0 || CurrentDefect.TopAngle != 0)
            {
                CurrentTree.DefectList.Add(CurrentDefect);
            }
            CurrentDefect = new Defects();
        }

        public static void SaveStandToParcel()
        {
            if (CurrentStand.PlotList.Count > 0)
            {
                CurrentParcel.Stands.Add(CurrentStand);
            }
            CurrentStand = new Stand();
        }
    }
}