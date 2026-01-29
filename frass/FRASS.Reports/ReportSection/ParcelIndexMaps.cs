using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;

namespace FRASS.Reports.ReportSection
{
	internal class ParcelIndexMaps
	{
		private List<DisplayImage> Images;
		public ParcelIndexMaps(Parcel parcel, string imagePath)
		{
			var stands = parcel.ParcelRiparians.Select(uu => uu).ToList<ParcelRiparian>();
			Images = DisplayImage.GetIndexImagesByFullPaths(parcel, imagePath);
		}

		public List<Section> GetImageSections()
		{
			var sections = new List<Section>();
			foreach (var image in Images)
			{
				sections.Add(GetImageSection(image));
			}
			return sections;
		}
		
		public Section GetImageSection(DisplayImage image)
		{
			var section = new Section();
			var imageSection = new Image(section);
			section.Paragraphs.Add(imageSection);
			imageSection.ImageInfo.File = image.FullPath;
			imageSection.ImageScale =  1f;
			imageSection.ImageInfo.Alignment = AlignmentType.Center;
			section.IsLandscape = false;


			//imageSection.ImageInfo.Alignment = AlignmentType.Center;
			if (image.Extention == "png")
			{
				imageSection.ImageInfo.ImageFileType = ImageFileType.Png;
			}
			else if (image.Extention == "jpg")
			{
				imageSection.ImageInfo.ImageFileType = ImageFileType.Jpeg;
			}

			var reportUtilities = new ReportUtilities();
			return section;
		}
	}
}
