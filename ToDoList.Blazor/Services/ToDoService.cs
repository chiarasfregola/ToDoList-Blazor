using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using ToDoList.Blazor;

public class ToDoService
{
    private readonly HttpClient _httpClient;

    public ToDoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TodoItem>> GetTodoItemsAsync()
    {
        var items = await _httpClient.GetFromJsonAsync<List<TodoItem>>("https://localhost:5001/api/todo");
        return items ?? new List<TodoItem>();
    }

    public async Task AddTodoItemAsync(TodoItem item)
    {
        await _httpClient.PostAsJsonAsync("https://localhost:5001/api/todo", item);
    }

    public async Task DeleteTodoItemAsync(int id)
    {
        await _httpClient.DeleteAsync($"https://localhost:5001/api/todo/{id}");
    }
}

public class TodoItem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool IsCompleted { get; set; }
}
