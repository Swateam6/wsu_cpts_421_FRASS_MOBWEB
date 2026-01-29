using System.Collections.Generic;
using System.Data.Linq;
using System.ServiceModel;
using FRASS.DAL;
using FRASS.Interfaces;

namespace FRASS.Services.ReportService
{
	[ServiceContract]
	public interface IReportService
	{
		[OperationContract]
		void GenerateFullParcelReportDeliveredMarketModel(int UserID, int ParcelID, int PortfolioID, int RPAPortfolioID);

		[OperationContract]
		void GenerateFullParcelReportStumpageMarketModel(int UserID, int ParcelID, int PortfolioID);
	}
}