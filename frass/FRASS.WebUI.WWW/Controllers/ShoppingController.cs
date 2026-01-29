using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Aspose.Pdf.Generator;
using FRASS.WebUI.WWW.BLL;
using FRASS.WebUI.WWW.DAL;

namespace FRASS.WebUI.WWW.Controllers
{
	public class ShoppingController : Controller
	{
		//
		// GET: /Shopping/

		public ActionResult TextBook()
		{
			return View();
		}

		public string GetSalesTax(string street, string city, string zip)
		{
			return RetrieveSalesTaxRate(street, city, zip).ToString();
		}

		private decimal RetrieveSalesTaxRate(string street, string city, string zip)
		{
			var wc = new WebClient();
			var urlPrefix = "http://dor.wa.gov/AddressRates.aspx?output=text";
			var uri = urlPrefix + "&addr=" + Server.UrlEncode(street) + "&city=" + Server.UrlEncode(city) + "&zip=" + Server.UrlEncode(zip);
			var reader = new StreamReader(wc.OpenRead(uri));
			var xml = reader.ReadToEnd();
			reader.Close();
			var results = xml.Split(' ');
			var rate = 0M;
			foreach (var r in results)
			{
				var t = r.ToLower();
				if (t.Contains("rate"))
				{
					if (Decimal.TryParse(t.Replace("rate=", "").Trim(), out rate))
					{
						if (rate <= 0)
						{
							return 0M;
						}
					}
				}
			}
			return rate;
		}

		public JsonResult GetOrderForm(string name, string street, string city, string state, string zip, int countryID, string email, int numberOfCopies, bool isTaxExempt, ShippingTypes shippingOption)
		{
			return Json(GeneratePDF(name, street, city, state, zip, countryID, email, numberOfCopies, isTaxExempt, shippingOption), JsonRequestBehavior.DenyGet);
		}

		private string GeneratePDF(string name, string street, string city, string state, string zip, int countryID, string email, int numberOfCopies, bool isTaxExempt, ShippingTypes shippingOption)
		{
			var stream = new MemoryStream();
			var pdf = CreateOrderForm(name, street, city, state, zip, countryID, email, numberOfCopies, isTaxExempt, shippingOption);
			var orderID = DataManager.GetInstance().SaveOrder(name, street, city, state, zip, countryID, email, numberOfCopies, isTaxExempt, shippingOption);
			var path = ConfigurationManager.AppSettings.Get("SavePDFPath");
			Aspose.Pdf.License license = new Aspose.Pdf.License();
			license.Embedded = true;
			license.SetLicense("FRASS.WebUI.WWW.Aspose.Pdf.lic");
			license.Embedded = true;

			pdf.Save(stream);
			var filePath = path + orderID.ToString() + ".pdf";
			FileInfo fi = new FileInfo(filePath);
			if (fi.Exists)
			{
				fi.Delete();
			}

			var fileStrem = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
			fileStrem.Write(stream.GetBuffer(), 0, Convert.ToInt32(stream.Length));
			fileStrem.Close();

			try
			{
				var mailMan = new MailMain();
				var mail = new MailMessage();
				var item = new System.Net.Mail.Attachment(filePath);
				mail.Attachments.Add(item);
				mail.To.Add("Schlosser@Resource-Analysis.com");
				mail.To.Add("Doroshenk@Resource-Analysis.com");
				mailMan.SendMail(mail);
			}
			catch (Exception ex) { }

			return orderID.ToString() + ".pdf";

		}

		private Pdf CreateOrderForm(string name, string street, string city, string state, string zip, int countryID, string email, int numberOfCopies, bool isTaxExempt, ShippingTypes shippingOption)
		{
			var unitPrice = 55M;
			var shippingCost = GetShippingCost(shippingOption);
			var taxRate = RetrieveSalesTaxRate(street, city, zip);
			var tax = (numberOfCopies * unitPrice + shippingCost) * taxRate;
			var pdf = new Pdf();
			var section = new Section();
			var ReportUtilities = new ReportUtilities(12);
			var overAllTable = ReportUtilities.GetNewTable();
			var table = ReportUtilities.AppendNewTable(overAllTable, 1);
			var row = table.Rows.Add();
			var cell = ReportUtilities.AddLabelCell(row, "Natural Resource Econometrics Textbook Order Form", AlignmentType.Left);

			ReportUtilities.AddLabelCellNonBold(table.Rows.Add(), "Name: " + name, AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(table.Rows.Add(), "Email: " + email, AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(table.Rows.Add(), "Address: ", AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(table.Rows.Add(), "     " + street, AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(table.Rows.Add(), "     " + city + ", " + state + " " + zip, AlignmentType.Left);

			ReportUtilities.AddLabelCell(table.Rows.Add(), " ", AlignmentType.Left);
			ReportUtilities.AddLabelCell(table.Rows.Add(), "--------------------------", AlignmentType.Left);
			ReportUtilities.AddLabelCell(table.Rows.Add(), " ", AlignmentType.Left);

			if (isTaxExempt)
			{
				ReportUtilities.AddLabelCellNonBold(table.Rows.Add(), "In order to process as Tax Exempt, please send along your tax exempt sheet", AlignmentType.Left);
			}

			ReportUtilities.AddLabelCellNonBold(table.Rows.Add(), "Number Of Copies: " + numberOfCopies, AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(table.Rows.Add(), "Unit Price: " + unitPrice.ToString("C2"), AlignmentType.Left);
			if (tax > 0)
			{
				ReportUtilities.AddLabelCellNonBold(table.Rows.Add(), "Tax: " + tax.ToString("C2"), AlignmentType.Left);
			}
			ReportUtilities.AddLabelCellNonBold(table.Rows.Add(), "Shipping Cost: " + shippingCost.ToString("C2"), AlignmentType.Left);

			var total = Convert.ToDecimal(numberOfCopies) * unitPrice + tax + shippingCost;

			ReportUtilities.AddLabelCellNonBold(table.Rows.Add(), "Total Cost: " + total.ToString("C2"), AlignmentType.Left);


			ReportUtilities.AddLabelCell(table.Rows.Add(), " ", AlignmentType.Left);
			ReportUtilities.AddLabelCell(table.Rows.Add(), "--------------------------", AlignmentType.Left);
			ReportUtilities.AddLabelCell(table.Rows.Add(), " ", AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(table.Rows.Add(), "Send a check or money order (do not send cash), in US Currency, to:", AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(table.Rows.Add(), "D & D Larix, LLC", AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(table.Rows.Add(), "1515 NW Kenny", AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(table.Rows.Add(), "Pullman, WA 99163-3724", AlignmentType.Left);

			section.Paragraphs.Add(overAllTable);
			pdf.Sections.Add(section);
			return pdf;
		}

		private decimal GetShippingCost(ShippingTypes shippingOption)
		{
			switch (shippingOption)
			{
				case ShippingTypes.USExpressMail:
					return 20M;
				case ShippingTypes.USPriortyMail:
					return 7.5M;
				case ShippingTypes.USStandardPost:
					return 6.50M;
				case ShippingTypes.CAExpressMail:
					return 40M;
				case ShippingTypes.CAPriorityMail:
					return 25M;
				case ShippingTypes.CAFirstClassMail:
					return 10M;
				default:
					throw new Exception("Invalid Shipping");
			}
		}

	}
}
