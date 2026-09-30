using MOBWEB_TEST.Screens.DataEntrySubsystems;
using MOBWEB_TEST.Screens.DataTransferSubsystems;
namespace MOBWEB_TEST
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for all "sub-pages". 
            // Because they are registered here and NOT in the XAML, 
            // they will remain hidden from the main menu but can still be navigated to.
            Routing.RegisterRoute("StandEntryData", typeof(Screens.DataEntrySubsystems.StandEntryData));
            Routing.RegisterRoute("PlotEntryScreen", typeof(Screens.DataEntrySubsystems.PlotEntry));
            Routing.RegisterRoute("TreeEntryPage", typeof(Screens.DataEntrySubsystems.TreeEntryPage));
            Routing.RegisterRoute("MesicSubsystemScreen", typeof(Screens.DataEntrySubsystems.MesicSubsystemScreen));
            Routing.RegisterRoute("DataUploadScreen", typeof(Screens.DataTransferSubsystems.DataUploadScreen));
            Routing.RegisterRoute("DataDownloadScreen", typeof(Screens.DataTransferSubsystems.DataDownloadScreen));
            Routing.RegisterRoute("DataOverviewScreen", typeof(Screens.DataTransferSubsystems.DataOverviewScreen));
            Routing.RegisterRoute("PlotSlopeScreen", typeof(Screens.PlotSlopeScreen));
            Routing.RegisterRoute("LocationDemo", typeof(Screens.LocationDemo));
            Routing.RegisterRoute("DatabaseTestScreen",typeof(Screens.DatabaseTestScreen));
            Routing.RegisterRoute("PlotCoordinateSet", typeof(Screens.PlotCoordinateSet));
            Routing.RegisterRoute("TreeHeightEntryPage",typeof(Screens.DataEntrySubsystems.TreeHeightEntryPage));
            Routing.RegisterRoute("DefectScreen", typeof(Screens.DataEntrySubsystems.DefectScreen));
            Routing.RegisterRoute("SummaryScreen", typeof(Screens.DataEntrySubsystems.SummaryScreen));
            Routing.RegisterRoute("DataEntryScreen", typeof(Screens.DataEntryScreen));
        }
    }
}