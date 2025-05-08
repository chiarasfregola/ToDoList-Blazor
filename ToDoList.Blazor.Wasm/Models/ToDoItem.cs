namespace ToDoList.Blazor.Wasm.Models
{
    public class ToDoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsDone { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}