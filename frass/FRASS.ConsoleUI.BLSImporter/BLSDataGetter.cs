using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using FRASS.DAL;
using FRASS.DAL.DataManager;

namespace FRASS.ConsoleUI.BLSImporter
{
	public class BLSDataGetter
	{
		public static BLSDataGetter GetInstance()
		{
			return new BLSDataGetter();
		}
		private BLSDataGetter()
		{
			
		}

		public void DownloadFile(string saveFileName, string URL, string seriesID, int marketModelTypeID)
		{
			var request = (HttpWebRequest)WebRequest.Create(URL);
			request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36 Edg/114.0.1823.43";
			request.Referer = "https://download.bls.gov/pub/time.series/pc/";
			request.UseDefaultCredentials = false;
			request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7";
            var response = (HttpWebResponse)request.GetResponse();
			try
			{
				var encoding = Encoding.GetEncoding(response.CharacterSet);
				var reader = new StreamReader(response.GetResponseStream(), encoding);
				using (var sw = new StreamWriter(saveFileName, false, encoding))
				{
					sw.Write(reader.ReadToEnd());
					sw.Flush();
					sw.Close();
				}
			}
			finally
			{
				response.Close();
			}

			ParseBLSFile(saveFileName, seriesID, marketModelTypeID);
		}
		private void ParseBLSFile(string fileName, string seriesID, int marketModelTypeID)
		{
			var list = new List<MarketModelData>();
			using (StreamReader sr = new StreamReader(fileName))
			{
				var line = string.Empty;
				while ((line = sr.ReadLine()) != null)
				{
					char[] delimiters = new char[] { '\t' };
					string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
					if (parts[0].Trim() == seriesID)
					{
						var marketModelData = new MarketModelData();
						marketModelData.SeriesID = parts[0].Trim().ToString();
						marketModelData.Year = Convert.ToInt32(parts[1].Trim());
						marketModelData.Period = Convert.ToInt32(parts[2].Trim().Replace("M0", "").Replace("M", ""));
						marketModelData.Value = Convert.ToDecimal(parts[3].Trim());
						marketModelData.MarketModelTypeID = marketModelTypeID;
						list.Add(marketModelData);
					}
				}
			}
			
			InsertBLSData(list, marketModelTypeID);
			
		}
		private void InsertBLSData(List<MarketModelData> marketModelData, int marketModelTypeID)
		{
			var repo = DeliveredLogMarketModelDataManager.GetInstance();
			repo.InsertMarketModelData(marketModelData);
		}
		public void TruncateBLSData()
		{
			var manager = DeliveredLogMarketModelDataManager.GetInstance();
			manager.TruncateMarketModelData();
		}
	}
}
