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

        public static Stand CurrentStand { get; set; } = new Stand();


        public static Plot CurrentPlot { get; set; } = new Plot();


        public static Tree CurrentTree { get; set; } = new Tree();


        public static void SaveTreeToPlot()
        {

            CurrentPlot.TreeList.Add(CurrentTree);


            CurrentTree = new Tree();
        }
        public static void SavePlotToStand()
        {
            
            CurrentStand.PlotList.Add(CurrentPlot);

            CurrentPlot = new Plot();
        }
    }
}
