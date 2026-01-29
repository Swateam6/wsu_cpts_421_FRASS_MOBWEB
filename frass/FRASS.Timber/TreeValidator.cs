
using System;
using System.Collections.Generic;
using System.Linq;
namespace FRASS.Timber
{
    namespace FlexCruiser.QA
    {
        public class TreeRecord
        {
            public string Species;
            public double DBH;
            public double TotalHeight;
            public double CrownRatio;
            public double StumpHeight;
            public List<double> DOB_Values;
            public List<double> DOB_Heights;
        }

        public class ValidationResult
        {
            public bool IsValid = true;
            public List<string> Messages = new List<string>();
        }

        public static class TreeValidator
        {
            private static readonly HashSet<string> ValidSpecies = new HashSet<string>() { "DF", "WH", "RA", "BM", "RC", "WI", "CH" };

            public static ValidationResult ValidateTreeRecord(TreeRecord tree)
            {
                var result = new ValidationResult();

                if (!ValidSpecies.Contains(tree.Species))
                {
                    result.IsValid = false;
                    result.Messages.Add("Species not recognized.");
                }

                if (tree.DBH < 1 || tree.DBH > 99)
                {
                    result.IsValid = false;
                    result.Messages.Add("DBH must be between 1 and 99 inches.");
                }

                if (tree.TotalHeight < 5 || tree.TotalHeight > 300)
                {
                    result.IsValid = false;
                    result.Messages.Add("Tree height must be between 5 and 300 feet.");
                }

                if (tree.CrownRatio < 0.05 || tree.CrownRatio > 0.95)
                {
                    result.IsValid = false;
                    result.Messages.Add("Crown Ratio must be between 0.05 and 0.95.");
                }

                if (tree.StumpHeight < 0 || tree.StumpHeight > 2.5)
                {
                    result.IsValid = false;
                    result.Messages.Add("Stump height should be between 0 and 2.5 feet.");
                }

                if (tree.DOB_Values != null && tree.DOB_Heights != null)
                {
                    if (tree.DOB_Values.Count != tree.DOB_Heights.Count)
                    {
                        result.IsValid = false;
                        result.Messages.Add("Mismatch in number of DOB values and their heights.");
                    }
                    else
                    {
                        for (int i = 0; i < tree.DOB_Values.Count; i++)
                        {
                            double dob = tree.DOB_Values[i];
                            double h = tree.DOB_Heights[i];

                            if (dob <= 0 || dob >= tree.DBH)
                            {
                                result.IsValid = false;
                                result.Messages.Add($"DOB at position {i + 1} must be >0 and <DBH.");
                            }

                            if (h < tree.StumpHeight || h > tree.TotalHeight)
                            {
                                result.IsValid = false;
                                result.Messages.Add($"DOB height {h} ft must be above stump and ≤ total height.");
                            }
                        }
                    }
                }

                return result;
            }
        }
    }
}
