using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Text;
using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Linq;

namespace Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager,
                              SignInManager<IdentityUser> signInManager,
                              IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Проверяем существование пользователя по email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new { error = "Неверные учётные данные" });
            }

            // Проверяем пароль
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { error = "Неверные учётные данные" });
            }

            // Получаем роль пользователя (предположим, что пользователю назначена ровно одна роль)
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "";

            // Формируем набор claims: ID, роль и дополнительный claim "guid" (в данном случае тот же user.Id)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("guid", user.Id),
                new Claim(ClaimTypes.Role, role)
            };

            // Загружаем секрет из конфигурации
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Создаем JWT-токен со временем жизни 1 час (можно изменить)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Возвращаем токен
            return Ok(new { token = tokenString });
        }

        // POST: api/auth/create-users
        // Принимает список email-ов и создает для каждого пользователя запись с паролем "12345"
        [HttpPost("create-users")]
        public async Task<IActionResult> CreateUsers([FromBody] List<string> emails)
        {
            var createdUsers = new List<string>();

            foreach (var email in emails)
            {
                // Если пользователь уже существует, пропускаем
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    continue;
                }

                var user = new IdentityUser { UserName = email, Email = email };
                var result = await _userManager.CreateAsync(user, "12345");
                if (result.Succeeded)
                {
                    createdUsers.Add(email);
                }
            }

            return Ok(new { created = createdUsers });
        }
    }

    // Класс-запрос для логина
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
