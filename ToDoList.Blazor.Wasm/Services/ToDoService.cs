using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using ToDoList.Blazor.Wasm.Models;

namespace ToDoList.Blazor.Wasm.Services
{
    public class ToDoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public ToDoService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        private async Task SetAuthHeaderAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<ToDoItem>> GetToDoItemsAsync()
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetFromJsonAsync<List<ToDoItem>>("https://localhost:7152/api/ToDo/List");
            return response ?? new List<ToDoItem>();
        }

        public async Task<ToDoItem> CreateToDoItemAsync(ToDoItem toDoItem)
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7152/api/ToDo/New", toDoItem);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ToDoItem>() ?? new ToDoItem();
        }

        public async Task<ToDoItem> UpdateToDoItemAsync(ToDoItem toDoItem)
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.PutAsJsonAsync($"https://localhost:7152/api/ToDo/{toDoItem.Id}", toDoItem);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Errore aggiornamento: {response.StatusCode} - {error}");
            }

            return await response.Content.ReadFromJsonAsync<ToDoItem>() ?? toDoItem;
        }

        public async Task DeleteToDoItemAsync(int id)
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync($"https://localhost:7152/api/ToDo/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
