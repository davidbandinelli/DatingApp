using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    // ControllerBase is for viewless controllers (API)
    public class AuthController : ControllerBase {
        //private readonly DataContext _context;
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        // la dependency injection dei repository o della configurazione nel controller fa parte degli API controller e quindi non serve più Castle.Windsor (o altri ioc container)
        public AuthController(IAuthRepository repo, IConfiguration config) {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto) {

            // validate request (il modelstate non serve con l'ApiController)
            //if (!ModelState.IsValid) return BadRequest(ModelState);
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if (await _repo.UserExists(userForRegisterDto.Username)) {
                return BadRequest("Username already exists");
            }
            var userToCreate = new User {
                Username = userForRegisterDto.Username
            };
            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto) {

            // le eccezioni vengono gestite da un handler globale definito in Startup.cs
            //throw new Exception("AuthController -> Login says NO!");

            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null) return Unauthorized();

            // se l'utente esiste viene creato il token JWT con i claims dell'utente
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            // generazione token JWT (firmato e con una vliadità di 1 giorno)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // token restituito nella response
            // il token può essere verificato sul sito https://jwt.io/
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });

        }
    }
            
}
