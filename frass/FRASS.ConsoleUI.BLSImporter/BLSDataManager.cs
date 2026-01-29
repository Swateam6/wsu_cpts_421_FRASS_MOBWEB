using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FRASS.ConsoleUI.BLSImporter;
using FRASS.BLL.Mail;
using System.Net;

namespace FRASS.Console.BLSImporter
{
	public class BLSDataManager
	{
		private BLSDataManager()
		{
			
		}
		public static BLSDataManager GetInstance()
		{
			return new BLSDataManager();
		}

		private BLSDataGetter Getter;
		public void Run()
		{
			ServicePointManager.Expect100Continue = true; 
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            
            Getter = BLSDataGetter.GetInstance();
			try
			{
				Getter.TruncateBLSData();
			}
			catch (Exception ex)
			{
				SendError("Truncate: " + ex.Message);
				throw ex;
			}
			
			RunLPI(); 
			RunCPI();
			RunPPI();
			SendNotice();
		}

		private void RunLPI()
		{
			try
			{
				var url = "https://download.bls.gov/pub/time.series/pc/pc.data.05.ForestryandLogging";
				var seriesID = "PCU113310113310";
				var marketModelTypeID = 1;
				var fileName = "c:\\temp\\LPI.txt";
				Getter.DownloadFile(fileName, url, seriesID, marketModelTypeID);
			}
			catch (Exception ex)
			{
				SendError("RunLPI: " + ex.Message);
				//throw ex;
			}
		}
		private void RunCPI()
		{
			try
			{
				var url = "https://download.bls.gov/pub/time.series/cu/cu.data.2.Summaries";
				var seriesID = "CUUR0000SA0";
				var marketModelTypeID = 2;
				var fileName = "c:\\temp\\CPI.txt";
				Getter.DownloadFile(fileName, url, seriesID, marketModelTypeID);
			}
			catch (Exception ex)
			{
				SendError("RunCPI: " + ex.Message);
				//throw ex;
			}
			
		}
		private void RunPPI()
		{
			try
			{
				var url = "https://download.bls.gov/pub/time.series/wp/wp.data.1.AllCommodities";
				var seriesID = "WPU00000000";
				var marketModelTypeID = 3;
				var fileName = "c:\\temp\\PPI.txt";
				Getter.DownloadFile(fileName, url, seriesID, marketModelTypeID);
			}
			catch (Exception ex)
			{
				SendError("RunPPI: " + ex.Message);
				//throw ex;
			}
			
		}
		private void SendNotice()
		{
			var mailMan = new MailMan();
			mailMan.SendBLSUpdatedEmail();
		}

		private void SendError(string message)
		{
			MailMan.SendErrorEmail(message);
		}
	}
}
