using Microsoft.AspNetCore.Mvc; //per gestire le richieste HTTP
using Microsoft.EntityFrameworkCore; //per accesso al DB
using Microsoft.AspNetCore.Authorization; //auteticazione e autorizzazione degli endpoint
using ToDoApi.Models;  //per ToDoItem
using ToDoApi.Data; //per ToDoContext

namespace ToDoList.WebApi.Controllers
{
    [Authorize] //tutto valido per utente autenticato
    [Route("api/[controller]")]  
    [ApiController] 
    public class ToDoController : ControllerBase
    {
        private readonly ToDoContext _context;

        //Dependency Injection
        public ToDoController(ToDoContext context)
        {
            _context = context;
        }

        // GET: recupera tutti i ToDoItem e restituisce una lista
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ToDoItem>>> GetToDoItems()
        {
            return await _context.ToDoItems.ToListAsync();
        }

        // GET: recupera l'attività a partire dall'id
        [HttpGet("{id}")] 
        public async Task<ActionResult<ToDoItem>> GetToDoItem(int id)
        {
            // Cerca l'attività per id
            var toDoItem = await _context.ToDoItems.FindAsync(id);

            // Se l'attività non è trovata, restituisce NotFound
            if (toDoItem == null)
            {
                return NotFound();
            }

            return toDoItem;
        }

        // POST: creazione della nuova attività
        [HttpPost("New")]
        public async Task<ActionResult<ToDoItem>> PostToDoItem(ToDoItem toDoItem)
        {
            _context.ToDoItems.Add(toDoItem);

            // Salva i cambiamenti nel database
            await _context.SaveChangesAsync();

            // Restituisci una risposta con l'ID dell'elemento appena creato
            return CreatedAtAction(nameof(GetToDoItem), new { id = toDoItem.Id }, toDoItem);
        }

        // PUT: aggiorna un'attività esistente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToDoItem(int id, ToDoItem toDoItem)
        {
            // Verifica che l'ID dell'elemento da aggiornare corrisponda
            if (id != toDoItem.Id)
            {
                return BadRequest();
            }

            // Segna l'elemento come modificato
            _context.Entry(toDoItem).State = EntityState.Modified;

            try
            {
                // Salva i cambiamenti
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Se l'elemento non esiste più, restituisce NotFound
                if (!ToDoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: elimina l'attività
        [HttpDelete("{id}")] 
        public async Task<IActionResult> DeleteToDoItem(int id)
        {
            // Trova l'attività da eliminare
            var toDoItem = await _context.ToDoItems.FindAsync(id);

            // Se l'attività non è trovata, restituisce NotFound
            if (toDoItem == null)
            {
                return NotFound();
            }

            // Rimuove l'attività dal contesto
            _context.ToDoItems.Remove(toDoItem);

            // Salva i cambiamenti nel database
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Verifica se l'elemento esiste nel database
        private bool ToDoItemExists(int id)
        {
            return _context.ToDoItems.Any(e => e.Id == id);
        }
    }
}
