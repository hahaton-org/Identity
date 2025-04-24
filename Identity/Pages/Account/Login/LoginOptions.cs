using System;

namespace IdentityServer.Pages.Account.Login
{
    public static class LoginOptions
    {
        // Основные настройки для авторизации
        public static bool AllowLocalLogin { get; set; } = true;
        public static bool AllowRememberLogin { get; set; } = true;
        public static TimeSpan RememberMeLoginDuration { get; set; } = TimeSpan.FromDays(30);

        // Новое свойство для сообщения об ошибке (текст из старой реализации)
        public static string InvalidCredentialsErrorMessage { get; set; } = "Invalid username or password";

        // Метод для проверки – разрешён ли локальный вход
        public static bool IsLocalLoginAllowed() => AllowLocalLogin;

        // Метод для проверки – разрешена ли функция "Запомнить меня"
        public static bool IsRememberLoginAllowed() => AllowRememberLogin;

        // Метод для получения продолжительности опции "Запомнить меня"
        public static TimeSpan GetRememberMeLoginDuration() => RememberMeLoginDuration;

        // Метод для единообразного обновления всех настроек авторизации.
        // (Если потребуется изменять настройки в рантайме)
        public static void SetLoginOptions(bool allowLocal, bool allowRemember, TimeSpan rememberDuration, string errorMessage)
        {
            AllowLocalLogin = allowLocal;
            AllowRememberLogin = allowRemember;
            RememberMeLoginDuration = rememberDuration;
            InvalidCredentialsErrorMessage = errorMessage;
        }
    }
}
