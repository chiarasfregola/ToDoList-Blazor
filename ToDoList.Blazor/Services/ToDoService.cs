using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using ToDoApi.Models;

namespace Services
{
    public class ToDoService
    {
        private readonly HttpClient _httpClient;
        private string _token;

        public ToDoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void SetAuthorizationToken(string token)
        {
            _token = token;
        }

        private Task<bool> AddAuthorizationHeaderAsync()
        {
            if (string.IsNullOrEmpty(_token)) return Task.FromResult(false);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            return Task.FromResult(true);
        }

        public async Task<List<ToDoItem>> GetAllToDoItemsAsync()
        {
            if (!await AddAuthorizationHeaderAsync()) return new List<ToDoItem>();

            return await _httpClient.GetFromJsonAsync<List<ToDoItem>>("https://localhost:7152/api/ToDo/List") ?? new();
        }

        public async Task AddToDoItemAsync(ToDoItem toDoItem)
        {
            if (!await AddAuthorizationHeaderAsync()) return;

            await _httpClient.PostAsJsonAsync("https://localhost:7152/api/ToDo/New", toDoItem);
        }

        public async Task UpdateToDoItemAsync(ToDoItem toDoItem)
        {
            if (!await AddAuthorizationHeaderAsync()) return;

            await _httpClient.PutAsJsonAsync($"https://localhost:7152/api/ToDo/{toDoItem.Id}", toDoItem);
        }

        public async Task DeleteToDoItemAsync(int id)
        {
            if (!await AddAuthorizationHeaderAsync()) return;

            await _httpClient.DeleteAsync($"https://localhost:7152/api/ToDo/{id}");
        }
    }
}
