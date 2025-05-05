using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ToDoApi.Models;
using ToDoApi.Data;

namespace ToDoList.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly ToDoContext _context;

        //inietta il Db
        public ToDoController(ToDoContext context)
        {
            _context = context;
        }

        //api/todo/list: restituisce la lista associata all'utente
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ToDoItem>>> GetToDoItems()
        {
            var userId = User.Identity?.Name;
            if (userId == null)
                return Unauthorized();

            var toDoItems = await _context.ToDoItems
                .Where(t => t.UserId == userId)
                .ToListAsync();

            return Ok(toDoItems);
        }

        //api/todo/{id}: 
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoItem>> GetToDoItem(int id)
        {
            var userId = User.Identity?.Name;
            var toDoItem = await _context.ToDoItems.FindAsync(id);

            if (toDoItem == null || toDoItem.UserId != userId)
            {
                return NotFound();
            }

            return Ok(toDoItem);
        }

        // POST: api/todo/new
        [HttpPost("New")]
        public async Task<ActionResult<ToDoItem>> PostToDoItem(ToDoItem toDoItem)
        {
            var userId = User.Identity?.Name;
            if (userId == null)
                return Unauthorized();

            toDoItem.UserId = userId;
            _context.ToDoItems.Add(toDoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetToDoItem), new { id = toDoItem.Id }, toDoItem);
        }

        // PUT: api/todo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToDoItem(int id, ToDoItem toDoItem)
        {
            var userId = User.Identity?.Name;

            if (id != toDoItem.Id)
                return BadRequest();

            var existing = await _context.ToDoItems.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

            if (existing == null || existing.UserId != userId)
                return NotFound();

            toDoItem.UserId = userId;
            _context.Entry(toDoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoItemExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/todo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDoItem(int id)
        {
            var userId = User.Identity?.Name;
            var toDoItem = await _context.ToDoItems.FindAsync(id);

            if (toDoItem == null || toDoItem.UserId != userId)
                return NotFound();

            _context.ToDoItems.Remove(toDoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //controlla se un elemento esiste nel DB (cerca per id)
        private bool ToDoItemExists(int id)
        {
            return _context.ToDoItems.Any(e => e.Id == id);
        }
    }
}
