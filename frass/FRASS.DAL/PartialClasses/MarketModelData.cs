using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Web;

namespace FRASS.DAL
{
	public partial class MarketModelData
	{
		public DateTime ReportDate
		{
			get
			{
				DateTime dt = new DateTime(Year, Period, 1);
				return dt;
			}
		}
	}
}
