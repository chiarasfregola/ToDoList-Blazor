using Blazored.LocalStorage;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace ToDoList.Blazor.Wasm.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public event Action? OnAuthStateChanged;

        private const string TokenKey = "authToken";
        private const string BaseUrl = "https://localhost:7152/api/";

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            var registerModel = new { Email = email, Password = password };

            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}Register", registerModel);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var loginModel = new { Email = email, Password = password };

            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}Login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.Token))
                {
                    await _localStorage.SetItemAsync(TokenKey, tokenResponse.Token);
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
                    NotifyAuthStateChanged();
                    return true;
                }
            }

            return false;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync(TokenKey);
            _httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthStateChanged();
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(TokenKey);
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>(TokenKey);
        }

        private void NotifyAuthStateChanged()
        {
            OnAuthStateChanged?.Invoke();
        }
    }

    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
