using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// Регистрируем контекст базы данных с использованием In‑Memory базы (для демонстрации)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TestDb"));

// Регистрируем ASP.NET Core Identity с ослаблёнными настройками пароля
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Добавляем поддержку Razor Pages
builder.Services.AddRazorPages();

// Добавляем поддержку контроллеров для API
builder.Services.AddControllers();

// Настраиваем обработку cookie-аутентификации (страница входа)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login/Index"; // Путь к странице входа
});

// Добавляем JWT Bearer аутентификацию для API (не переопределяем дефолтную схему Identity)
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Для демонстрации можно отключить, в продуктиве включите
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            // Читаем ключ из конфигурации (убедитесь, что Jwt:Secret задан в appsettings.json)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? "DefaultSecretForDemoUseOnly0123456")),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();

// Конфигурация middleware
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

// Маппинг контроллеров для API (например, AuthController)
app.MapControllers();

app.Run();
