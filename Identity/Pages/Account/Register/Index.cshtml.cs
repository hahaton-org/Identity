using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Pages.Account.Register
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public ViewModel View { get; set; } = new ViewModel();

        public void OnGet()
        {
            Input = new InputModel();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Регистрация производится только для партнёров – роль задаём по умолчанию.
            var role = "Partner";

            // Создаем нового пользователя с Email как логином.
            var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                // Назначаем пользователю роль "Partner"
                var roleResult = await _userManager.AddToRoleAsync(user, role);
                if (!roleResult.Succeeded)
                {
                    View.ErrorMessage = "Ошибка при назначении роли: " + string.Join(" ", roleResult.Errors.Select(e => e.Description));
                    return Page();
                }

                // Автоматически выполняем вход после успешной регистрации
                await _signInManager.SignInAsync(user, isPersistent: false);
                View.SuccessMessage = "Регистрация прошла успешно!";
                // Перенаправляем, например, на главную страницу.
                return RedirectToPage("/Index");
            }

            View.ErrorMessage = "Ошибка регистрации: " + string.Join(" ", result.Errors.Select(e => e.Description));
            return Page();
        }
    }
}
