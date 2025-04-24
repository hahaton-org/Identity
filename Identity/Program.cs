using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Identity.Data; // Пространство, где определён ApplicationDbContext

var builder = WebApplication.CreateBuilder(args);

// Регистрируем контекст базы данных с использованием In‑Memory базы для демонстрации.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TestDb"));

// Регистрируем ASP.NET Core Identity.
// Здесь используется стандартный тип IdentityUser и IdentityRole.
// Если нужно использовать IdentityUser<Guid>, можно заменить типы и соответственно настроить ApplicationDbContext.
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Настраиваем параметры пароля (для демонстрации ослабим требования)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Добавляем поддержку Razor Pages.
builder.Services.AddRazorPages();

// Настраиваем обработку cookie-аутентификации (страница входа)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login/Index"; // Путь к странице входа
});

var app = builder.Build();

// Конфигурация middleware

// Для разработки выводим детальную информацию об ошибках
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // В продуктиве можно использовать собственную страницу обработки ошибок и HSTS
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Включаем аутентификацию и авторизацию
app.UseAuthentication();
app.UseAuthorization();

// Роутинг для Razor Pages. Страница входа должна располагаться по адресу /Account/Login/Index.
app.MapRazorPages();

app.Run();
