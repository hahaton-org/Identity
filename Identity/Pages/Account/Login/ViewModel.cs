namespace Identity.Pages.Account.Login
{
    public class ViewModel
    {
        // Текст успешного входа, например "Успешный вход: Волонтер"
        public string SuccessMessage { get; set; }
        // Сообщение об ошибке, если вход неудачный
        public string ErrorMessage { get; set; }
    }
}
