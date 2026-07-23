using Class_Library__.NET_.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AmazonClone.Infrastructure.Authentication
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            string[] roles = { "User", "Admin", "Customer" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole(roleName));
                }
            }
        }
    }
}
