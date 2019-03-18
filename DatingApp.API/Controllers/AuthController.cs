using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers {

    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    // ControllerBase is for viewless controllers (API)
    public class AuthController : ControllerBase {
        //private readonly DataContext _context;
        //private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        private readonly SignInManager<User> _signInManager;

        // la dependency injection dei repository o della configurazione nel controller fa parte degli API controller e quindi non serve più Castle.Windsor (o altri ioc container)
        public AuthController(IConfiguration config, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto) {

            // validate request (il modelstate non serve con l'ApiController)
            //if (!ModelState.IsValid) return BadRequest(ModelState);
            //userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            /*
            if (await _repo.UserExists(userForRegisterDto.Username)) {
                return BadRequest("Username already exists");
            }
            */

            var userToCreate = _mapper.Map<User>(userForRegisterDto);
            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);
            //var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);
            var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);
            if (result.Succeeded) {
                return CreatedAtRoute("GetUser", new { controller="Users", id = userToCreate.Id }, userToReturn);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto) {

            // le eccezioni vengono gestite da un handler globale definito in Startup.cs
            //throw new Exception("AuthController -> Login says NO!");

            var user = await _userManager.FindByNameAsync(userForLoginDto.Username);
            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);
            if (result.Succeeded) {
                var appUser = await _userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.Username.ToUpper());
                var userToReturn = _mapper.Map<UserForListDto>(appUser);
                // token restituito nella response
                // il token può essere verificato sul sito https://jwt.io/
                return Ok(new {
                    token = GenerateJwtToken(appUser),
                    user = userToReturn
                });
            }
            return Unauthorized();
        }

        private string GenerateJwtToken(User user) {
            // se l'utente esiste viene creato il token JWT con i claims dell'utente
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            // aggiunta dei ruoli ai claim del token
            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles) {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            //claims.Add(new Claim(ClaimTypes.Role, "Member"));
            // generazione token JWT (firmato e con una validità di 1 giorno)
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
            return tokenHandler.WriteToken(token);
        }
    }
            
}
