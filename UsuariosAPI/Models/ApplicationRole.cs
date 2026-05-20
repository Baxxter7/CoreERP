using Microsoft.AspNetCore.Identity;

namespace UsuariosAPI.Models;
public class ApplicationRole : IdentityRole
{
    public bool Estado { get; set; } = true;
    public ApplicationRole() : base() { }
    public ApplicationRole(string roleName) : base(roleName) { }

}
