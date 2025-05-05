using Microsoft.AspNetCore.Identity.EntityFrameworkCore; //importa le classi per usare Identity EF Core
using Microsoft.EntityFrameworkCore; //per interagire con il DB
using ToDoApi.Models;


namespace ToDoApi.Data
{
    public class ToDoContext : IdentityDbContext<ApplicationUser>
    {
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options) { }

        public DbSet<ToDoItem> ToDoItems { get; set; }
    }
}
