using System.ComponentModel.DataAnnotations;

namespace Identity.Pages.Account.Register
{
    public class InputModel
    {
        // Полное имя
        [Required]
        [Display(Name = "ФИО")]
        public string FullName { get; set; } = string.Empty;

        // Электронная почта
        [Required]
        [EmailAddress]
        [Display(Name = "Электронная почта")]
        public string Email { get; set; } = string.Empty;

        // Название организации
        [Required]
        [StringLength(100, ErrorMessage = "Название организации не может превышать 100 символов.")]
        [Display(Name = "Название организации")]
        public string OrganizationName { get; set; } = string.Empty;

        // Пароль
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Пароль должен быть длиной не менее 6 символов.", MinimumLength = 6)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;

        // Подтверждение пароля
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // ИНН (идентификационный номер налогоплательщика)
        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "ИНН должен состоять из 10 цифр.")]
        [Display(Name = "ИНН")]
        public string INN { get; set; } = string.Empty;

        // Название роли
        [Required]
        [Display(Name = "Роль")]
        public string RoleName { get; set; } = string.Empty;

        // URL для перенаправления
        [Display(Name = "URL для возврата")]
        public string ReturnUrl { get; set; } = string.Empty;

        public Roles Roles { get; set; }
    }
}
