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

            // ����������� ������������ ������ ��� �������� � ���� ����� �� ���������.
            var role = "Partner";

            // ������� ������ ������������ � Email ��� �������.
            var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                // ��������� ������������ ���� "Partner"
                var roleResult = await _userManager.AddToRoleAsync(user, role);
                if (!roleResult.Succeeded)
                {
                    View.ErrorMessage = "������ ��� ���������� ����: " + string.Join(" ", roleResult.Errors.Select(e => e.Description));
                    return Page();
                }

                // ������������� ��������� ���� ����� �������� �����������
                await _signInManager.SignInAsync(user, isPersistent: false);
                View.SuccessMessage = "����������� ������ �������!";
                // ��������������, ��������, �� ������� ��������.
                return RedirectToPage("/Index");
            }

            View.ErrorMessage = "������ �����������: " + string.Join(" ", result.Errors.Select(e => e.Description));
            return Page();
        }
    }
}
