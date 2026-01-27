using Folio.Core.Interfaces;
using Folio.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Folio.Infrastructure.Identity
{
    public static class IdentityDependencyInjection
    {
        public static IServiceCollection AddIdentityInfrastructure
            (this IServiceCollection services,IConfiguration configuration)
        {
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();

            return services;
        }
    }
}
