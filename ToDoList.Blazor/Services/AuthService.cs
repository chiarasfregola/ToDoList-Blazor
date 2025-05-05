using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ToDoList.WebApi.Controllers;

namespace Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly NavigationManager _navigationManager;
        private string _currentToken;
        private bool _jsRuntimeReady = false;

        public string CurrentToken => _currentToken;

        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
        }

        public void EnableJsInterop()
        {
            _jsRuntimeReady = true;
        }

        public async Task<Result> RegisterAsync(string email, string password)
        {
            var registerModel = new RegisterModel { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7152/api/Register", registerModel);

            if (response.IsSuccessStatusCode)
                return new Result { IsSuccess = true };

            var errorMessage = await response.Content.ReadAsStringAsync();
            return new Result { IsSuccess = false, ErrorMessage = errorMessage };
        }

        public async Task<Result> LoginAsync(string email, string password)
        {
            var loginModel = new LoginModel { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7152/api/Login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                _currentToken = loginResponse.Token;

                if (_jsRuntimeReady)
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", loginResponse.Token);

                return new Result { IsSuccess = true, Token = loginResponse.Token };
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return new Result { IsSuccess = false, ErrorMessage = errorMessage };
        }

        public async Task SaveTokenAsync(string token)
        {
            if (!_jsRuntimeReady)
                throw new InvalidOperationException("JavaScript interop non è disponibile prima del completamento del rendering.");

            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
        }

        public async Task<string> GetTokenAsync()
        {
            if (!_jsRuntimeReady)
            {
                Console.WriteLine("⚠️ JS Interop non è pronto.");
                return string.Empty;
            }

            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        }

        public async Task RemoveTokenAsync()
        {
            if (!_jsRuntimeReady)
                throw new InvalidOperationException("JavaScript interop non è disponibile prima del completamento del rendering.");

            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        }

    public async Task LogoutAsync()
    {
        _currentToken = null;
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
    }

        public class LoginResponse
        {
            public string Token { get; set; }
        }

        public class Result
        {
            public bool IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public string Token { get; set; }
        }

        public class LoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
