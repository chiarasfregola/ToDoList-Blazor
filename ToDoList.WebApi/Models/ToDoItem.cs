namespace ToDoApi.Models;
public class ToDoItem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool IsDone { get; set; }

    public string? UserId { get; set; }
}
