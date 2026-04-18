using MOBWEB_TEST.Screens.DataEntrySubsystems;

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
            Routing.RegisterRoute("StandEntryScreen", typeof(Screens.DataEntrySubsystems.StandEntryData));
            Routing.RegisterRoute("PlotEntryScreen", typeof(Screens.DataEntrySubsystems.PlotEntry));
            Routing.RegisterRoute("TreeEntryPage", typeof(Screens.DataEntrySubsystems.TreeEntryPage));
            Routing.RegisterRoute("MesicSubsystemScreen", typeof(Screens.DataEntrySubsystems.MesicSubsystemScreen));
            Routing.RegisterRoute("GyroscopeScreen", typeof(Screens.DataEntrySubsystems.GyroscopeScreen));
            Routing.RegisterRoute("DefectScreen", typeof(Screens.DataEntrySubsystems.GyroscopeSubsystem.DefectScreen));
            Routing.RegisterRoute("PlotSlopeScreen", typeof(Screens.PlotSlopeScreen));
            Routing.RegisterRoute("LocationDemo", typeof(Screens.LocationDemo));
            Routing.RegisterRoute("DatabaseTestScreen",typeof(Screens.DatabaseTestScreen));
            Routing.RegisterRoute("NavigationScreen", typeof(Screens.NavigationScreen));
        }
    }
}