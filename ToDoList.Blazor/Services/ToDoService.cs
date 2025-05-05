using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using ToDoApi.Models;

namespace Services
{
    //riceve un HttpClient tramite Dependency Injection
    public class ToDoService(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;
        private string? _token;

        //imposta il token ricevuto dal login
        public void SetAuthorizationToken(string token)
        {
            _token = token;
        }

        //aggiunge il token all'header Authorization
        private Task<bool> AddAuthorizationHeaderAsync()
        {
            if (string.IsNullOrEmpty(_token)) return Task.FromResult(false);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            return Task.FromResult(true);
        }

        //recupera i ToDo dal backend
        public async Task<List<ToDoItem>> GetAllToDoItemsAsync()
        {
            if (!await AddAuthorizationHeaderAsync()) return new List<ToDoItem>();
            
            //restituisce una lista di oggetti ToDoItem
            return await _httpClient.GetFromJsonAsync<List<ToDoItem>>("https://localhost:7152/api/ToDo/List") ?? new();
        }

        //nuovo ToDoItem
        public async Task AddToDoItemAsync(ToDoItem toDoItem)
        {
            if (!await AddAuthorizationHeaderAsync()) return;

            await _httpClient.PostAsJsonAsync("https://localhost:7152/api/ToDo/New", toDoItem);
        }

        //fa riferimento al metodo PUT, funziona con l'ID
        public async Task UpdateToDoItemAsync(ToDoItem toDoItem)
        {
            if (!await AddAuthorizationHeaderAsync()) return;

            await _httpClient.PutAsJsonAsync($"https://localhost:7152/api/ToDo/{toDoItem.Id}", toDoItem);
        }

        //chiama il metodo DELETE del back
        public async Task DeleteToDoItemAsync(int id)
        {
            if (!await AddAuthorizationHeaderAsync()) return;

            await _httpClient.DeleteAsync($"https://localhost:7152/api/ToDo/{id}");
        }
    }
}