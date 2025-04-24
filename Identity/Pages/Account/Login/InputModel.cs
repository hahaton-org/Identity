using System.ComponentModel.DataAnnotations;

namespace Identity.Pages.Account.Login
{
    public class InputModel
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Неверный формат Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Выберите роль")]
        public string LoginRole { get; set; }

        // Новая функция "Запомнить меня"
        public bool RememberMe { get; set; }
    }
}
