using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Pages.Account.Login
{
    public class Index : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public ViewModel View { get; set; } = new ViewModel();

        public class InputModel
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public bool RememberLogin { get; set; }
            public string ReturnUrl { get; set; } = string.Empty;
        }

        public class ViewModel
        {
            public bool EnableLocalLogin { get; set; } = true;
            public bool AllowRememberLogin { get; set; } = true;
            public List<ExternalProvider> VisibleExternalProviders { get; set; } = new();
        }

        public class ExternalProvider
        {
            public string DisplayName { get; set; } = string.Empty;
            public string AuthenticationScheme { get; set; } = string.Empty;
        }

        public void OnGet(string? returnUrl)
        {
            Input.ReturnUrl = returnUrl ?? "~/";

            // Заполняем провайдеров
            View.VisibleExternalProviders.Add(new ExternalProvider
            {
                DisplayName = "Google",
                AuthenticationScheme = "Google"
            });
            View.VisibleExternalProviders.Add(new ExternalProvider
            {
                DisplayName = "Facebook",
                AuthenticationScheme = "Facebook"
            });
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Пример проверки данных
            if (Input.Email == "test@example.com" && Input.Password == "password123")
            {
                if (Url.IsLocalUrl(Input.ReturnUrl))
                {
                    return Redirect(Input.ReturnUrl);
                }

                return Redirect("~/");
            }

            ModelState.AddModelError(string.Empty, "Неверный Email или пароль.");
            return Page();
        }
    }
}
