using System.Linq;
using System.Collections.Generic;
using System.Data.Linq;
using FRASS.DAL;
using FRASS.Interfaces;
using FRASS.Services.Proxies.ReportService;
namespace FRASS.Services.Proxies
{
    public class ReportServiceProxy : IReportService
    {
        //public void GenerateFullParcelReportDeliveredMarketModel(int UserID, int ParcelID, int PortfolioID, int RPAPortfolioID)
        //{
        //	var proxy = new ReportServiceClient();
        //	proxy.GenerateFullParcelReportDeliveredMarketModel(UserID, ParcelID, PortfolioID, RPAPortfolioID);

        //}
        public void GenerateFullParcelReportDeliveredMarketModel(int UserID, int ParcelID, int PortfolioID)
        {
            var proxy = new ReportServiceClient();
            proxy.GenerateFullParcelReportDeliveredMarketModel(UserID, ParcelID, PortfolioID);

        }

        public void GenerateFullParcelReportStumpageMarketModel(int UserID, int ParcelID, int PortfolioID)
        {
            var proxy = new ReportServiceClient();
            proxy.GenerateFullParcelReportStumpageMarketModel(UserID, ParcelID, PortfolioID);
        }
    }
}