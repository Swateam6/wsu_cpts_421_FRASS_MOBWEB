using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FRASS.Console.BLSImporter;

namespace FRASS.ConsoleUI.BLSImporter
{
	class Program
	{
		static void Main(string[] args)
		{
			var manager = BLSDataManager.GetInstance();
			manager.Run();
		}
	}
}
