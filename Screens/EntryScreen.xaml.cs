namespace MOBWEB_TEST.Screens
{
    public partial class EntryScreen : ContentPage
    {
        public EntryScreen()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object? sender, EventArgs e)
        {
            // if null set to empty
            string username = UsernameEntry.Text ?? string.Empty;
            string password = PasswordEntry.Text ?? string.Empty;

            //TODO: IMPLEMENT PROPER AUTHENTICATION
            if (username.ToLower() == "admin" && password.ToLower() == "admin")
            {
                // go to homescreen using shell nav
                await Shell.Current.GoToAsync("///HomeScreen");
            }
            else
            {
                await DisplayAlert("Login Failed", "Invalid username or password. Please try again.", "OK");

                UsernameEntry.Text = string.Empty;
                PasswordEntry.Text = string.Empty;
            }
        }
    }
}
