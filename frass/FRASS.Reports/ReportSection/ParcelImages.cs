using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;

namespace FRASS.Reports.ReportSection
{
	internal class ParcelImages
	{
		private List<DisplayImage> Images;
		public ParcelImages(Parcel parcel, string mapsImagePath, string photosImagePath)
		{
			var stands = parcel.ParcelRiparians.Select(uu => uu).ToList<ParcelRiparian>();
			Images = DisplayImage.GetMapImagesByFullPaths(parcel, stands, mapsImagePath, photosImagePath);
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
			imageSection.Margin.Left = PageSize.LetterWidth / 2 - imageSection.ImageWidth;
			imageSection.ImageInfo.File = image.FullPath;
			imageSection.ImageInfo.Alignment = AlignmentType.Center;
			if (image.Extention == "png")
			{
				imageSection.ImageInfo.ImageFileType = ImageFileType.Png;
			}
			else if (image.Extention == "jpg")
			{
				imageSection.ImageInfo.ImageFileType = ImageFileType.Jpeg;
			}

			if (image.FullPath.Contains("Photo"))
			{
				imageSection.ImageInfo.Title = image.FileName;
			}

			var reportUtilities = new ReportUtilities();
			section.PageInfo.Margin = reportUtilities.SectionMargin;
			return section;
		}
	}
}