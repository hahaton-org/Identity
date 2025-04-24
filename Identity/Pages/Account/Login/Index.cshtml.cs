using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using static IdentityServer.Pages.Account.Login.LoginOptions;

namespace Identity.Pages.Account.Login
{
    public class IndexModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(SignInManager<IdentityUser> signInManager,
                          UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        // ������ ��� ������ ����������� � ��������������
        public ViewModel View { get; set; }

        // ������ ��������� ����� ��� ����������� ������
        public IEnumerable<SelectListItem> RoleOptions { get; set; }

        // ������ �������� ������������� ��� �������� �����
        private readonly List<(string Email, string Password, string Role)> _testUsers = new()
        {
            ("admin@gmail.com", "123", "Administrator"),
            ("partner@gmail.com", "123", "Partner"),
            ("volunteer@gmail.com", "123", "Volunteer")
        };

        public void OnGet()
        {
            Input = new InputModel();
            View = new ViewModel();
            LoadRoleOptions();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                LoadRoleOptions();
                return Page();
            }

            if (!IsLocalLoginAllowed())
            {
                View = new ViewModel { SuccessMessage = string.Empty, ErrorMessage = "Local login is disabled." };
                LoadRoleOptions();
                return Page();
            }

            // ��������� �������� �������������
            var matchedTestUser = _testUsers.Find(user =>
                user.Email == Input.Email &&
                user.Password == Input.Password &&
                user.Role == Input.LoginRole);

            if (matchedTestUser != default)
            {
                View = new ViewModel
                {
                    SuccessMessage = $"�������� ����: {matchedTestUser.Role}",
                    ErrorMessage = string.Empty
                };

                LoadRoleOptions();
                return Page();
            }

            // ���� �������� ������������ �� ������, ��������� ����������� ������ ASP.NET Identity
            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                View = new ViewModel
                {
                    SuccessMessage = string.Empty,
                    ErrorMessage = InvalidCredentialsErrorMessage
                };
                LoadRoleOptions();
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                View = new ViewModel
                {
                    SuccessMessage = string.Empty,
                    ErrorMessage = InvalidCredentialsErrorMessage
                };
                LoadRoleOptions();
                return Page();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains(Input.LoginRole))
            {
                await _signInManager.SignOutAsync();
                View = new ViewModel
                {
                    SuccessMessage = string.Empty,
                    ErrorMessage = "��������� ���� �� ������������� ������� ������ ������������."
                };
                LoadRoleOptions();
                return Page();
            }

            string successMessage = "�������� ����: " + Input.LoginRole;
            View = new ViewModel
            {
                SuccessMessage = successMessage,
                ErrorMessage = string.Empty
            };

            return Page();
        }

        // ���������� ��� ������ (Logout)
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await _signInManager.SignOutAsync();
            // ������� ������ � ��������� ����� ������
            View = new ViewModel();
            Input = new InputModel();
            LoadRoleOptions();
            return Page();
        }

        private void LoadRoleOptions()
        {
            RoleOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Volunteer", Text = "�������" },
                new SelectListItem { Value = "Partner", Text = "������" },
                new SelectListItem { Value = "Administrator", Text = "�������������" }
            };
        }
    }
}
