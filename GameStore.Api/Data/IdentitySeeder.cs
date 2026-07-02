using GameStore.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Api.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        const string adminRole = "Admin";
        const string adminEmail = "admin@gamestore.com";
        const string adminPassword = "Admin123!";

        // create role
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        // create user
        var user = await userManager.FindByEmailAsync(adminEmail);

        if (user == null)
        {
            user = new User
            {
                UserName = "Admin",
                Email = adminEmail,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(user, adminPassword);
            await userManager.AddToRoleAsync(user, adminRole);
        }
    }
}