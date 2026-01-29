using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.Interfaces;
using FRASS.DAL.DataManager;
using FRASS.DAL;

namespace FRASS.BLL.Models
{
	public class HarvestStumpageGroup
	{
		public StumpageGroup StumpageGroup { get; set; }
		public int QualityCodeNumber { get; set; }
		public int ValueType { get; set; }
		public decimal ValueAtHarvest_R1 { get; set; }
		public decimal ValueMBF_R1 { get; set; }
		public decimal ValueSort_R1 { get; set; }
		public decimal ValueAtHarvest_R2 { get; set; }
		public decimal ValueAtHarvest_R3 { get; set; }
	}
}
