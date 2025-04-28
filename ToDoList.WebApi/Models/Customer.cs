namespace ToDoApi.Models
{
    public class Customer
    {
        public int Id { get; set; }        // Id del cliente (chiave primaria)
        public required string Name { get; set; } = string.Empty;  // Nome del cliente
        public required string Email { get; set; } = string.Empty;  // Email del cliente

    }
}
