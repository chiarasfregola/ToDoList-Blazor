using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient; //x chiamate HHTP alla WebApi
        private readonly IJSRuntime _jsRuntime; //interagisce con il localStorage
        private readonly NavigationManager _navigationManager; //nagiva tra pagine Blazor
        private string _currentToken = string.Empty; //token JWT
        private bool _jsRuntimeReady = false;

        //evento lanciato ogni volta che cambio lo stato di autenticazione
        public event Action? OnAuthStateChanged;

        //restituisco il token e lo stato login
        public string CurrentToken => _currentToken;
        public bool IsLoggedIn => !string.IsNullOrWhiteSpace(_currentToken);

        //Depedency injection
        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
        }

        public void EnableJsInterop() => _jsRuntimeReady = true;


        //registrazione dell'utente
        public async Task<Result> RegisterAsync(string email, string password)
        {
            var registerModel = new RegisterModel { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7152/api/Register", registerModel);

            if (response.IsSuccessStatusCode)
                return new Result { IsSuccess = true };

            var errorMessage = await response.Content.ReadAsStringAsync();
            return new Result { IsSuccess = false, ErrorMessage = errorMessage };
        }


        //login dell'utente
        public async Task<Result> LoginAsync(string email, string password)
        {
            var loginModel = new LoginModel { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7152/api/Login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                var token = loginResponse?.Token ?? string.Empty;

                await SetTokenInternalAsync(token);
                return new Result { IsSuccess = true, Token = token };
            }

            var error = await response.Content.ReadAsStringAsync();
            return new Result { IsSuccess = false, ErrorMessage = error };
        }

        //salvataggio del token
        public async Task SaveTokenAsync(string token)
        {
            if (!_jsRuntimeReady)
                throw new InvalidOperationException("JavaScript interop non è disponibile prima del rendering.");

            await SetTokenInternalAsync(token);
        }
        private async Task SetTokenInternalAsync(string token)
        {
            _currentToken = token; //aggiorno 
            if (_jsRuntimeReady)
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token); //salvo nel localStorage
            //lancio OnAuthStateChanged che chiamo in NavMenu e MainLayout
            OnAuthStateChanged?.Invoke();
        }

        //recuper il token dal localStorage
        public async Task<string> GetTokenAsync()
        {
            if (!_jsRuntimeReady)
                return string.Empty;

            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken") ?? string.Empty;

            if (_currentToken != token) //se diverso aggiorno e lancio OnAuthStateChanged
            {
                _currentToken = token;
                OnAuthStateChanged?.Invoke();
            }

            return _currentToken;
        }

        public async Task LogoutAsync()
        {
            _currentToken = string.Empty;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            OnAuthStateChanged?.Invoke();
        }

        // Modelli interni
        public class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
        }

        public class Result
        {
            public bool IsSuccess { get; set; }
            public string ErrorMessage { get; set; } = string.Empty;
            public string Token { get; set; } = string.Empty;
        }

        public class LoginModel
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class RegisterModel
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}

