using MOBWEB_TEST.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MOBWEB_TEST.Services
{
    public static class DataService
    {
        public static Parcel CurrentParcel { get; set; } = new Parcel();
        public static Stand CurrentStand { get; set; } = new Stand();
        public static Plot CurrentPlot { get; set; } = new Plot();
        public static Tree CurrentTree { get; set; } = new Tree();
        public static Defects CurrentDefect { get; set; } = new Defects();

        /// <summary>
        /// Saves or updates CurrentTree in CurrentPlot.TreeList without resetting it to a new object.
        /// </summary>
        /// 


        public static void SetActivePlot(int plotNumber)
        {
            // Find the pre-planned plot among your 174 centers
            var existingPlot = CurrentStand.PlotList.FirstOrDefault(p => p.PlotNumber == plotNumber);

            if (existingPlot != null)
            {
                CurrentPlot = existingPlot;

                // Ensure TreeList is instantiated so trees can be added
                if (CurrentPlot.TreeList == null)
                {
                    CurrentPlot.TreeList = new ObservableCollection<Tree>();
                }
            }

            StartNewTree();
        }
        public static void SaveTreeToPlot()
        {
            if (CurrentTree.Dbh > 0 || !string.IsNullOrEmpty(CurrentTree.Species) || CurrentTree.DefectList.Count > 0)
            {
                // If it is not already in the plot list, add it
                if (!CurrentPlot.TreeList.Contains(CurrentTree))
                {
                    CurrentPlot.TreeList.Add(CurrentTree);
                }
                // If it IS already in CurrentPlot.TreeList, changes made to CurrentTree
                // are already updated by reference!
            }
            // DO NOT call CurrentTree = new Tree() here so you can keep editing it!
        }

        /// <summary>
        /// Points CurrentTree directly to an existing tree in the list for editing.
        /// </summary>
        public static void SelectTreeForEditing(Tree tree)
        {
            if (tree != null)
            {
                CurrentTree = tree;
            }
        }

        /// <summary>
        /// Selects an existing tree by its 0-based index in CurrentPlot.TreeList.
        /// </summary>
        public static void SelectTreeByIndex(int index)
        {
            if (CurrentPlot.TreeList != null && index >= 0 && index < CurrentPlot.TreeList.Count)
            {
                CurrentTree = CurrentPlot.TreeList[index];
            }
        }

        /// <summary>
        /// Explicitly resets CurrentTree only when the cruiser decides to start a brand new tree.
        /// </summary>
        public static void StartNewTree()
        {
            CurrentTree = new Tree
            {
                Id = (CurrentPlot.TreeList?.Count ?? 0) + 1
            };
        }

        public static void SavePlotToStand()
        {
            if (CurrentPlot.TreeList.Count > 0 || CurrentPlot.Slope > 0 || CurrentPlot.Aspect > 0)
            {
                if (!CurrentStand.PlotList.Contains(CurrentPlot))
                {
                    CurrentStand.PlotList.Add(CurrentPlot);
                }
            }
            // DO NOT reset CurrentPlot until moving to a brand new plot
        }

        public static void StartNewPlot()
        {
            CurrentPlot = new Plot
            {
                PlotNumber = (CurrentStand.PlotList?.Count ?? 0) + 1
            };
            StartNewTree();
        }

        public static void SaveDefectToTree()
        {
            if (!string.IsNullOrEmpty(CurrentDefect.Description) || CurrentDefect.BaseAngle != 0 || CurrentDefect.TopAngle != 0)
            {
                if (!CurrentTree.DefectList.Contains(CurrentDefect))
                {
                    CurrentTree.DefectList.Add(CurrentDefect);
                }
            }
            CurrentDefect = new Defects();
        }

        public static void SaveStandToParcel()
        {
            if (CurrentStand.PlotList.Count > 0)
            {
                if (!CurrentParcel.Stands.Contains(CurrentStand))
                {
                    CurrentParcel.Stands.Add(CurrentStand);
                }
            }
        }
    }
}