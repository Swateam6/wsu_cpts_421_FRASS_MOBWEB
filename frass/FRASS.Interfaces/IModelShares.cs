using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRASS.Interfaces
{
	public interface IModelShare
	{
		int ModelID { get; }
		string PortfolioName { get; }
		DateTime LastEdited { get; }
		string Creator { get; }
		string Editor { get; }
	}
}
