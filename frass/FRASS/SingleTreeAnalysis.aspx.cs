using FRASS.DAL.Repositories;
using FRASS.Timber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace FRASS
{
    public partial class SingleTreeAnalysis : Page
    {
        private TimberRepository _timberRepository = TimberRepository.GetInstance();
        private TreeLogMerchandizer treeLogMerchandizer = new TreeLogMerchandizer();

        private FRASS.DAL.Repositories.TreeData CurrentTreeData
        {
            get { return ViewState["CurrentTreeData"] as FRASS.DAL.Repositories.TreeData; }
            set { ViewState["CurrentTreeData"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button_Calculate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    var treeInput = new TreeInput
                    {
                        Species = DropDownList_Species.SelectedValue,
                        DBH = Convert.ToDouble(TextBox_DBH.Text),
                        Height = Convert.ToDouble(TextBox_Height.Text),
                        CR = Convert.ToDouble(TextBox_CR.Text) / 100,
                        CFV_Target = Convert.ToDouble(TextBox_CFV.Text),
                        StumpHeight = Convert.ToDouble(TextBox_StumpHeight.Text)
                    };

                    var scribnerTable = this._timberRepository.GetScribnerTable(32);

                    List<LogSegment> results = this.treeLogMerchandizer.ProcessTree(treeInput, scribnerTable, isDemo:true);

                    Literal_Results.Text = GenerateResultsTable(results);

                    double totalBF = results.Sum(r => r.BFVolume);
                    Label_FlexTaperBF.Text = "FlexTaper Board Foot Volume: " + totalBF.ToString();
                    if (CurrentTreeData != null)
                    {
                        Label_FVSBF.Text = "FVS Board Foot Volume: " + CurrentTreeData.VolPerTreeBF.ToString();
                    }
                    else
                    {
                        Label_FVSBF.Text = "FVS Board Foot Volume: N/A";
                    }

                    Literal_Diagram.Text = GenerateTreeDiagram(results, treeInput.Height);

                    // Show additional notes after calculation
                    Label_MarketDate.Text = DateTime.Now.ToString("M/d/yyyy");
                    Label_MarketDate.Visible = true;

                    Label_Error.Text = "";
                }
                catch (Exception ex)
                {
                    Label_Error.Text = "An error occurred during calculation: " + ex.Message;
                }
            }
        }

        protected void Button_LoadRandom_Click(object sender, EventArgs e)
        {
            try
            {
                var treeData = _timberRepository.GetRandomTreeFrom2025();



                if (treeData != null)
                {
                    CurrentTreeData = treeData;
                    DropDownList_Species.SelectedValue = treeData.Species.Trim();
                    TextBox_DBH.Text = treeData.DBH.ToString("F1");
                    TextBox_Height.Text = treeData.Height.ToString();
                    TextBox_CR.Text = (treeData.CR * 100).ToString(); // Convert from double percent to int 0-100
                    TextBox_CFV.Text = treeData.VolPerTreeCU.ToString();

                    // Clear previous results
                    Literal_Results.Text = "";
                    Literal_Diagram.Text = "";
                    Label_Error.Text = "";

                    // Hide additional notes until next calculation
                    Label_MarketDate.Visible = true;
                }
                else
                {
                    Label_Error.Text = "No tree data found in the database.";
                }
            }
            catch (Exception ex)
            {
                Label_Error.Text = "An error occurred while loading random tree: " + ex.Message;
            }
        }

        private string GenerateTreeDiagram(List<LogSegment> segments, double totalHeight)
        {
            int svgWidth = 250;
            int svgHeight = 400;
            double scale = svgHeight / totalHeight;

            StringBuilder svg = new StringBuilder();
            svg.AppendFormat("<svg width='{0}' height='{1}' style='border:1px solid #ccc;'>", svgWidth, svgHeight);

            // Draw ground line
            svg.AppendFormat("<line x1='0' y1='{0}' x2='{1}' y2='{0}' stroke='black' stroke-width='2' />", svgHeight, svgWidth);

            // Draw tree trunk
            double trunkX = svgWidth / 2;
            svg.AppendFormat("<line x1='{0}' y1='{1}' x2='{0}' y2='0' stroke='#8B4513' stroke-width='4' />", trunkX, svgHeight);

            // Add height scale
            double[] fractions = { 0, 0.25, 0.5, 0.75, 1.0 };
            foreach (double frac in fractions)
            {
                double heightVal = frac * totalHeight;
                double y;
                if (frac == 0)
                {
                    y = svgHeight - 10;
                }
                else if (frac == 1.0)
                {
                    y = 13; // Position top label to avoid clipping
                }
                else
                {
                    y = svgHeight - (heightVal * scale) + 3;
                }
                string label = string.Format("{0:F1} ft", heightVal);
                svg.AppendFormat("<text x='10' y='{0}' font-size='10' fill='black' text-anchor='start'>{1}</text>", y, label);
            }

            foreach (var segment in segments)
            {
                double y1 = svgHeight - (segment.BaseHeight * scale);
                double y2 = svgHeight - ((segment.SEDHeight - 1.5) * scale);
                double height = y1 - y2;

                string color = GetGradeColor(segment.Grade);

                // Draw segment rectangle
                svg.AppendFormat("<rect x='{0}' y='{1}' width='60' height='{2}' fill='{3}' stroke='black' stroke-width='1' />",
                    trunkX - 30, y2, height, color);

                // Add label
                string label = string.Format("{0} ft ({1})", segment.length, segment.Grade);
                svg.AppendFormat("<text x='{0}' y='{1}' font-size='10' text-anchor='middle' fill='white'>{2}</text>",
                    trunkX, y2 + height / 2 + 3, label);
            }

            svg.Append("</svg>");
            return svg.ToString();
        }

        private string GetGradeColor(string grade)
        {
            if (grade.StartsWith("P"))
            {
                return "#6e3701"; // Brown
            }

            // Extract the number from sort codes like D1, H2, C3, B4
            if (grade.Length >= 2 && char.IsLetter(grade[0]) && char.IsDigit(grade[1]))
            {
                char gradeLetter = grade[0];
                int gradeNumber;
                if (int.TryParse(grade[1].ToString(), out gradeNumber))
                {
                    switch (gradeNumber)
                    {
                        case 1: return "#013000"; // Dark Green (high quality)
                        case 2: return "#045702"; // Green (good quality)
                        case 3: return "#038001"; // Lighter Green (medium quality)
                        case 4: return "#059c02"; // Slightly Lighter Green (lower quality)
                        case 5: return "#EF4444"; // Red (lowest quality)
                        default: return "#6B7280"; // Neutral gray
                    }
                }
            }

            // Default color for unrecognized grades
            return "#6B7280"; // Neutral gray
        }

        private string GenerateResultsTable(List<LogSegment> results)
        {
            if (results == null || results.Count == 0)
            {
                return "<p>No results to display.</p>";
            }

            StringBuilder table = new StringBuilder();
            table.Append("<table style='border-collapse: collapse; width: 100%; border: 1px solid #ddd;'>");
            table.Append("<thead>");
            table.Append("<tr style='background-color: #f2f2f2;'>");
            table.Append("<th style='border: 1px solid #ddd; padding: 8px; text-align: center;'>Base Height (ft)</th>");
            table.Append("<th style='border: 1px solid #ddd; padding: 8px; text-align: center;'>SED (in)</th>");
            table.Append("<th style='border: 1px solid #ddd; padding: 8px; text-align: center;'>Grade</th>");
            table.Append("<th style='border: 1px solid #ddd; padding: 8px; text-align: center;'>Scribner Board Foot Volume (Decimal C reporting)</th>");
            table.Append("<th style='border: 1px solid #ddd; padding: 8px; text-align: center;'>Log Length (ft)</th>");
            table.Append("<th style='border: 1px solid #ddd; padding: 8px; text-align: center;'>SED Height (ft)</th>");
            table.Append("<th style='border: 1px solid #ddd; padding: 8px; text-align: center;'>Estimated Value $</th>");
            table.Append("</tr>");
            table.Append("</thead>");
            table.Append("<tbody>");

            bool alternate = false;
            foreach (var segment in results)
            {
                string rowStyle = alternate ? "background-color: #f9f9f9;" : "";
                table.AppendFormat("<tr style='{0}'>", rowStyle);
                table.AppendFormat("<td style='border: 1px solid #ddd; text-align: center; padding: 8px;'>{0}</td>", segment.BaseHeight);
                table.AppendFormat("<td style='border: 1px solid #ddd; text-align: center; padding: 8px;'>{0:F2}</td>", segment.SED);
                table.AppendFormat("<td style='border: 1px solid #ddd; text-align: center; padding: 8px;'>{0}</td>", segment.Grade);
                table.AppendFormat("<td style='border: 1px solid #ddd; text-align: center; padding: 8px;'>{0}</td>", segment.BFVolume);
                table.AppendFormat("<td style='border: 1px solid #ddd; text-align: center; padding: 8px;'>{0}</td>", segment.length);
                table.AppendFormat("<td style='border: 1px solid #ddd; text-align: center; padding: 8px;'>{0:F2}</td>", segment.SEDHeight.ToString("F1"));
                table.AppendFormat("<td style='border: 1px solid #ddd; text-align: center; padding: 8px;'>{0}</td>", segment.EstimatedValue?.ToString("N2"));
                table.Append("</tr>");
                alternate = !alternate;
            }

            table.Append("</tbody>");
            table.Append("</table>");
            return table.ToString();
        }
    }
}
