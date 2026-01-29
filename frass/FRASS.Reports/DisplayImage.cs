using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using FRASS.DAL;
using FRASS.DAL.DataManager;

namespace FRASS.Reports
{
	public class DisplayImage
	{
		public string FileName { get; set; }
		public string PathToRoot { get; set; }
		public string FullPath { get; set; }
		public string Extention { get; set; }

		private DisplayImage(string fileName, string filePath, string urlPath, string extention)
		{
			FileName = fileName;
			PathToRoot = filePath;
			FullPath = urlPath;
			Extention = extention;
		}

		private static List<DisplayImage> GetDisplayImage(Parcel parcel, List<ParcelRiparian> stands, string mapsImagePath, string photosImagePath)
		{
			
			var images = new List<DisplayImage>();
			if (stands != null)
			{
				images.AddRange(GetMaps(parcel, stands, mapsImagePath));
				images.AddRange(GetPhotos(parcel, stands, photosImagePath));
			}
			return GetDistinctImages(images);
		}

		private static List<DisplayImage> GetMaps(Parcel parcel, List<ParcelRiparian> stands, string mapsImagePath)
		{
			var images = new List<DisplayImage>();
			var files = Directory.GetFiles(mapsImagePath, "*.png");
			var pos = parcel.ParcelAllotments;
			foreach (var file in files.OrderBy(f => f))
			{
				var fileName = file.Replace(mapsImagePath, "");
				DisplayImage dm = new DisplayImage(fileName, "/Images/Maps/" + fileName, mapsImagePath + fileName, "png");
				foreach (var po in pos)
				{
					string section = "";
					if (po.Section.Trim().Length == 1)
					{
						section = "0";
					}
					section = section + po.Section.Trim();
					string range = "";
					if (po.Range.Trim().Length == 2)
					{
						range = "0";
					}
					range = range + po.Range.Trim();
					var fname = "T" + po.Town.Trim() + "R" + range + "S" + section;
					if (dm.FileName.StartsWith(fname))
					{
						images.Add(dm);
					}
				}
			}
			return images;
		}
		private static List<DisplayImage> GetPhotos(Parcel parcel, List<ParcelRiparian> stands, string photosImagePath)
		{
			var images = new List<DisplayImage>();
			var files = Directory.GetFiles(photosImagePath, "*.jpg");
			foreach (var file in files.OrderBy(f => f))
			{
				var fileName = file.Replace(photosImagePath, "");
				string filePath = "/Images/Photos/" + fileName;
				string urlPath = photosImagePath + fileName;
                DisplayImage dm = new DisplayImage(fileName, filePath, urlPath, "jpg");
				foreach (var stand in stands.Distinct())
				{
					if (stand != null && stand.TimberCurrentStand != null)
					{
						var fname = stand.TimberCurrentStand.Veg_Label + "_SI" + stand.TimberCurrentStand.Yield_Index;
						if (dm.FileName.StartsWith(fname))
						{
							images.Add(dm);
						}
					}
				}
			}
			return images;
		}
		private static List<DisplayImage> GetDistinctImages(List<DisplayImage> images)
		{
			var maps2 = new List<DisplayImage>();
			foreach (var map in images)
			{
				var m = (from ma in maps2 where ma.FileName == map.FileName select ma).FirstOrDefault();
				if (m == null)
				{
					maps2.Add(map);
				}
			}
			return maps2;
		}

		public static List<DisplayImage> GetMapImagesByFullPaths(Parcel parcel, List<ParcelRiparian> stands, string mapsImagePath, string photosImagePath)
		{
			
			return GetDisplayImage(parcel, stands, mapsImagePath, photosImagePath);
		}

		public static List<DisplayImage> GetIndexImagesByFullPaths(Parcel parcel, string indexImagePath)
		{
			var images = new List<DisplayImage>();
			var db = ParcelDataManager.GetInstance();
			var fp = indexImagePath + "\\Index_Maps\\" + "FRASS_Overview.png";
			var image = new DisplayImage("FRASS_Overview.png", fp, fp, "png");
			images.Add(image);
			//foreach(var allotment in parcel.ParcelAllotments)
			//{
			//	var town = allotment.Town;
			//	var range = allotment.Range;
			//	var section = allotment.Section;
			//	var imageNumber = db.GetIndexNumber(GetNumber(town), GetNumber(range), GetNumber(section));
			//	var fileName = "INDEX_" + imageNumber + ".png";
			//	if (imageNumber < 10)
			//	{
			//		fileName = "INDEX_0" + imageNumber + ".png";
			//	}
			//	DisplayImage dm = new DisplayImage(fileName, indexImagePath + "\\Index_Maps\\" + fileName, indexImagePath + "\\Index_Maps\\" + fileName, "png");
			//	images.Add(dm);
			//}
			return GetDistinctImages(images);
		}
		private static decimal GetNumber(string str)
		{
			var result = str.Replace("N","");
			result = result.Replace("S", "");
			result = result.Replace("E", "");
			result = result.Replace("W", "");
			return Convert.ToDecimal(result);
		}
	}
}