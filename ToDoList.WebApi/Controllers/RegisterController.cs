using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using ToDoApi.Models;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        //Dependency Injection di UserManager
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        //metodo di registrazione
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            //creo un nuovo oggetto usando l'email 
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            //con CreateAsync registro l'utente nel Db
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok();
            }

            // Restituisce errori come lista di stringhe per il frontend
            var errorMessages = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(errorMessages);
        }
    }

    public class RegisterModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
