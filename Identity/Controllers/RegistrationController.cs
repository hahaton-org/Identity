using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public RegistrationController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // POST: api/Registration
        // Ожидается, что в теле запроса передается массив GUID в виде строк, например:
        // [ "6f9619ff-8b86-d011-b42d-00cf4fc964ff", "3c3f9f35-3e5b-4470-bbf3-d5d4b2f0d1b1" ]
        [HttpPost]
        public async Task<IActionResult> RegisterUsers([FromBody] List<string> guidList)
        {
            const string fixedPassword = "123456";
            var createdUsers = new List<string>();
            var failedUsers = new List<object>();

            foreach (var guidStr in guidList)
            {
                // Валидируем, что строка является корректным GUID.
                if (!Guid.TryParse(guidStr, out Guid guid))
                {
                    failedUsers.Add(new { id = guidStr, error = "Неверный формат GUID" });
                    continue;
                }

                // Генерируем email на основе GUID
                string email = guidStr + "@example.com";

                // Проверяем, существует ли уже пользователь с таким email.
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    // Можно либо пропустить, либо добавить в список уже существующих.
                    continue;
                }

                // Создаем нового пользователя.
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email
                };

                var result = await _userManager.CreateAsync(user, fixedPassword);
                if (result.Succeeded)
                {
                    createdUsers.Add(guidStr);
                }
                else
                {
                    failedUsers.Add(new { id = guidStr, errors = result.Errors.Select(e => e.Description) });
                }
            }

            return Ok(new { created = createdUsers, failed = failedUsers });
        }
    }
}
