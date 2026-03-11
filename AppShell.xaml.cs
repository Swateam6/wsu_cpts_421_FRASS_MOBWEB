using MOBWEB_TEST.Screens.DataEntrySubsystems;

namespace MOBWEB_TEST
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register the "address" for each screen so GoToAsync works
            Routing.RegisterRoute("StandEntryScreen", typeof(Screens.DataEntrySubsystems.StandEntryData));
            Routing.RegisterRoute("PlotEntryScreen", typeof(Screens.DataEntrySubsystems.PlotEntry));
            Routing.RegisterRoute("TreeEntryPage",typeof(Screens.DataEntrySubsystems.TreeEntryPage));

            // If you have a general Data Entry Hub:
            Routing.RegisterRoute("DataEntryScreen", typeof(Screens.DataEntryScreen));
        }
    }
}
