using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Identity.Models; // Здесь находится ApplicationUser

namespace IdentityServer.Infrastructure
{
    public class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public ClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            IList<string> roles = await UserManager.GetRolesAsync(user);
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);
            // Если у пользователя есть роли, добавляем первую как claim
            if (roles.Any())
            {
                identity.AddClaim(new Claim(JwtClaimTypes.Role, roles.First()));
            }
            return identity;
        }
    }
}
