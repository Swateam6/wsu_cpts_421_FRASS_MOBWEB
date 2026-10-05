using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace MOBWEB_TEST.Services
{
    public class AuthService
    {
        // go to website and back
        const string BaseUrl = "https://frass.forest-econometrics.com";
        const string CallbackUrl = "frassapp://callback";

        static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(30) };

        public enum LoginResult { Success, Cancelled, Failed, NetworkError }

        public async Task<LoginResult> LoginAsync()
        {
            //generate a random code verifier and challenge for proof key code exchange (used in oauth for login)
            string verifier = Base64Url(RandomNumberGenerator.GetBytes(32));
            string challenge = Base64Url(SHA256.HashData(Encoding.ASCII.GetBytes(verifier)));
            string state = Base64Url(RandomNumberGenerator.GetBytes(16));

            var loginUri = new Uri(
                $"{BaseUrl}/default.aspx?mobile=1" +
                $"&code_challenge={challenge}" +
                $"&state={state}");

            // opens android custom tab (add ios later) and waits for the callback url to be called
            WebAuthenticatorResult authResult;
            try
            {
                authResult = await WebAuthenticator.Default.AuthenticateAsync(
                    new WebAuthenticatorOptions
                    {
                        Url = loginUri,
                        CallbackUrl = new Uri(CallbackUrl)
                    });
            }
            catch (TaskCanceledException)
            {
                return LoginResult.Cancelled; // user closed the browser
            }

            //check that response is valid and contains the code and state we sent
            if (!authResult.Properties.TryGetValue("state", out var returnedState) || returnedState != state)
                return LoginResult.Failed;

            if (!authResult.Properties.TryGetValue("code", out var code) || string.IsNullOrEmpty(code))
                return LoginResult.Failed;

            //get a token from the website with the code and verifier
            // check and save only if valid then return success
            try
            {
                var response = await Http.PostAsync($"{BaseUrl}/MobileToken.ashx",
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["grant_type"] = "authorization_code",
                        ["code"] = code,
                        ["code_verifier"] = verifier
                    }));

                if (!response.IsSuccessStatusCode)
                    return LoginResult.Failed;

                var tokens = await response.Content.ReadFromJsonAsync<TokenResponse>();
                if (tokens?.AccessToken == null || tokens.User == null)
                    return LoginResult.Failed;

                await SaveSessionAsync(tokens);
                return LoginResult.Success;
            }
            catch (HttpRequestException)
            {
                return LoginResult.NetworkError;
            }
            catch (TaskCanceledException) // HttpClient timeout
            {
                return LoginResult.NetworkError;
            }
        }

        static async Task SaveSessionAsync(TokenResponse t)
        {
            await SecureStorage.Default.SetAsync("access_token", t.AccessToken!);
            await SecureStorage.Default.SetAsync("refresh_token", t.RefreshToken ?? "");
            await SecureStorage.Default.SetAsync("access_expires_utc",
                DateTime.UtcNow.AddSeconds(t.ExpiresIn).ToString("O"));
            await SecureStorage.Default.SetAsync("user_id", t.User!.Id.ToString());
        }

        public static async Task<string?> GetAccessTokenAsync() =>
            await SecureStorage.Default.GetAsync("access_token");

        public static async Task<bool> IsLoggedInAsync() =>
            !string.IsNullOrEmpty(await SecureStorage.Default.GetAsync("refresh_token"));

        public static void Logout() => SecureStorage.Default.RemoveAll();

        static string Base64Url(byte[] bytes) =>
            Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        // JSON models matching MobileToken.ashx
        class TokenResponse
        {
            [JsonPropertyName("access_token")] public string? AccessToken { get; set; }
            [JsonPropertyName("refresh_token")] public string? RefreshToken { get; set; }
            [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
            [JsonPropertyName("user")] public UserInfo? User { get; set; }
        }

        class UserInfo
        {
            [JsonPropertyName("id")] public int Id { get; set; }
        }
    }
}