using Microsoft.AspNetCore.Identity;
using UsuariosAPI.Models;

namespace UsuariosAPI.Seeder;
public static class IdentitySeeder
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

        string adminEmail = "administrador@mail.com";
        string adminPassword = "Admin123@";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

        if(existingAdmin == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Nombre = "Administrador",
                Estado = true
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);

            if (createResult.Succeeded)
                await userManager.AddToRolesAsync(adminUser, new[] { "Admin" });
            else
            {
                string? errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Error al crear el usuario Administrador: {errors}");
            }
        }
           
    }
}
