using MOBWEB_TEST.Services;

namespace MOBWEB_TEST.Screens
{
    public partial class EntryScreen : ContentPage
    {
        private readonly AuthService _auth = new AuthService();

        public EntryScreen()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object? sender, EventArgs e)
        {
            LoginButton.IsEnabled = false;
            try
            {
                var result = await _auth.LoginAsync();

                switch (result)
                {
                    case AuthService.LoginResult.Success:
                        await Shell.Current.GoToAsync("DataOverviewScreen");
                        break;

                    case AuthService.LoginResult.Cancelled:
                        break; // user backed out, no message needed

                    case AuthService.LoginResult.NetworkError:
                        await DisplayAlert("No Connection",
                            "Could not reach the FRASS server. Check your connection and try again.", "OK");
                        break;

                    default:
                        await DisplayAlert("Login Failed",
                            "Could not complete the login. Please try again.", "OK");
                        break;
                }
            }
            finally
            {
                LoginButton.IsEnabled = true;
            }

        }
    }
}
