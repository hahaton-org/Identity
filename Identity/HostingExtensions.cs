using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Identity.Data;
using Identity.Models;
using IdentityServer.Infrastructure; // ������������ ���� � ������ ���������� ��������
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Identity
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // ����������� �������� � ClaimsPrincipalFactory
            // ���������, ��� ��� ����� ClaimsPrincipalFactory ��������� ����������� �� UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
            builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ClaimsPrincipalFactory>();

            // ����������� �������� � ProfileService
            builder.Services.AddScoped<IProfileService, ProfileService>();

            builder.Services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    // ���������: https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                    options.EmitStaticAudienceClaim = true;
                })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddAspNetIdentity<ApplicationUser>();

            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    // ������� �������������� ������ �� Google:
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });

            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapRazorPages()
                .RequireAuthorization();

            return app;
        }
    }
}
