using System;

namespace IdentityServer.Pages.Account.Login
{
    public static class LoginOptions
    {
        // �������� ��������� ��� �����������
        public static bool AllowLocalLogin { get; set; } = true;
        public static bool AllowRememberLogin { get; set; } = true;
        public static TimeSpan RememberMeLoginDuration { get; set; } = TimeSpan.FromDays(30);

        // ����� �������� ��� ��������� �� ������ (����� �� ������ ����������)
        public static string InvalidCredentialsErrorMessage { get; set; } = "Invalid username or password";

        // ����� ��� �������� � �������� �� ��������� ����
        public static bool IsLocalLoginAllowed() => AllowLocalLogin;

        // ����� ��� �������� � ��������� �� ������� "��������� ����"
        public static bool IsRememberLoginAllowed() => AllowRememberLogin;

        // ����� ��� ��������� ����������������� ����� "��������� ����"
        public static TimeSpan GetRememberMeLoginDuration() => RememberMeLoginDuration;

        // ����� ��� �������������� ���������� ���� �������� �����������.
        // (���� ����������� �������� ��������� � ��������)
        public static void SetLoginOptions(bool allowLocal, bool allowRemember, TimeSpan rememberDuration, string errorMessage)
        {
            AllowLocalLogin = allowLocal;
            AllowRememberLogin = allowRemember;
            RememberMeLoginDuration = rememberDuration;
            InvalidCredentialsErrorMessage = errorMessage;
        }
    }
}
