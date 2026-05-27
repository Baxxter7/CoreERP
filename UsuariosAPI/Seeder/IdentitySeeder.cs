using Microsoft.AspNetCore.Identity;
using UsuariosAPI.Models;

namespace UsuariosAPI.Seeder;
public class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var roles = new[] { "Admin", "Seller", "Manager" };

        foreach(var role in roles)
        {
            if(!await roleManager.RoleExistsAsync(role))
            {
                var newRole = new ApplicationRole(role);
                await roleManager.CreateAsync(newRole);
            }
        }

        var adminEmail = "administrador@mail.com";
        var password = "Admin123@";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

        if(existingAdmin == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Estado = true
            };
        }
           
    }
}
