using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Models;

namespace Services
{
    public class ToDoService
    {
        private readonly HttpClient _httpClient;

        public ToDoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ToDoItem>> GetAllToDoItemsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ToDoItem>>("api/ToDo");
        }

        public async Task<ToDoItem> GetToDoItemByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ToDoItem>($"api/ToDo/{id}");
        }

        public async Task AddToDoItemAsync(ToDoItem toDoItem)
        {
            var response = await _httpClient.PostAsJsonAsync("api/ToDo", toDoItem);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateToDoItemAsync(ToDoItem toDoItem)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/ToDo/{toDoItem.Id}", toDoItem);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteToDoItemAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/ToDo/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
