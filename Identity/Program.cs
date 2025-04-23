using System.Globalization;
using Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Добавляем Serilog
    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    // Конфигурация локализации
    builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

    // Добавляем Razor Pages
    builder.Services.AddRazorPages()
        .AddViewLocalization()
        .AddDataAnnotationsLocalization();

    // Добавляем аутентификацию и авторизацию
    builder.Services.AddAuthentication("Cookies")
        .AddCookie("Cookies", options =>
        {
            options.LoginPath = "/Account/Login/Index";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    // Настройка локализации
    var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("ru") };
    app.UseRequestLocalization(new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture("ru"),
        SupportedCultures = supportedCultures,
        SupportedUICultures = supportedCultures
    });

    // Конвейер обработки запросов
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorPages();

    // Логика заполнения базы данных (при необходимости)
    if (args.Contains("/seed"))
    {
        Log.Information("Seeding database...");
        // TODO: Добавить логику заполнения базы данных
        Log.Information("Done seeding database. Exiting.");
        return;
    }

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
