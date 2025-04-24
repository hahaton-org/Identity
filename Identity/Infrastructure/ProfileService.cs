using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;

namespace IdentityServer.Infrastructure
{
	public class ProfileService : IProfileService
	{
		public virtual async Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			var roleClaims = context.Subject.FindAll(JwtClaimTypes.Role);
			context.IssuedClaims.AddRange(roleClaims);
			await Task.CompletedTask;
		}
		public async Task IsActiveAsync(IsActiveContext context)
		{
			await Task.CompletedTask;
		}
	}
}
