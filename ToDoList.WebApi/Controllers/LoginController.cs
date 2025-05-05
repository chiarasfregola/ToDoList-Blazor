using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ToDoApi.Models;


namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        //serve per controllare l'identità e la password dell'utente
        private readonly SignInManager<ApplicationUser> _signInManager;

        //accede ai dati di configurazione
        private readonly IConfiguration _configuration;

        public LoginController(SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _configuration = configuration;
        }

        //riceve in oggetto LoginModel
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            //cerca l'utente tramite email
            var user = await _signInManager.UserManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized();
            //verifica se la password è corretta
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            //se l'utente non è trovato o la password è errata: UNAUTHORIZED
            if (!result.Succeeded)
                return Unauthorized();

            //generazione del token JWT
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            //recupera la chiave dall'appsettings.json
            var authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration")
                )
            );
            
            //creo il token JWT
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            //restituisco il token
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
    }

    public class LoginModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
