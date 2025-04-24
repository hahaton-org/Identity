using System.ComponentModel.DataAnnotations;

namespace Identity.Pages.Account.Login
{
    public class InputModel
    {
        [Required(ErrorMessage = "Email ����������")]
        [EmailAddress(ErrorMessage = "�������� ������ Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "������ ����������")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "�������� ����")]
        public string LoginRole { get; set; }

        // ����� ������� "��������� ����"
        public bool RememberMe { get; set; }
    }
}
