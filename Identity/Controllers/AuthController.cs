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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // POST: api/auth/login
        // Принимает JSON с полем "email"
        // Если пользователя не существует, создаёт его с паролем "123456"
        // Генерирует и возвращает JWT-токен с данными пользователя и ролью
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Фиксированный пароль, используйте минимум 6 символов
            const string fixedPassword = "123456";

            // Ищем пользователя по email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Если пользователь не существует, создаём его
                user = new IdentityUser { Email = request.Email, UserName = request.Email };
                var createResult = await _userManager.CreateAsync(user, fixedPassword);
                if (!createResult.Succeeded)
                {
                    return BadRequest(new { error = "Не удалось создать пользователя", details = createResult.Errors });
                }

                // Перед назначением роли проверяем, есть ли роль "Partner"
                if (!await _roleManager.RoleExistsAsync("Partner"))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole("Partner"));
                    if (!roleResult.Succeeded)
                    {
                        return BadRequest(new { error = "Не удалось создать роль", details = roleResult.Errors });
                    }
                }

                // Назначаем роль "Partner" пользователю
                var addRoleResult = await _userManager.AddToRoleAsync(user, "Partner");
                if (!addRoleResult.Succeeded)
                {
                    return BadRequest(new { error = "Ошибка при назначении роли", details = addRoleResult.Errors });
                }
            }

            // Проверяем, что пароль соответствует значению "123456"
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, fixedPassword, false);
            if (!signInResult.Succeeded)
            {
                return Unauthorized(new { error = "Некорректные данные для входа" });
            }

            // Получаем роль пользователя, если назначена
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "";

            // Формируем набор claims для JWT
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("guid", user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role)
            };

            // Генерируем JWT-токен, используя секрет из конфигурации
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Возвращаем JWT-токен
            return Ok(new { token = tokenString });
        }
    }

    // Класс для получения запроса (ожидается только поле Email)
    public class LoginRequest
    {
        public string Email { get; set; }
    }
}
